using System.Collections.Generic;

namespace GraphUpdateDataAccess.DbModel
{
    public class ChildB
    {
        public ChildB()
        {
            this.GrandChildB1s = new List<GrandChildB1>();
            this.GrandChildB2s = new List<GrandChildB2>();
        }

        public int Id { get; set; }
        public long? Field1 { get; set; }

        public virtual ICollection<GrandChildB1> GrandChildB1s { get; set; }
        public virtual ICollection<GrandChildB2> GrandChildB2s { get; set; }
    }
}