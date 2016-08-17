namespace GraphUpdateDataAccess.DbModel
{
    public class GrandChildA1
    {
        public int Id { get; set; }
        public long P1 { get; set; }
        public byte P2 { get; set; }
        public bool? P3 { get; set; }
        public virtual ChildA1 ChildA1 { get; set; }
    }
}
