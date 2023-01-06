using DomainServices.Repos;
using DomainServices.Services;
using Moq;

namespace Domain.Tests
{
    public class DomainTests
    {
        //UC2_AC2 - Canteen employee can see packages from other canteens
        [Fact]
        public Task UC2_AC2_CanteenEmployee_Sees_Other_Canteens_Offers()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

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

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllPackagesBasic())
                .Returns(packages.AsQueryable);

            var result = canteenServices.FilterCanteenPackages(canteen2.City, canteen2.Location);

            var comparePackages = packages.Where(p => p.Canteen.City == canteen2.City)
                .Where(p => p.Canteen.Location == canteen2.Location).ToList();
            Assert.Equal(result, comparePackages);
            return Task.CompletedTask;
        }

        //UC3_AC1 - Canteen employee can use crud on packages
        [Fact]
        public async Task UC3_AC1_CanteenEmployee_Can_Use_Crud_On_Package()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Peer",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/HLRqlU9.png"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Gezond pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product>{ product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                EighteenPlus = false,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            //Add Package
            productRepoMock.Setup(productRepo => productRepo.GetProductById(product.Id)).Returns(product);
            packageRepoMock.Setup(packageRepo => packageRepo.AddPackage(package)).Returns(Task.FromResult(true));

            var addResult = canteenServices.AddPackage(package.Id, package, new List<int>{ product.Id });
            Assert.True(await addResult);

            //Edit package
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package.Id)).Returns(package);
            packageRepoMock.Setup(packageRepo => packageRepo.DeletePackageById(package.Id))
                .Returns(Task.FromResult(true));

            var editResult = canteenServices.EditPackageWithProducts(package.Id, package, new List<int> { product.Id });
            Assert.Equal("success", await editResult);

            //Delete package
            var deleteResult = canteenServices.DeletePackageById(package.Id);
            Assert.Equal("success", await deleteResult);
        }

        //UC3_AC2 - Canteen employee can only edit/remove package with no reservation
        [Fact]
        public async Task UC3_AC2_CanteenEmployee_Can_Only_Edit_Or_Remove_A_Package_With_No_Reservations()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Peer",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/HLRqlU9.png"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Gezond pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product> { product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                EighteenPlus = false,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = 1
            };

            //Edit package
            productRepoMock.Setup(productRepo => productRepo.GetProductById(product.Id)).Returns(product);
            packageRepoMock.Setup(packageRepo => packageRepo.AddPackage(package)).Returns(Task.FromResult(true));

            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package.Id)).Returns(package);
            packageRepoMock.Setup(packageRepo => packageRepo.DeletePackageById(package.Id))
                .Returns(Task.FromResult(true));

            var editResult = canteenServices.EditPackageWithProducts(package.Id, package, new List<int> { product.Id });
            Assert.Equal("already-reserved", await editResult);

            //Delete package
            var deleteResult = canteenServices.DeletePackageById(package.Id);
            Assert.Equal("already-reserved", await deleteResult);
        }

        //UC3_AC3 - Canteen employee has overview of packages at his/her location
        [Fact]
        public Task UC3_AC3_CanteenEmployee_Sees_Packages_At_Own_Location()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

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
                CanteenId = canteen1.Id,
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
                    CanteenId = 2,
                    Canteen = canteen2,
                    PickupTime = new DateTime(2022, 11, 12, 13, 15, 00),
                    AvailableTill = new DateTime(2022, 11, 12, 17, 15, 00),
                    EighteenPlus = true,
                    Price = decimal.Parse("12,50"),
                    Category = (int) Category.Drank,
                    ReservedByStudentId = null
                }
            };

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllPackagesBasic()).Returns(packages.OrderBy(a => a.AvailableTill).AsQueryable());

            var result = canteenServices.FilterCanteenPackages(canteenEmployee.Canteen.City, canteenEmployee.Canteen.Location); 
            Assert.Single(result);

            return Task.CompletedTask;
        }

        //UC4_AC1 - Product has alcohol indicator
        [Fact]
        public Task UC4_AC1_Product_Has_Alcohol_Indicator()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Peer",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/HLRqlU9.png"
            };

            productRepoMock.Setup(productRepo => productRepo.GetProductById(product.Id)).Returns(product);

            var result = canteenServices._productRepo.GetProductById(product.Id);
            Assert.Equal("ContainsAlcohol", result.GetType().GetProperty("ContainsAlcohol")?.Name);
            return Task.CompletedTask;
        }

        //UC4_AC2 - A package gets a 18+ indicator based on the products inside the package
        [Fact]
        public async Task UC4_AC2_Package_Has_Eighteen_plus_Indicator_Automatically()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var productRepoMock = new Mock<IProductRepo>();
            var canteenServices = new CanteenServices(packageRepoMock.Object, productRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Hertog Jan",
                ContainsAlcohol = true,
                Picture = "https://i.imgur.com/InH4TUw.jpg"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Bier pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product> { product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(2022, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(2022, 11, 16, 17, 15, 00),
                EighteenPlus = false,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            //Add Package
            productRepoMock.Setup(productRepo => productRepo.GetProductById(product.Id)).Returns(product);
            packageRepoMock.Setup(packageRepo => packageRepo.AddPackage(package)).Returns(Task.FromResult(true));

            var addResult = canteenServices.AddPackage(package.Id, package, new List<int> { product.Id });
            Assert.True(await addResult);
            Assert.True(package.EighteenPlus);
        }

        //UC4_AC3 - Student under 18 can't order a alcoholic package
        [Fact]
        public async Task UC4_AC3_Student_Cant_Order_Eighteen_Plus_Package()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            var packageServices = new PackageServices(packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "De Boer",
                Email = "henkdb@gmail.com",
                Birthday = new DateTime((DateTime.Now.Year - 17), 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Hertog Jan",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/InH4TUw.jpg"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Bier pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product> { product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(DateTime.Now.Year, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(DateTime.Now.Year, 11, 16, 17, 15, 00),
                EighteenPlus = true,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            studentRepoMock.Setup(studentRepo => studentRepo.GetStudentById(student.Id)).Returns(student);
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package.Id)).Returns(package);

            var result = packageServices.ReservePackageById(1, package.Id);
            Assert.Equal("not-18", await result);
        }

        //UC5_AC1 - Student can order a package
        [Fact]
        public async Task UC5_AC1_Student_Can_Order_A_Package()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            var packageServices = new PackageServices(packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "De Boer",
                Email = "henkdb@gmail.com",
                Birthday = new DateTime((DateTime.Now.Year - 20), 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Hertog Jan",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/InH4TUw.jpg"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Bier pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product> { product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(DateTime.Now.Year, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(DateTime.Now.Year, 11, 16, 17, 15, 00),
                EighteenPlus = true,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            studentRepoMock.Setup(studentRepo => studentRepo.GetStudentById(student.Id)).Returns(student);
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package.Id)).Returns(package);

            var result = packageServices.ReservePackageById(1, package.Id);
            Assert.Equal("success", await result);
        }

        //UC5_AC2 - Student can only order 1 package on a pickup day
        [Fact]
        public async Task UC5_AC2_Student_Can_Only_Order_One_Package_Per_Pickup_Day()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            var packageServices = new PackageServices(packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var student = new Student()
            {
                Id = 1,
                FirstName = "Henk",
                LastName = "De Boer",
                Email = "henkdb@gmail.com",
                Birthday = new DateTime((DateTime.Now.Year - 20), 2, 15),
                StudentNumber = 2184500,
                StudyCity = City.Breda,
                PhoneNumber = "+31612345678"
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Hertog Jan",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/InH4TUw.jpg"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Bier pakket",
                Description = "Lekker gezond pakket met fruit aan te raden voor elke student",
                Products = new List<Product> { product },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(DateTime.Now.Year, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(DateTime.Now.Year, 11, 16, 17, 15, 00),
                EighteenPlus = true,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            var package2 = new Package()
            {
                Id = 2,
                Name = "Broodjes pakket",
                Description = "Heerlijke broodjes als lunch of als tussendoortje",
                Products = new List<Product>(),
                CanteenId = 2,
                Canteen = canteen1,
                PickupTime = new DateTime(DateTime.Now.Year, 11, 16, 14, 15, 00),
                AvailableTill = new DateTime(DateTime.Now.Year, 11, 16, 18, 15, 00),
                EighteenPlus = false,
                Price = decimal.Parse("6,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null,
            };

            studentRepoMock.Setup(studentRepo => studentRepo.GetStudentById(student.Id)).Returns(student);
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package.Id)).Returns(package);

            //Reserve a package
            var firstOrderResult = packageServices.ReservePackageById(1, package.Id);
            Assert.Equal("success", await firstOrderResult);

            List<Package> returnPackageList = new List<Package> { package2 };
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackageById(package2.Id)).Returns(package2);
            packageRepoMock.Setup(packageRepo => packageRepo.GetPackagesFromLoggedInStudent(student.Email))
                .Returns(returnPackageList.AsQueryable);

            //Try to reserve a second package on the same day
            var secondOrderResult = packageServices.ReservePackageById(1, package2.Id);
            Assert.Equal("already-reservation", await secondOrderResult);
        }

        //UC6_AC1 - Package has products
        [Fact]
        public Task UC6_AC1_Package_Has_Products()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            var packageServices = new PackageServices(packageRepoMock.Object, studentRepoMock.Object);

            //Test data
            Canteen canteen1 = new Canteen()
            {
                Id = 1,
                Location = (int)Location.Ld,
                City = (int)City.Breda
            };

            var product = new Product()
            {
                Id = 1,
                Name = "Peer",
                ContainsAlcohol = false,
                Picture = "https://i.imgur.com/HLRqlU9.png"
            };

            var product2 = new Product()
            {
                Id = 2,
                Name = "Hertog Jan",
                ContainsAlcohol = true,
                Picture = "https://i.imgur.com/InH4TUw.jpg"
            };

            var package = new Package()
            {
                Id = 1,
                Name = "Random pakket",
                Description = "Een pakket met verschillende producten",
                Products = new List<Product> { product, product2 },
                CanteenId = 1,
                Canteen = canteen1,
                PickupTime = new DateTime(DateTime.Now.Year, 11, 16, 13, 15, 00),
                AvailableTill = new DateTime(DateTime.Now.Year, 11, 16, 17, 15, 00),
                EighteenPlus = true,
                Price = decimal.Parse("4,50"),
                Category = (int)Category.Fruit,
                ReservedByStudentId = null
            };

            Assert.Contains(product, package.Products);
            Assert.Contains(product2, package.Products);
            return Task.CompletedTask;
        }

        //UC8_AC1 - Student can filter on location & UC8_AC2 Student can filter on meal category
        [Fact]
        public Task UC8_AC1_2_Student_Can_Filter_Packages()
        {
            var packageRepoMock = new Mock<IPackageRepo>();
            var studentRepoMock = new Mock<IStudentRepo>();
            var packageServices = new PackageServices(packageRepoMock.Object, studentRepoMock.Object);

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

            packageRepoMock.Setup(packageRepo => packageRepo.GetAllUnreservedPackages())
                .Returns(packages.AsQueryable);

            var result = packageServices.FilterPackages(canteen2.Location, (int) Category.Drank);

            var comparePackages = packages.Where(p => p.Canteen.Location == canteen2.Location)
                .Where(p => p.Category == (int)Category.Drank).ToList();

            Assert.Equal(result, comparePackages);
            return Task.CompletedTask;
        }
    }
}