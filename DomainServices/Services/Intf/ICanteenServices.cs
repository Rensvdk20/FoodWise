using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Services.Intf
{
    public interface ICanteenServices
    {
        IEnumerable<Package> FilterCanteenPackages(int searchCity, int searchLocation);
        Task<string> EditPackageWithProducts(int packageId, Package package, IEnumerable<int> productsIds);
        Task<bool> AddPackage(int packageId, Package package, IEnumerable<int> productsIds);
        Task<string> DeletePackageById(int packageId);
    }
}
