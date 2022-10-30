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
using static System.Formats.Asn1.AsnWriter;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Portal.Models;

namespace Portal.Tests
{
    public class ControllerTests
    {
        //UseCase 1 - Page with packages
        [Fact]
        public void Package_Index_Should_Return_Unreserved_Packages_In_Model()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(new IdentityUser()
                {

                    Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                    UserName = "Henk",
                    Email = "Henk@gmail.com"
                });

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();

            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

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
            var packagesInModel = result.Model as List<Package>;
            Assert.Equal(2, packagesInModel.Count);
        }

        //UseCase 1 - Page with my reservations
        [Fact]
        public async Task MyPackages_Should_Return_Student_Reserved_Packages_In_Model()
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

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

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

        //UseCase 2 - Canteen offer
        [Fact]
        public async Task CanteenEmployee_Sees_Canteen_Offers()
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

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var canteenEmployeeRepoMock = new Mock<ICanteenEmployeeRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenRepoMock = new Mock<ICanteenRepo>();

            AccountController accountController =
                new AccountController(userManagerMock.Object, null, packageRepoMock.Object,
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
            };

            canteenEmployeeRepoMock.Setup(canteenEmployeeRepo =>
                    canteenEmployeeRepo.GetCanteenEmployeeByEmail(canteenEmployee.Email))
                .Returns(canteenEmployee);

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllPackagesBasic())
                .Returns(packages.Where(p => p.Canteen.Location == canteenEmployee.Canteen.Location)
                    .Where(p => p.Canteen.City == canteenEmployee.Canteen.City).AsQueryable);



            var result = await accountController.CanteenPackages() as ViewResult;
            var packagesInModel = result.Model as List<Package>;
            Assert.Single(packagesInModel);
        }

        //UseCase 3 - Offer a package
        [Fact]
        public async Task CanteenEmployee_Add_Package()
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

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var canteenEmployeeRepoMock = new Mock<ICanteenEmployeeRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenRepoMock = new Mock<ICanteenRepo>();

            AccountController accountController =
                new AccountController(userManagerMock.Object, null, packageRepoMock.Object,
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
            List<Canteen> canteens = new List<Canteen>
            {
                new Canteen()
                {
                    Id = 1,
                    Location = (int)Location.Ld,
                    City = (int)City.Breda
                }
            };

            ICollection<Product> products = new List<Product>
            {
                new Product()
                {
                    Id = 2,
                    Name = "Peer",
                    ContainsAlcohol = false,
                    Picture = "https://i.imgur.com/HLRqlU9.png"
                }
            };

            var packageVM = new AddPackageViewModel()
            {
                Id = 1,
                Name = "Gezond pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                SelectedProducts = new List<int>(products.ToList()[0].Id),
                CanteenId = canteens.ToList()[0].Id,
                PickupTime = DateTime.Now.AddHours(1),
                AvailableTill = DateTime.Now.AddHours(3),
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
            };

            productRepoMock.Setup(productRepo => productRepo.GetProductById(2)).Returns(products.ToList()[0]);

            Package package = new Package
            {
                Name = packageVM.Name,
                Description = packageVM.Description,
                Products = products,
                CanteenId = packageVM.CanteenId,
                PickupTime = packageVM.PickupTime,
                AvailableTill = packageVM.AvailableTill,
                EighteenPlus = false,
                Price = packageVM.Price,
                Category = packageVM.Category
            };

            var result = await accountController.CanteenAddPackage(packageVM) as ViewResult;
            Assert.Equal("Pakket is succesvol toegevoegd", result.ViewData["Message"]);
        }

        // UseCase 4 - User has to be 18 plus for a 18 plus package
        [Fact]
        public async Task Package_Only_For_Eighteen_Plus()
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

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

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
                Birthday = new DateTime(2010, 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            studentRepoMock.Setup(s => s.GetStudentByEmail(identityUser.Email)).Returns(student);

            int packageId = 2;
            packageRepoMock.Setup(r => r.ReservePackageById(student, packageId))
                .Returns(Task.FromResult("not-18"));

            var result = await packageController.ReservePackage(packageId) as RedirectToActionResult;
            Assert.Equal("Je moet 18+ zijn om dit 18+ pakket te bestellen", result.RouteValues["errorMessage"]);
        }

        //UseCase 5 - Reserve Package
        [Fact]
        public async Task Reserve_package_Only_One_Per_Day()
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

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

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
                Birthday = new DateTime(2010, 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            studentRepoMock.Setup(s => s.GetStudentByEmail(identityUser.Email)).Returns(student);

            packageRepoMock.Setup(r => r.ReservePackageById(student, 2))
                .Returns(Task.FromResult("success"));

            var result = await packageController.ReservePackage(2) as RedirectToActionResult;
            Assert.Equal("Pakket is succesvol gereserveerd", result.RouteValues["successMessage"]);

            packageRepoMock.Setup(r => r.ReservePackageById(student, 1))
                .Returns(Task.FromResult("already-reservation"));

            var result2 = await packageController.ReservePackage(1) as RedirectToActionResult;
            Assert.Equal("Je hebt al een pakket gereserveerd op deze dag", result2.RouteValues["errorMessage"]);

        }

        //UseCase 6 - Products in package
        [Fact]
        public async Task Products_In_Packages()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(new IdentityUser()
                {

                    Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                    UserName = "Henk",
                    Email = "Henk@gmail.com"
                });

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();

            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            Product product1 = new Product()
            {
                Id = 1,
                Name = "Appel",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/Dy86B5w.png"
            };
            Product product2 = new Product()
            {
                Id = 2,
                Name = "Peer",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/HLRqlU9.png"
            };

            var packages = new List<Package>()
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>
                    {
                        product1
                    },
                    CanteenId = 1,
                    PickupTime = DateTime.Now.AddHours(1),
                    AvailableTill = DateTime.Now.AddHours(3),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Gezond pakket 2",
                    Description = "Heerlijke peren",
                    Products = new List<Product>
                    {
                        product2
                    },
                    CanteenId = 2,
                    PickupTime = DateTime.Now.AddHours(1),
                    AvailableTill = DateTime.Now.AddHours(3),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
            };

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllUnreservedPackages())
                .Returns(packages.AsQueryable());

            var result = packageController.Index() as ViewResult;
            var packagesInModel = result.Model as List<Package>;
            Assert.Contains(product1, packagesInModel[0].Products);
            Assert.Contains(product2, packagesInModel[1].Products);
        }

        //UseCase 8 - Products in package
        [Fact]
        public async Task Filter_On_Location_And_MealType()
        {
            var user = new Mock<IUserStore<IdentityUser>>();
            user.Setup(x => x.FindByIdAsync("2fc4c69d-3efd-41b8-8b57-7b45e0457b2d", CancellationToken.None))
                .ReturnsAsync(new IdentityUser()
                {

                    Id = "2fc4c69d-3efd-41b8-8b57-7b45e0457b2d",
                    UserName = "Henk",
                    Email = "Henk@gmail.com"
                });

            var userManagerMock =
                new Mock<UserManager<IdentityUser>>(user.Object, null, null, null, null, null, null, null, null);
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();

            PackageController packageController =
                new PackageController(userManagerMock.Object, packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            List<Canteen> canteens = new List<Canteen>
            {
                new Canteen()
                {
                    Id = 1,
                    Location = (int)Location.Ld,
                    City = (int)City.Breda
                },
                new Canteen()
                {
                    Id = 2,
                    Location = (int)Location.Ha,
                    City = (int)City.Tilburg
                }
            };

            var packages = new List<Package>()
            {
                new Package()
                {
                    Id = 1,
                    Name = "Gezond pakket",
                    Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                    Products = new List<Product>(),
                    CanteenId = 1,
                    Canteen = canteens[0],
                    PickupTime = DateTime.Now.AddHours(1),
                    AvailableTill = DateTime.Now.AddHours(3),
                    EighteenPlus = false,
                    Price = decimal.Parse("4,50"),
                    Category = (int)Category.Fruit,
                    ReservedByStudentId = null
                },
                new Package()
                {
                    Id = 2,
                    Name = "Broodjes pakket",
                    Description = "Heerlijke broodjes",
                    Products = new List<Product>(),
                    CanteenId = 2,
                    Canteen = canteens[1],
                    PickupTime = DateTime.Now.AddHours(1),
                    AvailableTill = DateTime.Now.AddHours(3),
                    EighteenPlus = false,
                    Price = decimal.Parse("6,50"),
                    Category = (int)Category.Brood,
                    ReservedByStudentId = null
                },
            };

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllUnreservedPackages())
                .Returns(packages.AsQueryable());

            //Filter category
            var result = packageController.FilterPackages(-1, (int)Category.Fruit);
            var packagesInModel = result.Model as List<Package>;
            foreach (Package package in packagesInModel)
            {
                Assert.Equal((int)Category.Fruit, package.Category);
            }

            //Filter city
            var result2 = packageController.FilterPackages((int) Location.Ld, -1);
            var packagesInModel2 = result2.Model as List<Package>;
            foreach (Package package in packagesInModel2)
            {
                Assert.Equal((int)Location.La, package.Canteen.City);
            }
        }
    }
}