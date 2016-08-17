using System.Collections.Generic;
using GraphUpdate;
using GraphUpdateDataAccess.DbModel;

namespace GraphUpdateDataAccess.CustomMappers
{
    public class ChildA1Mapper : IEntityMapper
    {
        public long GetModelPrimaryKey(object modelEntity)
        {
            return ((ChildA1)modelEntity).Id;
        }

        public long GetDbPrimaryKey(object dbEntity)
        {
            return ((ChildA1)dbEntity).Id;
        }

        public void MapProperties(object modelEntity, object dbEntity)
        {
            var from = (ChildA1)modelEntity;
            var to = (ChildA1)dbEntity;
            to.P1 = from.P1;
            to.P2 = from.P2;
            to.P3 = from.P3;
        }

        public IEnumerable<int> GetNavigationPropertyIndexes()
        {
            yield return 0;
        }

        public object GetModelNavigation(int index, object modelEntity)
        {
            return ((ChildA1)modelEntity).GrandChildA1s;
        }

        public object GetDbNavigation(int index, object dbEntity)
        {
            return ((ChildA1)dbEntity).GrandChildA1s;
        }
    }
}
