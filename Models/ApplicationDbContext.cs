using Microsoft.EntityFrameworkCore;

namespace LibraryBookManagement.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ❗ Ensure title is unique
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title)
                .IsUnique();
        }
    }
}
