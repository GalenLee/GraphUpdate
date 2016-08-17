using System.Collections.Generic;
using GraphUpdate;
using GraphUpdateDataAccess.DbModel;

namespace GraphUpdateDataAccess.CustomMappers
{
    public class ParentAMapper: IEntityMapper
    {
        public long GetModelPrimaryKey(object modelEntity)
        {
            return ((ParentA)modelEntity).Id;
        }

        public long GetDbPrimaryKey(object dbEntity)
        {
            return ((ParentA)dbEntity).Id;
        }

        public void MapProperties(object modelEntity, object dbEntity)
        {
            var from = (ParentA)modelEntity;
            var to = (ParentA)dbEntity;
            to.P1 = from.P1;
            to.P2 = from.P2;
            to.P3 = from.P3;
        }

        public IEnumerable<int> GetNavigationPropertyIndexes()
        {
            // For more navigation properties yield return 1... etc.
            // And have GetModelNavigation and GetDbNavigation return navigation given index.
            yield return 0;
        }

        public object GetModelNavigation(int index, object modelEntity)
        {
            return ((ParentA)modelEntity).ChildA1s;
        }

        public object GetDbNavigation(int index, object dbEntity)
        {
            return ((ParentA)dbEntity).ChildA1s;
        }
    }
}
