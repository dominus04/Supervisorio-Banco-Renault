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
            string dbPath = "D:/Indusbras/Programação/Supervisórios/Supervisório Banco Renault OP10 e OP20/Supervisório Banco Renault/Supervisório Banco Renault/supervisorio.db";
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
            modelBuilder.Entity<User>().Property(u => u.HashedPassword).IsRequired();

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
        }
    }
}
