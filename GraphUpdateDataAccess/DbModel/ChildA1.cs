using System;
using System.Collections.Generic;

namespace GraphUpdateDataAccess.DbModel
{
    public class ChildA1
    {
        public ChildA1()
        {
            this.GrandChildA1s = new HashSet<GrandChildA1>();
        }

        public int Id { get; set; }
        public string P1 { get; set; }
        public DateTime P2 { get; set; }
        public DateTime? P3 { get; set; }

        public virtual ParentA ParentA { get; set; }
        public virtual ICollection<GrandChildA1> GrandChildA1s { get; set; }
    }
}
