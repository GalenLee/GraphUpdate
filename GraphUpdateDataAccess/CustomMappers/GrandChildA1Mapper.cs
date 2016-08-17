using System;
using System.Collections.Generic;
using GraphUpdate;
using GraphUpdateDataAccess.DbModel;

namespace GraphUpdateDataAccess.CustomMappers
{
    public class GrandChildA1Mapper : IEntityMapper
    {
        public long GetModelPrimaryKey(object modelEntity)
        {
            return ((GrandChildA1)modelEntity).Id;
        }

        public long GetDbPrimaryKey(object dbEntity)
        {
            return ((GrandChildA1)dbEntity).Id;
        }

        public void MapProperties(object modelEntity, object dbEntity)
        {
            var from = (GrandChildA1)modelEntity;
            var to = (GrandChildA1)dbEntity;
            to.P1 = from.P1;
            to.P2 = from.P2;
            to.P3 = from.P3;
        }

        public IEnumerable<int> GetNavigationPropertyIndexes()
        {
            yield break;
        }

        public object GetModelNavigation(int index, object modelEntity)
        {
            throw new NotImplementedException();
        }

        public object GetDbNavigation(int index, object dbEntity)
        {
            throw new NotImplementedException();
        }
    }
}
