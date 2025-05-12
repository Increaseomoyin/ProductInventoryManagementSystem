using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProductInventoryManagementSystem.Models;

namespace ProductInventoryManagementSystem.Data
{
    public class DataContext :IdentityDbContext<AppUser>
    {
        public DataContext( DbContextOptions<DataContext> options) : base(options) 
        {
            
        }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProfileUser> ProfileUsers { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Creating Roles
            var identityRoles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Id = "1",
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                 new IdentityRole()
                {
                    Id = "2",
                    Name = "Manager",
                    NormalizedName = "MANAGER"
                },
                  new IdentityRole()
                {
                    Id = "3",
                    Name = "Worker",
                    NormalizedName = "WORKER"
                },
            };

            modelBuilder.Entity<IdentityRole>()
                .HasData(identityRoles);

            //Configuring the join table
            modelBuilder.Entity<ProductCategory>()
                .HasKey(pc => new { pc.ProductId, pc.CategoryId });
            //Doing the Product Part
            modelBuilder.Entity<ProductCategory>()
                .HasOne(p=> p.Product)
                .WithMany(pc=>pc.ProductCategories)
                .HasForeignKey(p=>p.ProductId);
            //Doing the Category Part
            modelBuilder.Entity<ProductCategory>()
                .HasOne(c => c.Category)
                .WithMany(pc => pc.ProductCategories)
                .HasForeignKey(c => c.CategoryId);

            //Setting the AppUser and User
            modelBuilder.Entity<ProfileUser>()
                .HasOne(u => u.AppUser)
                .WithOne() // <-- Add the reverse navigation
                .HasForeignKey<ProfileUser>(u => u.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);










        }
    }
}
