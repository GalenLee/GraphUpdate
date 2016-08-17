using System.Collections.Generic;

namespace GraphUpdateDataAccess.DbModel
{
    public class ParentA
    {
        public ParentA()
        {
            this.ChildA1s = new HashSet<ChildA1>();
            this.ChildA2s = new HashSet<ChildA2>();
        }

        public int Id { get; set; }
        public string P1 { get; set; }
        public int P2 { get; set; }
        public int? P3 { get; set; }

        public virtual ICollection<ChildA1> ChildA1s { get; set; }
        public virtual ICollection<ChildA2> ChildA2s { get; set; }
    }
}
