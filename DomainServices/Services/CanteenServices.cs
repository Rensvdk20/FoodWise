using Domain;
using DomainServices.Repos;
using DomainServices.Services.Intf;
using Microsoft.EntityFrameworkCore;

namespace DomainServices.Services
{
    public class CanteenServices : ICanteenServices
    {
        public readonly IPackageRepo _packageRepo;
        public readonly IProductRepo _productRepo;

        public CanteenServices(IPackageRepo packageRepo,
            IProductRepo productRepo)
        {
            _packageRepo = packageRepo;
            _productRepo = productRepo;
        }

        //The value -1 searches for all
        public IEnumerable<Package> FilterCanteenPackages(int searchCity = -1, int searchLocation = -1)
        {
            IQueryable<Package> packages = _packageRepo.GetAllPackagesBasic().Include(c => c.Canteen).OrderBy(a => a.AvailableTill);

            //The value -1 searches for all
            if (searchCity == -1 && searchLocation == -1)
            {
                return packages.ToList();
            }
            else if (searchCity != -1 && searchLocation == -1)
            {
                return packages.Where(l => l.Canteen.City == searchCity).ToList();
            }
            else if (searchCity == -1 && searchLocation != -1)
            {
                return packages.Where(l => l.Canteen.Location == searchLocation).ToList();
            }

            return packages.Where(l => l.Canteen.Location == searchLocation).Where(l => l.Canteen.City == searchCity).ToList();
        }

        public async Task<string> EditPackageWithProducts(int packageId, Package package, IEnumerable<int> productsIds)
        {
            //Set all products
            var products = new List<Product>();
            foreach (int productId in productsIds)
            {
                products.Add(_productRepo.GetProductById(productId));
            }

            package.Products = products;

            //Set eighteen plus on package based on the products
            bool eighteenPlus = false;
            var eighteenPlusPackages = products.Where(p => p.ContainsAlcohol == true);
            if (eighteenPlusPackages.Any())
            {
                eighteenPlus = true;
            }

            package.EighteenPlus = eighteenPlus;

            string deleteResult = await DeletePackageById(packageId);
            if (deleteResult != "success")
            {
                return deleteResult;
            }

            if (await _packageRepo.AddPackage(package))
            {
                return "success";
            }

            return "Something went wrong";
        }

        public async Task<bool> AddPackage(int packageId, Package package, IEnumerable<int> productsIds)
        {
            //Set all products
            var products = new List<Product>();
            foreach (int productId in productsIds)
            {
                products.Add(_productRepo.GetProductById(productId));
            }

            package.Products = products;

            //Set eighteen plus on package based on the products
            bool eighteenPlus = false;
            var eighteenPlusPackages = products.Where(p => p.ContainsAlcohol == true);
            if (eighteenPlusPackages.Any())
            {
                eighteenPlus = true;
            }

            package.EighteenPlus = eighteenPlus;

            if (await _packageRepo.AddPackage(package))
            {
                return true;
            }

            return false;
        }

        public async Task<string> DeletePackageById(int packageId)
        {
            var package = _packageRepo.GetPackageById(packageId);

            if (package != null)
            {
                if (package.ReservedByStudentId == null)
                {
                    if (await _packageRepo.DeletePackageById(packageId))
                    {
                        return "success";
                    }
                }

                return "already-reserved";
            }

            return "not-found";
        }
    }
}
