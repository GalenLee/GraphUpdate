using System.Linq;
using GraphUpdateDataAccess.Context;
using GraphUpdate;
using GraphUpdateDataAccess.DbModel;
using System.Data.Entity;
using GraphUpdateDataAccess.DomainModel;

namespace GraphUpdateDataAccess.Service
{
    public class Service
    {
        private readonly GraphUpdateContextBase context;
        private readonly GraphUpdater graphUpdater;

        public Service(GraphUpdateContextBase context, GraphUpdater graphUpdater)
        {
            this.context = context;
            this.graphUpdater = graphUpdater;
        }

        public ParentA GetParentA(int id)
        {
            return this.context.ParentAs
                .Include(x => x.ChildA1s)
                .Include(x => x.ChildA2s)
                .Include(x => x.ChildA1s.Select(y => y.GrandChildA1s))
                .FirstOrDefault(x => x.Id == id);
        }

        public ParentA GetParentANoTracking(int id)
        {
            return
                this.context.ParentAs
                .Include(x => x.ChildA1s)
                .Include(x => x.ChildA2s)
                .Include(x => x.ChildA1s.Select(y => y.GrandChildA1s))
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        public ParentA InsertParentA(ParentA parentA)
        {
            var newObject = this.graphUpdater.Insert<ParentA, ParentA>(this.context, parentA);
            this.context.SaveChanges();
            return newObject;
        }

        public void UpdateParentA(ParentA modelParentA, ParentA dbParentA)
        {
            this.graphUpdater.Update(this.context, modelParentA, dbParentA);
            this.context.SaveChanges();
        }

        public void DeleteParentA(ParentA dbParentA)
        {
            this.graphUpdater.Delete(this.context, dbParentA);
            this.context.SaveChanges();
        }


        public ParentB GetParentB(int id)
        {
            return this.context.ParentBs
                 .Include(x => x.ChildBs)
                 .Include(x => x.ChildBs.Select(y => y.GrandChildB1s))
                 .Include(x => x.ChildBs.Select(y => y.GrandChildB2s))
                 .FirstOrDefault(x => x.Id == id);
        }

        public DmParentB GetDmParentB(int id)
        {
            var dmParentB = new DmParentB();
            var parentB = this.context.ParentBs
                 .Include(x => x.ChildBs)
                 .Include(x => x.ChildBs.Select(y => y.GrandChildB1s))
                 .Include(x => x.ChildBs.Select(y => y.GrandChildB2s))
                 .AsNoTracking()
                 .FirstOrDefault(x => x.Id == id);
            if (parentB == null)
            {
                return null;
            }

            dmParentB.Id = parentB.Id;
            dmParentB.Field1 = parentB.Field1;
            dmParentB.Field2 = parentB.Field2;
            dmParentB.Field3 = parentB.Field3;
            foreach (var childB in parentB.ChildBs)
            {
                var dmChildB = new DmChildB();
                dmParentB.ChildBs.Add(dmChildB);
                dmChildB.Id = childB.Id;
                dmChildB.Field1 = childB.Field1;

                foreach (var grandChildB1 in childB.GrandChildB1s)
                {
                    var dmGrandChildB1 = new DmGrandChildB1();
                    dmChildB.GrandChildB1s.Add(dmGrandChildB1);
                    dmGrandChildB1.Id = grandChildB1.Id;
                    dmGrandChildB1.Field1 = grandChildB1.Field1;
                    dmGrandChildB1.Field2 = grandChildB1.Field2;
                }

                foreach (var grandChildB2 in childB.GrandChildB2s)
                {
                    var dmGrandChildB2 = new DmGrandChildB2();
                    dmChildB.GrandChildB2s.Add(dmGrandChildB2);
                    dmGrandChildB2.Id = grandChildB2.Id;
                    dmGrandChildB2.Field1 = grandChildB2.Field1;
                }
            }

            return dmParentB;
        }

        public ParentB InsertParentB(DmParentB dmParentB)
        {
            var newObject = this.graphUpdater.Insert<DmParentB, ParentB>(this.context, dmParentB);
            this.context.SaveChanges();
            return newObject;
        }

        public void UpdateParentB(DmParentB modelParentB, ParentB dbParentB)
        {
            this.graphUpdater.Update(this.context, modelParentB, dbParentB);
            this.context.SaveChanges();
        }

        public void DeleteParentB(ParentB dbParentB)
        {
            this.graphUpdater.Delete(this.context, dbParentB);
            this.context.SaveChanges();
        }
    }
}
