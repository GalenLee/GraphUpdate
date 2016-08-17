using System.Collections.Generic;

namespace GraphUpdate
{
    /// <summary>
    /// Interface for all entity mappers.  Gets primary key values, 
    /// maps from model entity to db entity and gets navigations.
    /// </summary>
    public interface IEntityMapper
    {
        /// <summary>
        /// Gets the primary key value on the model entity.
        /// </summary>
        long GetModelPrimaryKey(object modelEntity);

        /// <summary>
        /// Gets the primary key value on the db entity.
        /// </summary>
        long GetDbPrimaryKey(object dbEntity);

        /// <summary>
        /// Maps the simple properties from the model entity to the db entity.
        /// </summary>
        void MapProperties(object modelEntity, object dbEntity);

        /// <summary>
        /// Returns an index value for each navigation property.
        /// GetModelNavigation and GetDbNavigation uses this value
        /// to get the corresponding navigation.
        /// </summary>
        IEnumerable<int> GetNavigationPropertyIndexes();

        /// <summary>
        /// Gets the navigation for index parameter on the model entity.
        /// </summary>
        object GetModelNavigation(int index, object modelEntity);

        /// <summary>
        /// Gets the navigation for index parameter on the db entity.
        /// </summary>
        object GetDbNavigation(int index, object dbEntity);
    }
}
