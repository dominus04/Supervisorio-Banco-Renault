using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;

namespace Supervisório_Banco_Renault.Data
{
    public class AppDbContext : DbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=supervisorio.db");
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.TagRFID).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.AccessLevel).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.IsDeleted).HasDefaultValue(false);

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
