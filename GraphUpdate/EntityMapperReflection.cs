using System;
using System.Collections.Generic;
using System.Reflection;

namespace GraphUpdate
{
    /// <summary>
    /// Entity mapper class that uses reflection to get the primary keys,
    /// mapped properties and navigations.
    /// </summary>
    public class EntityMapperReflection : IEntityMapper
    {
        private readonly PropertyInfo modelIdPropertyInfo;
        private readonly PropertyInfo dbIdPropertyInfo;
        private readonly IList<PropertyInfoPair> mappingPairs;
        private readonly IList<PropertyInfoPair> navigationPairs;

        /// <summary>
        /// Construtor which takes property infos.
        /// </summary>
        public EntityMapperReflection(
            PropertyInfo modelIdPropertyInfo,
            PropertyInfo dbIdPropertyInfo,
            IList<PropertyInfoPair> mappingPairs,
            IList<PropertyInfoPair> navigationPairs)
        {
            this.modelIdPropertyInfo = modelIdPropertyInfo;
            this.dbIdPropertyInfo = dbIdPropertyInfo;
            this.mappingPairs = mappingPairs;
            this.navigationPairs = navigationPairs;
        }

        /// <summary>
        /// Gets the model entity's primary key.
        /// </summary>
        public long GetModelPrimaryKey(object modelEntity)
        {
            try
            {
                return (long)Convert.ChangeType(this.modelIdPropertyInfo.GetValue(modelEntity, null), typeof(long));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not get primary key for model entity " + modelEntity.GetType(), ex);
            }
        }

        /// <summary>
        /// Gets the db entity's primary key.
        /// </summary>
        public long GetDbPrimaryKey(object dbEntity)
        {
            try
            {
                return (long)Convert.ChangeType(this.dbIdPropertyInfo.GetValue(dbEntity, null), typeof(long));
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not get primary key for db entity " + dbEntity.GetType(), ex);
            }
        }

        /// <summary>
        /// Maps the simple property values from model entity to db entity.
        /// </summary>
        public void MapProperties(object modelEntity, object dbEntity)
        {
            foreach (var pair in this.mappingPairs)
            {
                try
                {
                    var value = pair.ModelPropertyInfo.GetValue(modelEntity, null);
                    pair.DbPropertyInfo.SetValue(dbEntity, value);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException(
                        "Can not map property " + pair.DbPropertyInfo.Name + 
                        " from entity " + modelEntity.GetType() + 
                        " to entity " + dbEntity.GetType(), ex);
                }
            }
        }

        /// <summary>
        /// Gets navigation property identifiers.
        /// </summary>
        public IEnumerable<int> GetNavigationPropertyIndexes()
        {
            for (var index = 0; index < this.navigationPairs.Count; index++)
            {
                yield return index;
            }
        }

        /// <summary>
        /// Gets the model entity's navigation for navigation property identified by index.
        /// </summary>
        public object GetModelNavigation(int index, object modelEntity)
        {
            try
            {
                return this.navigationPairs[index].ModelPropertyInfo.GetValue(modelEntity, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not get navigation for model entity " + modelEntity.GetType(), ex);
            }
        }

        /// <summary>
        /// Gets the db entity's navigation for navigation property identified by index.
        /// </summary>
        public object GetDbNavigation(int index, object dbEntity)
        {
            try
            {
                return this.navigationPairs[index].DbPropertyInfo.GetValue(dbEntity, null);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Can not get navigation for db entity " + dbEntity.GetType(), ex);
            }
        }
    }
}
