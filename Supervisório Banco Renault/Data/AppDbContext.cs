using Microsoft.EntityFrameworkCore;
using Supervisório_Banco_Renault.Models;
using System.IO;

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
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<OP10_Traceability> OP10_Traceabilities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            #region User db settings
            modelBuilder.Entity<User>().HasKey(u => u.Id);
            modelBuilder.Entity<User>().Property(u => u.Name).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.TagRFID).IsRequired();
            modelBuilder.Entity<User>().HasIndex(U => U.TagRFID).IsUnique();
            modelBuilder.Entity<User>().Property(u => u.AccessLevel).IsRequired();
            modelBuilder.Entity<User>().Property(u => u.IsDeleted).HasDefaultValue(false);

            modelBuilder.Entity<User>().HasQueryFilter(u => !u.IsDeleted);
            modelBuilder.Entity<User>().HasData(
                    new User
                    {
                        Id = new System.Guid("00000000-0000-0000-0000-000000000001"),
                        Name = "Admin",
                        TagRFID = "01972265660",
                        AccessLevel = Models.Enums.AccessLevel.SuperUser
                    }
                );
            #endregion

            #region Recipe db settings

            modelBuilder.Entity<Recipe>().HasKey(r => r.Id);
            modelBuilder.Entity<Recipe>().Property(r => r.ModuleCode).IsRequired();
            modelBuilder.Entity<Recipe>().HasIndex(r => r.ModuleCode).IsUnique();

            #endregion

            #region OP10_Traceability db settings

            modelBuilder.Entity<OP10_Traceability>().HasKey(t => t.Id);
            modelBuilder.Entity<OP10_Traceability>().Property(t => t.RadiatorCode).IsRequired();
            modelBuilder.Entity<OP10_Traceability>().Property(t => t.CondenserCode).IsRequired();
            modelBuilder.Entity<OP10_Traceability>().Property(t => t.OP20_Executed).HasDefaultValue(false);
            modelBuilder.Entity<OP10_Traceability>().HasOne(t => t.User).WithMany().HasForeignKey(t => t.UserId);

            #endregion

        }
    }
}
