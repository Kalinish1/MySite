using Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Core.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        public DbSet<Category> Categories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                    new Category { Id= 1, Name = "Action", DisplayOrder = 450},
                    new Category { Id = 2, Name = "SciFi", DisplayOrder = 100 },
                    new Category { Id = 3, Name = "History", DisplayOrder = 1004 }
                );
        }
    }
}
