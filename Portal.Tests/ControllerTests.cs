using System.Collections;
using System.Net;
using Domain;
using DomainServices.Repos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Portal.Controllers;
using System.Security.Claims;
using System.Security.Principal;
using DomainServices.Services.Intf;
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portal.Models;

namespace Portal.Tests
{
    public class ControllerTests
    {
        //UC1_AC1 - Page with packages
        [Fact]
        public void UC1_AC1_Package_Index_Should_Return_Unreserved_Packages_In_Model()
        {
            var packageServicesMock = new Mock<IPackageServices>();
            var user = new Mock<IUserStore<IdentityUser>>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();

            PackageController packageController =
                new PackageController(packageServicesMock.Object, userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            var packages = new List<Package>()
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
            };

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllUnreservedPackages())
                .Returns(packages.AsQueryable());

            var result = packageController.Index() as ViewResult;
            var packagesInModel = result.Model as IEnumerable<Package>;
            Assert.Equal(2, packagesInModel.Count());
        }

        //UC1_AC2 - Page with my reservations
        [Fact]
        public async Task UC1_AC2_MyPackages_Should_Return_Student_Reserved_Packages_In_Model()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            var identityUser = new IdentityUser()
            {

                Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                UserName = "Henk",
                NormalizedUserName = "Henk".ToUpper(),
                Email = "Henk@gmail.com",
                NormalizedEmail = "Henk@gmail.com".ToUpper()
            };

            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(identityUser);

