using System.Collections.Generic;

namespace GraphUpdateDataAccess.DbModel
{
    public class ParentB
    {
        public ParentB()
        {
            this.ChildBs = new List<ChildB>();
            this.FieldComplex = new List<string>();
        }

        public int Id { get; set; }
        public string Field1 { get; set; }
        public int Field2 { get; set; }
        public decimal Field3 { get; set; }
        public int FieldNoMatch { get; set; }
        public IList<string> FieldComplex { get; set; }

        public virtual IList<ChildB> ChildBs { get; set; }
    }
}
