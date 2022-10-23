using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF
{
    public class FoodWiseDbContext : DbContext
    {
        public DbSet<Student> Students { get; set; }
        public DbSet<Package> Packages { get; set; }
        public DbSet<Product> Products { get; set; }
        //public DbSet<Category> Categories { get; set }
        //public DbSet<City> Cities { get; set; }
        public DbSet<Canteen> Canteens { get; set; }
        public DbSet<CanteenEmployee> CanteenEmployees { get; set; }

        public FoodWiseDbContext(DbContextOptions<FoodWiseDbContext> contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Student
            IEnumerable<Student> students = new List<Student>
            {
                new Student()
                {
                    Id = 1,
                    FirstName = "Mark",
                    LastName = "De Groot",
                    Email = "Mark@gmail.com",
                    Birthday = new DateTime(2001, 2, 15),
                    StudentNumber = 2184500,
                    StudyCity = City.Breda,
                    PhoneNumber = "+31612345678"
                }
            };

            //Canteen
            IEnumerable<Canteen> canteens = new List<Canteen>
            {
                new Canteen()
                {
                    Id = 1,
                    Location = (int) Location.Ld,
                    City = (int) City.Breda
                },
                new Canteen()
                {
                    Id = 2,
                    Location = (int) Location.La,
                    City = (int) City.Breda
                }
            };

            Product product1 = new Product()
            {
                Id = 1,
                Name = "Appel",
                containsAlcohol = false
            };
            Product product2 = new Product()
            {
                Id = 2,
                Name = "Peer",
                containsAlcohol = false
            };
            Product product3 = new Product()
            {
                Id = 3,
                Name = "Broodje Frikandel",
                containsAlcohol = false
            };
            Product product4 = new Product()
            {
                Id = 4,
                Name = "Kaiserbroodje",
                containsAlcohol = false
            };
            Product product5 = new Product()
            {
                Id = 5,
                Name = "Hertog Jan",
                containsAlcohol = true
            };

            //Package
            IEnumerable<Package> packages = new List<Package>
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = canteens.ToList()[0].Id,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int) Category.Fruit,
                    ReservedBy = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = canteens.ToList()[1].Id,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = true,
                    Price = decimal.Parse("6,50"),
                    Category = (int) Category.Fruit,
                    ReservedBy = null
                }
            };

            //Canteen employees
            IEnumerable<CanteenEmployee> canteenEmployees = new List<CanteenEmployee>
            {
                new CanteenEmployee()
                {
                    Id = 1,
                    Name = "Helma",
                    EmployeeNumber = 555,
                    CanteenId = canteens.ToList()[0].Id

                },
                new CanteenEmployee()
                {
                    Id = 2,
                    Name = "Erika",
                    EmployeeNumber = 678,
                    CanteenId = canteens.ToList()[1].Id
                }
            };

            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Student>().HasData(students);
            modelBuilder.Entity<Canteen>().HasData(canteens);
            modelBuilder.Entity<Package>().HasData(packages);
            modelBuilder.Entity<Product>().HasData(product1, product2, product3, product4, product5);
            modelBuilder.Entity<CanteenEmployee>().HasData(canteenEmployees);

            //##### Relations #####
            //Package and Canteen
            modelBuilder.Entity<Package>().HasOne(c => c.Canteen).WithMany(c => c.Packages);
            //Package and Product
            modelBuilder.Entity<Package>().HasMany(p => p.Products)
                .WithMany(p => p.Packages)
                .UsingEntity<Dictionary<string, object>>(
                    "Package_Product",
                    r => r.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                    l => l.HasOne<Package>().WithMany().HasForeignKey("PackageId"),
                    je =>
                    {
                        je.HasKey("PackageId", "ProductId");
                        je.HasData(
                            new { PackageId = 1, ProductId = 1 },
                            new { PackageId = 1, ProductId = 2 },
                            new { PackageId = 2, ProductId = 3 },
                            new { PackageId = 2, ProductId = 4 },
                            new { PackageId = 2, ProductId = 5 }
                        );
                    });

            //CanteenEmployee and Canteen
            modelBuilder.Entity<CanteenEmployee>().HasOne(c => c.Canteen);
        }
    }
}
