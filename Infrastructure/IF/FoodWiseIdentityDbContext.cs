using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Infrastructure.IF
{
    public class FoodWiseIdentityDbContext : IdentityDbContext
    {
        public FoodWiseIdentityDbContext(DbContextOptions<FoodWiseIdentityDbContext> contextOptions) : base(contextOptions)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ##### Roles #####
            IdentityRole canteenEmployeeRole = new IdentityRole
            {
                Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                Name = "CanteenEmployee",
                NormalizedName = "CanteenEmployee".ToUpper()
            };

            IdentityRole studentRole = new IdentityRole
            {
                Id = "769745ae-6d81-4b44-9df1-aca86729b89a",
                Name = "Student",
                NormalizedName = "Student".ToUpper()
            };

            modelBuilder.Entity<IdentityRole>().HasData(canteenEmployeeRole, studentRole);

            // ##### Users #####

            var hasher = new PasswordHasher<IdentityUser>();

            IdentityUser canteenEmployee1 = new IdentityUser
            {
                Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                UserName = "Helma",
                NormalizedUserName = "Helma".ToUpper(),
                Email = "Helma@avanscanteen.nl",
                NormalizedEmail = "Helma@avanscanteen.nl".ToUpper(),
                PasswordHash = hasher.HashPassword(null, "12345")
            };

            IdentityUser student1 = new IdentityUser
            {
                Id = "5217197a-f73e-4131-9b5a-dbbfa612a332",
                UserName = "Mark",
                NormalizedUserName = "Mark".ToUpper(),
                Email = "Mark@gmail.com",
                NormalizedEmail = "Mark@gmail.com".ToUpper(),
                PasswordHash = hasher.HashPassword(null, "1234")
            };

            modelBuilder.Entity<IdentityUser>().HasData(canteenEmployee1, student1);

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = canteenEmployeeRole.Id,
                    UserId = canteenEmployee1.Id
                },
                new IdentityUserRole<string>
                {
                    RoleId = studentRole.Id,
                    UserId = student1.Id
                }
            );
        }
    }
}
