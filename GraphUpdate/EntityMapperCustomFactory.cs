using System;
using System.Collections.Generic;

namespace GraphUpdate
{
    /// <summary>
    /// Allows for custom entity mappers.  Entity mappers are cached.
    /// </summary>
    public class EntityMapperCustomFactory : IEntityMapperFactory
    {
        private readonly IDictionary<Type, IEntityMapper> entityMappers;

        /// <summary>
        /// The passed in dictionary has key of type Type.  This is the db entity type.
        /// The db entity type is used to lookup the entity mapper.
        /// </summary>
        public EntityMapperCustomFactory(IDictionary<Type, IEntityMapper> entityMappers)
        {
            this.entityMappers = entityMappers;
        }

        /// <summary>
        /// Returns the entity mapper for the db entity using it's type.
        /// </summary>
        public virtual IEntityMapper Create(Type modelEntityType, Type dbEntityType)
        {
            return this.entityMappers[dbEntityType];
        }
    }
}
