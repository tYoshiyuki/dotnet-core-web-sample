using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DotNetCoreWebSample.Web.Models
{
    public class DotnetCoreWebSampleContext : DbContext
    {
        public DotnetCoreWebSampleContext(DbContextOptions<DotnetCoreWebSampleContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Sessions>(entity =>
            {
                entity.HasIndex(e => e.ExpiresAtTime)
                    .HasName("Index_ExpiresAtTime");

                entity.Property(e => e.Id)
                    .HasMaxLength(449)
                    .ValueGeneratedNever();

                entity.Property(e => e.Value).IsRequired();
            });
        }

        public DbSet<Sessions> Sessions { get; set; }

        public DbSet<Todo> Todo { get; set; }
    }
}
