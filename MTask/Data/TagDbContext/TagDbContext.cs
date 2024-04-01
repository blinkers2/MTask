using Microsoft.EntityFrameworkCore;
using MTask.Data.Entities;

namespace MTask.Data
{
    public class TagDbContext : DbContext
    {
        public TagDbContext(DbContextOptions<TagDbContext> options) : base(options) {}
        
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Tag>().ToTable("Tags");
            modelBuilder.Entity<Tag>().HasKey(t => t.Id);
            modelBuilder.Entity<Tag>().Property(t => t.Name).IsRequired().HasMaxLength(255);
            modelBuilder.Entity<Tag>().Property(t => t.Count).IsRequired();
            modelBuilder.Entity<Tag>().Property(t => t.PercentageInWholePopulation).HasColumnType("decimal(18,2)");
        }
    }
}