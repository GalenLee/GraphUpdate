using System;
using System.Collections.Generic;

namespace GraphUpdate
{
    /// <summary>
    /// Wrapper around another entiy mapper factory.  Entity mappers 
    /// are cached so they do not need to be created each time.
    /// Speeds test shows caching does not increase performance much.
    /// </summary>
    public class EntityMapperCachedFactory: IEntityMapperFactory
    {
        private readonly object lockObject = new object();

        private readonly IEntityMapperFactory containedFactory;

        /// <summary>
        /// Mapping for model entity to db entity.
        /// </summary>
        private readonly IDictionary<Type, IEntityMapper> mappersModelAndDb;

        /// <summary>
        /// Mapping used for removing db entities.  The model entity may not
        /// exist so we would have just look at the db entity when creating.
        /// </summary>
        private readonly IDictionary<Type, IEntityMapper> mappersDbOnly;

        /// <summary>
        /// Property to get the total calls to create.  Used in tests.
        /// </summary>
        public long TotalCreateCalls
        {
            get
            {
                lock (this.lockObject)
                {
                    return this.totalCreateCalls;
                }
            }
        }
        private long totalCreateCalls;

        /// <summary>
        /// constructor takes in the wrapped entity mapper factory.
        /// </summary>
        public EntityMapperCachedFactory(IEntityMapperFactory containedFactory)
        {
            this.containedFactory = containedFactory;
            this.mappersModelAndDb = new Dictionary<Type, IEntityMapper>();
            this.mappersDbOnly = new Dictionary<Type, IEntityMapper>();
        }

        /// <summary>
        /// Returns the cached entity mapper for the db entity using it's type.
        /// </summary>
        public IEntityMapper Create(Type modelEntityType, Type dbEntityType)
        {
            lock (this.lockObject)
            {
                if (modelEntityType != null)
                {
                    if (!this.mappersModelAndDb.ContainsKey(dbEntityType))
                    {
                        this.mappersModelAndDb.Add(dbEntityType, this.containedFactory.Create(modelEntityType, dbEntityType));
                        this.totalCreateCalls++;
                    }
                    return this.mappersModelAndDb[dbEntityType];
                }

                if (!this.mappersDbOnly.ContainsKey(dbEntityType))
                {
                    this.mappersDbOnly.Add(dbEntityType, this.containedFactory.Create(null, dbEntityType));
                    this.totalCreateCalls++;
                }

                return this.mappersDbOnly[dbEntityType];
            }
        }
    }
}
