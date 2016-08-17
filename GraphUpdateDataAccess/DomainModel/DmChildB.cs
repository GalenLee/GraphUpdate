using System.Collections.Generic;

namespace GraphUpdateDataAccess.DomainModel
{
    public class DmChildB
    {
        public DmChildB()
        { 
            this.GrandChildB1s = new List<DmGrandChildB1>();
            this.GrandChildB2s = new List<DmGrandChildB2>();
        }

        public int Id { get; set; }
        public long? Field1 { get; set; }
        public string OtherField1 { get; set; }

        public virtual ICollection<DmGrandChildB1> GrandChildB1s { get; set; }
        public virtual ICollection<DmGrandChildB2> GrandChildB2s { get; set; }
    }
}