            var packageServicesMock = new Mock<IPackageServices>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController = new PackageController(packageServicesMock.Object, userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            var userPrinicipal = new Mock<ClaimsPrincipal>();
            packageController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrinicipal.Object
                }
            };

            userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<IdentityUser>(), "Student"));

            //Test data
            Student student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "Toren",
                Email = "Henk@gmail.com",
                Birthday = new DateTime(2001, 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            studentRepoMock.Setup(s => s.GetStudentByEmail(identityUser.Email)).Returns(student);

            var packages = new List<Package>
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudent = student,
                    ReservedByStudentId = 1
                },
            };

            packageRepoMock.Setup(packageRepo => packageRepo.GetPackagesFromLoggedInStudent(student.Email))
                .Returns(packages.Where(p => p.ReservedByStudentId == student.Id).AsQueryable);

            var result = await packageController.MyPackages() as ViewResult;
            var packagesInModel = result.Model as List<Package>;
            Assert.Single(packagesInModel);
        }

        //UC2_AC1 - Canteen employee sees own canteen packages
        [Fact]
        public async Task UC2_AC1_CanteenEmployee_Sees_Own_Canteen_Offers()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            var identityUser = new IdentityUser()
            {

                Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                UserName = "Helma",
                NormalizedUserName = "Helma".ToUpper(),
                Email = "Helma@avanscanteen.nl",
                NormalizedEmail = "Helma@avanscanteen.nl".ToUpper()
            };

            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(identityUser);

            var canteenServicesMock = new Mock<ICanteenServices>();
            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var canteenEmployeeRepoMock = new Mock<ICanteenEmployeeRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenRepoMock = new Mock<ICanteenRepo>();

            AccountController accountController =
                new AccountController(canteenServicesMock.Object, userManagerMock.Object, null, packageRepoMock.Object,
                    canteenEmployeeRepoMock.Object, productRepoMock.Object, canteenRepoMock.Object);

            var userPrinicipal = new Mock<ClaimsPrincipal>();
            accountController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrinicipal.Object
                }
            };

            userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(userManager =>
                userManager.IsInRoleAsync(It.IsAny<IdentityUser>(), "CanteenEmployee"));

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            Canteen canteen2 = new Canteen()
            {
                Id = 2,
                Location = (int)Location.La,
                City = (int)City.Breda
            };

            CanteenEmployee canteenEmployee = new CanteenEmployee()
            {
                Id = 1,
                Name = "Helma",
                Email = "Helma@avanscanteen.nl",
                EmployeeNumber = 555,
                CanteenId = 1,
                Canteen = canteen1
            };

            var packages = new List<Package>
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    Canteen = canteen1,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    Canteen = canteen2,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 3,
                    Name = "Drank pakket",
                    Description = "Verschillende soorten drank in 1 pakket",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    Canteen = canteen1,
                    PickupTime = new DateTime(2022, 11, 12, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 12, 17, 15, 00),
                    EighteenPlus = true,
                    Price = decimal.Parse("12,50"),
                    Category = (int) Category.Drank,
                    ReservedByStudentId = null
                }
            };

            canteenEmployeeRepoMock.Setup(canteenEmployeeRepo =>
                    canteenEmployeeRepo.GetCanteenEmployeeByEmail(canteenEmployee.Email))
                .Returns(canteenEmployee);

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllCanteenPackages(canteenEmployee)).Returns(packages
                .Where(c => c.Canteen.Location == canteenEmployee.Canteen.Location)
                .Where(c => c.Canteen.City == canteenEmployee.Canteen.City).OrderBy(a => a.AvailableTill)
                .AsQueryable());

            var result = await accountController.CanteenPackages() as ViewResult;
            var packagesInModel = result.Model as List<Package>;
            Assert.Equal(2, packagesInModel.Count());
        }

        // UC7_AC1 - If the package is already reserved, give a customer friendly error
        [Fact]
        public async Task UC7_AC1_Package_Already_Reserved_Gives_Error()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            var identityUser = new IdentityUser()
            {

                Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                UserName = "Henk",
                NormalizedUserName = "Henk".ToUpper(),
                Email = "Henk@gmail.com",
                NormalizedEmail = "Henk@gmail.com".ToUpper()
            };

            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(identityUser);

            var packageServicesMock = new Mock<IPackageServices>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController = new PackageController(packageServicesMock.Object, userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            var userPrinicipal = new Mock<ClaimsPrincipal>();
            packageController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrinicipal.Object
                }
            };

            userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<IdentityUser>(), "Student"));

            //Test data
            Student student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "Toren",
                Email = "Henk@gmail.com",
                Birthday = new DateTime(2001, 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            studentRepoMock.Setup(s => s.GetStudentByEmail(identityUser.Email)).Returns(student);

            var packages = new List<Package>
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudent = student,
                    ReservedByStudentId = 1
                },
            };

            packageServicesMock.Setup(packageService => packageService.ReservePackageById(student.Id, packages[1].Id)).Returns(Task.FromResult("error-reserved"));

            var result = await packageController.ReservePackage(packages[1].Id) as RedirectToActionResult;
            var packageReserveResponse = result.RouteValues["errorMessage"];
            Assert.Equal("Dit pakket is al gereserveerd", packageReserveResponse);
        }

        // UC7_AC2 - If the package is not reserved, the student gets the package
        [Fact]
        public async Task UC7_AC2_Reserve_Package_Succesfully()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            var identityUser = new IdentityUser()
            {

                Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                UserName = "Henk",
                NormalizedUserName = "Henk".ToUpper(),
                Email = "Henk@gmail.com",
                NormalizedEmail = "Henk@gmail.com".ToUpper()
            };

            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(identityUser);

            var packageServicesMock = new Mock<IPackageServices>();
            var userManagerMock = new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController = new PackageController(packageServicesMock.Object, userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            var userPrinicipal = new Mock<ClaimsPrincipal>();
            packageController.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = userPrinicipal.Object
                }
            };

            userManagerMock.Setup(userManager => userManager.FindByIdAsync(It.IsAny<string>()))
                .ReturnsAsync(identityUser);
            userManagerMock.Setup(userManager => userManager.IsInRoleAsync(It.IsAny<IdentityUser>(), "Student"));

            //Test data
            Student student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "Toren",
                Email = "Henk@gmail.com",
                Birthday = new DateTime(2001, 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            studentRepoMock.Setup(s => s.GetStudentByEmail(identityUser.Email)).Returns(student);

            var packages = new List<Package>
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes als lunch of als tussendoortje",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    PickupTime = new DateTime(2022, 11, 18, 14, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 18, 16, 15, 00),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudent = student,
                    ReservedByStudentId = 1
                },
            };

            packageServicesMock.Setup(packageService => packageService.ReservePackageById(student.Id, packages[1].Id)).Returns(Task.FromResult("success"));

            var result = await packageController.ReservePackage(packages[1].Id) as RedirectToActionResult;
            var packageReserveResponse = result.RouteValues["successMessage"];
            Assert.Equal("Pakket is succesvol gereserveerd", packageReserveResponse);
        }
    }
}