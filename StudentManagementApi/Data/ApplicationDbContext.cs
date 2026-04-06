using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementApi.Models;

namespace StudentManagementApi.Data
{
    // Application database context - manages database connections and entity sets
    public class ApplicationDbContext : DbContext
    {
        // Constructor accepting DbContext options
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
            : base(options)
        {
        }

        // DbSet for Student entity - represents Students table
        public DbSet<Student> Students { get; set; }

        // Configure model relationships and constraints
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Student entity
            modelBuilder.Entity<Student>(entity =>
            {
                // Set primary key
                entity.HasKey(e => e.Id);

                // Configure Name property
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure Email property - unique index
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure Age property with validation
                entity.Property(e => e.Age)
                    .IsRequired()
                    .HasAnnotation("MinValue", 1)
                    .HasAnnotation("MaxValue", 150);

                // Configure Course property
                entity.Property(e => e.Course)
                    .IsRequired()
                    .HasMaxLength(100);

                // Configure CreatedDate with default value
                entity.Property(e => e.CreatedDate)
                    .HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
