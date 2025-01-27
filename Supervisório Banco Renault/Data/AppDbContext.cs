using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Supervisório_Banco_Renault.Models;
using System.IO;
using System.Windows;

namespace Supervisório_Banco_Renault.Data
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string appDataDbFolder = Path.Combine(appDataPath, "Supervisorio Banco Renault");
            string dbPath = Path.Combine(appDataDbFolder, "supervisorio.db");

            if (!Directory.Exists(appDataDbFolder))
            {
                Directory.CreateDirectory(appDataDbFolder);
            }

            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.TagRFID).IsRequired();
            modelBuilder.Entity<User>().HasIndex(U => U.TagRFID).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.AccessLevel).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.IsDeleted).HasDefaultValue(false);

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<User>().HasData(
                    new User { 
                        Id = new System.Guid("00000000-0000-0000-0000-000000000001"), 
                        Name = "Admin", 
                        TagRFID = "01972265660", 
                        AccessLevel = Models.Enums.AccessLevel.SuperUser}
                );
        }
    }
}
