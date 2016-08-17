using System.Collections.Generic;

namespace GraphUpdateDataAccess.DomainModel
{
    public class DmParentB
    {
        public DmParentB()
        {
            this.ChildBs = new List<DmChildB>();
            this.FieldComplex = new List<string>();
        }

        public int Id { get; set; }
        public string Field1 { get; set; }
        public int Field2 { get; set; }
        public decimal Field3 { get; set; }
        public int OtherField1 { get; set; }
        public string OtherField2 { get; set; }
        public IList<string> FieldComplex { get; set; }

        public virtual IList<DmChildB> ChildBs { get; set; }
    }
}
