using Microsoft.EntityFrameworkCore;

namespace DotNetCoreWebSample.Web.Models
{
    public class DotnetCoreWebSampleContext : DbContext
    {
        public DotnetCoreWebSampleContext(DbContextOptions<DotnetCoreWebSampleContext> options) : base(options) { }

        public DotnetCoreWebSampleContext()
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasIndex(e => e.ExpiresAtTime)
                    .HasDatabaseName("Index_ExpiresAtTime");

                entity.Property(e => e.Id)
                    .HasMaxLength(449)
                    .ValueGeneratedNever();

                entity.Property(e => e.Value).IsRequired();
            });
        }

        public virtual DbSet<Sessions> Sessions { get; set; }

        public virtual DbSet<Todo> Todo { get; set; }
    }
}
