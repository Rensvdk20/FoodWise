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
        //private readonly UserManager<IdentityUser> _userManager;

        public FoodWiseIdentityDbContext(DbContextOptions<FoodWiseIdentityDbContext> contextOptions) : base(contextOptions)
        {

        }

        protected override async void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(new IdentityRole
            {
                Id = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                Name = "CanteenEmployee",
                NormalizedName = "CANTEENEMPLOYEE"
            });

            var hasher = new PasswordHasher<IdentityUser>();

            modelBuilder.Entity<IdentityUser>().HasData(
                new IdentityUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb9",
                    UserName = "Helma",
                    NormalizedUserName = "HELMA",
                    Email = "Helma@avanskantine.nl",
                    NormalizedEmail = "HELMA@AVANSKANTINE.NL",
                    PasswordHash = hasher.HashPassword(null, "12345")
                });

            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb9"
                });

            //const string CANTEEN_EMPLOYEE_CLAIM = "UserType";

            //const string CANTEEN_EMPLOYEE_1 = "Helma";
            //const string CANTEEN_EMPLOYEE_1_PASSWORD = "12345";

            //IdentityUser canteenEmployee1 = await _userManager.FindByIdAsync(CANTEEN_EMPLOYEE_1);
            //if (canteenEmployee1 == null)
            //{
            //    canteenEmployee1 = new IdentityUser(CANTEEN_EMPLOYEE_1);

            //    await _userManager.CreateAsync(canteenEmployee1, CANTEEN_EMPLOYEE_1_PASSWORD);
            //    await _userManager.AddClaimAsync(canteenEmployee1,
            //        new Claim(CANTEEN_EMPLOYEE_CLAIM, "canteenEmployee"));
            //}


        }
    }
}
