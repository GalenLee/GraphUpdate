using System.Data.Entity;
using GraphUpdateDataAccess.DbModel;

namespace GraphUpdateDataAccess.Context
{
    public class GraphUpdateContextBase : DbContext
    {
        public GraphUpdateContextBase() : base("name=GraphUpdateContext")
        {
        }

        // Test group A
        public virtual DbSet<ParentA> ParentAs { get; set; }
        public virtual DbSet<ChildA1> ChildA1s { get; set; }
        public virtual DbSet<ChildA2> ChildA2s { get; set; }
        public virtual DbSet<GrandChildA1> GradChildA1s { get; set; }

        // Test group B
        public DbSet<ParentB> ParentBs { get; set; }
        public DbSet<ChildB> ChildBs { get; set; }
        public DbSet<GrandChildB1> GradChildBs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Test group A
            modelBuilder.Entity<ParentA>()
                .HasMany(e => e.ChildA1s)
                .WithRequired(e => e.ParentA)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ParentA>()
                .HasMany(e => e.ChildA2s)
                .WithRequired(e => e.ParentA)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<ChildA1>()
                .HasMany(e => e.GrandChildA1s)
                .WithRequired(e => e.ChildA1)
                .WillCascadeOnDelete(false);

            // Test group B
            modelBuilder.Entity<ParentB>()
                .HasMany(e => e.ChildBs)
                .WithRequired()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChildB>()
                .HasMany(e => e.GrandChildB1s)
                .WithRequired()
                .WillCascadeOnDelete(true);

            modelBuilder.Entity<ChildB>()
                .HasMany(e => e.GrandChildB2s)
                .WithRequired()
                .WillCascadeOnDelete(true);
        }
    }
}
