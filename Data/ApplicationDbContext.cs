using Microsoft.EntityFrameworkCore;
using StudCrud.Models;
using System.Collections.Generic;

namespace StudCrud.Data
{
    // ApplicationDbContext.cs
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Subject> Subjects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between Student and Subject
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Subject)
                .WithMany(sub => sub.Students)
                .HasForeignKey(s => s.SubjectId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
