using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Text;

namespace GraphUpdate
{
    /// <summary>
    /// Code class that handles inserting, updating or removing disconnected graphs.
    /// The detached model entity is used to update the tracked db entity. The model
    /// entity does not need to be the same type as property values are mapped from 
    /// the model entity to the db entity.
    /// </summary>
    public class GraphUpdater
    {
        private readonly IEntityMapperFactory entityMapperFactory;

        /// <summary>
        /// Constructor which takes an entity mapper factory.
        /// </summary>
        public GraphUpdater(IEntityMapperFactory entityMapperFactory)
        {
            this.entityMapperFactory = entityMapperFactory;
        }

        /// <summary>
        /// Inserts a model entity graph into a newly created db entity which will
        /// be tracked by the context.  The db entity is not saved, so your code
        /// will need to call SaveChanges on the context.
        /// </summary>
        /// <typeparam name="T">Type of the model entity</typeparam>
        /// <typeparam name="TU">Type of the db entity</typeparam>
        /// <param name="context">The entity framework context</param>
        /// <param name="modelEntity">The model entity to map to a db entity to insert</param>
        /// <returns>A refernce to the db entity which can be used to get the identiy after the context calls SaveChanges</returns>
        public TU Insert<T, TU>(DbContext context, T modelEntity) where T : class, new() where TU : class, new()
        {
            if (modelEntity == null)
            {
                throw new ArgumentNullException(nameof(modelEntity));
            }

            var entityMapper = this.entityMapperFactory.Create(typeof(T), typeof(TU));

            var id = entityMapper.GetModelPrimaryKey(modelEntity);
            if (id > 0)
            {
                throw new InvalidOperationException("Can not insert entity with id > 0.  value: " + id);
            }

            var newDbEntity = new TU();
            this.Save(context, modelEntity, newDbEntity, entityMapper);
            context.Entry(newDbEntity).State = EntityState.Added;
            return newDbEntity;
        }

        /// <summary>
        /// Marks a db entity for deletion including children and grandchildren.
        /// Call SaveChanges on the context to delete the db entity.
        /// </summary>
        /// <typeparam name="TU">Type of the db entity</typeparam>
        /// <param name="context">The entity framework context</param>
        /// <param name="dbEntity">The db entity to delete</param>
        public void Delete<TU>(DbContext context, TU dbEntity) where TU: class, new()
        {
            if (dbEntity == null)
            {
                throw new ArgumentNullException(nameof(dbEntity));
            }

            var entityMapper = this.entityMapperFactory.Create(null, typeof(TU));
            this.Remove(context, dbEntity, entityMapper);
            context.Entry(dbEntity).State = EntityState.Deleted;
        }

        /// <summary>
        /// Maps the detached model entity to the tracked db entity.  Mapping properties,
        /// inserting, updating or deleting children and grandchildren as needed.  Call
        /// SaveChanges on the context to update the db entity.
        /// </summary>
        /// <typeparam name="T">Type of the model entity</typeparam>
        /// <typeparam name="TU">Type of the db entity</typeparam>
        /// <param name="context">The entity framework context</param>
        /// <param name="modelEntity">The model entity to </param>
        /// <param name="dbEntity">The modle entity to map to a db entity to update</param>
        public void Update<T, TU>(DbContext context, T modelEntity, TU dbEntity) where T : class, new() where TU : class, new()
        {
            if (modelEntity == null)
            {
                throw new ArgumentNullException(nameof(modelEntity));
            }

            if (dbEntity == null)
            {
                throw new ArgumentNullException(nameof(dbEntity));
            }

            var entityMapper = this.entityMapperFactory.Create(typeof(T), typeof(TU));

            var idModel = entityMapper.GetModelPrimaryKey(modelEntity);
            var idDb = entityMapper.GetDbPrimaryKey(dbEntity);
            if (idModel != idDb)
            {
                throw new InvalidOperationException("Can only update model and db entities that have same id values.");
            }

            this.Save(context, modelEntity, dbEntity, entityMapper);
        }

        private void Save<T, TU>(DbContext context, T modelEntity, TU dbEntity, IEntityMapper entityMapper)
        {
            entityMapper.MapProperties(modelEntity, dbEntity);

            foreach (var index in entityMapper.GetNavigationPropertyIndexes())
            {
                dynamic modelChildEntities = entityMapper.GetModelNavigation(index, modelEntity);
                dynamic dbChildEntities = entityMapper.GetDbNavigation(index, dbEntity);
                this.Save(context, modelChildEntities, dbChildEntities);
            }
        }

        private void Save<T, TU>(DbContext context, ICollection<T> modelEntities, ICollection<TU> dbEntities) where TU : class, new()
        {
            var entityMapper = this.entityMapperFactory.Create(typeof(T), typeof(TU));

            // Model entities with PK values > 0 are expected to exist in the database.
            var dictModelEntities = modelEntities
                .Where(x => entityMapper.GetModelPrimaryKey(x) > 0)
                .ToDictionary(x => entityMapper.GetModelPrimaryKey(x), x => x);

            // Model entities with PK values <= 0 are new entities.
            var newModelEntities = modelEntities.Where(x => entityMapper.GetModelPrimaryKey(x) <= 0).ToList();

            foreach (var dbEntity in dbEntities.ToList())
            {
                var id = entityMapper.GetDbPrimaryKey(dbEntity);
                if (dictModelEntities.ContainsKey(id))
                {
                    // Child db entity exists in model so update via save method.
                    var modelEntity = dictModelEntities[id];
                    this.Save(context, modelEntity, dbEntity, entityMapper);
                    dictModelEntities.Remove(id);
                }
                else
                {
                    // Child db entity does not exist in model so remove it from navigation.
                    // Needs to remove all children and set their entity state to deleted.
                    this.Remove(context, dbEntity, entityMapper);
                    dbEntities.Remove(dbEntity);
                    context.Entry(dbEntity).State = EntityState.Deleted;
                }
            }

            if (dictModelEntities.Count > 0)
            {
                var sb = new StringBuilder();
                sb.Append("Detached model entity(s) no longer exists in db.");
                foreach (var item in dictModelEntities)
                {
                    sb.Append(" Entity type: " + typeof(T) + ", Entity Id: " + item.Key);
                }
                throw new InvalidOperationException(sb.ToString());
            }

            foreach (var modelEntity in newModelEntities)
            {
                // Add new model entites to navigation.
                var dbEntity = new TU();
                this.Save(context, modelEntity, dbEntity, entityMapper);
                dbEntities.Add(dbEntity);
            }
        }

        private void Remove<TU>(DbContext context, TU dbEntity, IEntityMapper entityMapper)
        {
            foreach (var index in entityMapper.GetNavigationPropertyIndexes())
            {
                dynamic dbChildEntities = entityMapper.GetDbNavigation(index, dbEntity);
                this.Remove(context, dbChildEntities);
            }
        }

        private void Remove<TU>(DbContext context, ICollection<TU> dbEntities) where TU : class
        {
            var entityMapper = this.entityMapperFactory.Create(null, typeof(TU));
            foreach (var dbEntity in dbEntities.ToList())
            {
                // Loop on each db entity in navigation property and remove child.
                this.Remove(context, dbEntity, entityMapper);
                dbEntities.Remove(dbEntity);
                context.Entry(dbEntity).State = EntityState.Deleted;
            }
        }
    }
}
