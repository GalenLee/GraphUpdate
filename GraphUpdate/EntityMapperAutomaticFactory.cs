using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphUpdate
{
    /// <summary>
    /// Automatically determines mapping given model entity type and db entity type.
    /// Uses EntityMapperReflection.
    /// </summary>
    public class EntityMapperAutomaticFactory : IEntityMapperFactory
    {
        private readonly string identityProperty;

        /// <summary>
        /// Constructor which takes the name of the model and db entity primary key.
        /// </summary>
        public EntityMapperAutomaticFactory(string identityProperty)
        {
            this.identityProperty = identityProperty;
        }

        /// <summary>
        /// Creates the entity mapper automatically from the model entity's
        /// type and the db entity's type.
        /// </summary>
        public IEntityMapper Create(Type modelEntityType, Type dbEntityType)
        {
            var modelIdPropertyInfo = modelEntityType?.GetProperty(this.identityProperty);
            var dbIdPropertyInfo = dbEntityType.GetProperty(this.identityProperty);
            var mappingPairs = new List<PropertyInfoPair>();
            var navigationPairs = new List<PropertyInfoPair>();

            if (modelEntityType == null)
            {
                // Remove methods will not have model entity.  Only needs db entity info.
                navigationPairs.AddRange(from dbPropertyInfo in dbEntityType.GetProperties()
                                         where IsNavigationProperty(dbPropertyInfo.PropertyType)
                                         select new PropertyInfoPair { ModelPropertyInfo = null, DbPropertyInfo = dbPropertyInfo });
                return new EntityMapperReflection(null, dbIdPropertyInfo, null, navigationPairs);
            }

            if (modelIdPropertyInfo == null)
            {
                throw new InvalidOperationException("Model entity type does not have identity property: " + modelEntityType);
            }

            if (dbIdPropertyInfo == null)
            {
                throw new InvalidOperationException("Db entity type does not have identity property: " + dbEntityType);
            }

            foreach (var dbPropertyInfo in dbEntityType.GetProperties())
            {
                var modelPropertyInfo = modelEntityType.GetProperty(dbPropertyInfo.Name);
                if (modelPropertyInfo == null)
                {
                    // Property does not exist in both entities.
                    continue;
                }

                var isNavigationProperty = IsNavigationProperty(dbPropertyInfo.PropertyType);
                if (isNavigationProperty)
                {
                    if (!IsNavigationProperty(modelPropertyInfo.PropertyType))
                    {
                        throw new InvalidOperationException("Navigation property type mismatch " + dbPropertyInfo.PropertyType);
                    }

                    navigationPairs.Add(new PropertyInfoPair{ ModelPropertyInfo = modelPropertyInfo, DbPropertyInfo = dbPropertyInfo });
                    continue;
                }

                var isSimpleProperty = IsSimpleProperty(dbPropertyInfo.PropertyType);
                if (isSimpleProperty)
                {
                    mappingPairs.Add(new PropertyInfoPair{ ModelPropertyInfo = modelPropertyInfo, DbPropertyInfo = dbPropertyInfo} );
                }
            }

            return new EntityMapperReflection(modelIdPropertyInfo, dbIdPropertyInfo, mappingPairs, navigationPairs);
        }

        private static bool IsNavigationProperty(Type type)
        {
            var enumerableType = type
                .GetInterfaces()
                .FirstOrDefault(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            if (enumerableType == null)
            {
                return false;
            }
            var itemType = enumerableType.GetGenericArguments()[0];

            // FYI: Strings are IEnumerable<T> so make sure generic type is not a simple type.
            return !IsSimpleProperty(itemType);
        }

        /// <summary>
        /// Primitive, string, decimal, date related or nullable of simple type.
        /// </summary>
        private static bool IsSimpleProperty(Type type)
        {
            var isSimple = type.IsPrimitive || 
                type.IsEnum || 
                type == typeof(string) || 
                type == typeof(decimal) || 
                type == typeof(DateTime) ||
                type == typeof(TimeSpan) ||
                type == typeof(DateTimeOffset);
            if (isSimple)
            {
                return true;
            }

            var nullableType = Nullable.GetUnderlyingType(type);
            if (nullableType != null)
            {
                return IsSimpleProperty(nullableType);
            }

            return false;
        }
    }
}