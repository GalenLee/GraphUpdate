using System;
using System.Collections.Generic;

namespace GraphUpdate
{
    /// <summary>
    /// Create entity mappers that use reflection.  The configuration defines what
    /// properties are the primary keys, mapped properties and navigation properties.
    /// Entity mappers are cached.
    /// </summary>
    public class EntityMapperConfigurableFactory : IEntityMapperFactory
    {
        private Dictionary<Type, IEntityMapper> EntityMappers { get; }

        /// <summary>
        /// Constructor that takes in configuartion for each entity mapper to create.
        /// </summary>
        public EntityMapperConfigurableFactory(IList<EntityMapperConfigurableData> configuration)
        {
            this.EntityMappers = new Dictionary<Type, IEntityMapper>();
            foreach (var map in configuration)
            {
                try
                {
                    var modelIdPropertyInfo = map.ModelType.GetProperty(map.IdProperties.ModelProperty);
                    var dbIdPropertyInfo = map.DbType.GetProperty(map.IdProperties.DbProperty);
                    var mappingPairs = new List<PropertyInfoPair>();
                    var navigationPairs = new List<PropertyInfoPair>();

                    foreach (var pair in map.MappingPairs)
                    {
                        var modelPropertyInfo = map.ModelType.GetProperty(pair.ModelProperty);
                        var dbPropertyInfo = map.DbType.GetProperty(pair.DbProperty);
                        mappingPairs.Add(new PropertyInfoPair { ModelPropertyInfo = modelPropertyInfo, DbPropertyInfo = dbPropertyInfo });
                    }

                    foreach (var pair in map.NavigationPairs)
                    {
                        var modelPropertyInfo = map.ModelType.GetProperty(pair.ModelProperty);
                        var dbPropertyInfo = map.DbType.GetProperty(pair.DbProperty);
                        navigationPairs.Add(new PropertyInfoPair { ModelPropertyInfo = modelPropertyInfo, DbPropertyInfo = dbPropertyInfo });
                    }

                    this.EntityMappers.Add(
                        map.DbType,
                        new EntityMapperReflection(modelIdPropertyInfo, dbIdPropertyInfo, mappingPairs, navigationPairs));
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Exception setting up mapping for map with db type " + map.DbType, ex);
                }
            }
        }

        /// <summary>
        /// Returns the entity mapper for the db entity using it's type.
        /// </summary>
        public IEntityMapper Create(Type modelEntityType, Type dbEntityType)
        {
            return this.EntityMappers[dbEntityType];
        }
    }
}
