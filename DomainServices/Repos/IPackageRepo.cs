using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace DomainServices.Repos
{
    public interface IPackageRepo
    {
        IQueryable<Package> GetAllPackages();
        Package GetPackageById(int id);
        Package GetPackageByIdWithProducts(int id);
        IQueryable<Package> GetAllUnreservedPackages();
        IQueryable<Package> GetAllPackagesBasic();
        IQueryable<Package> GetAllReservedPackages();
        //Task<string> ReservePackageById(Student studentId, int packageId);
        IQueryable<Package> GetPackagesFromLoggedInStudent(string email);
        IQueryable<Package> GetAllCanteenPackages(CanteenEmployee canteenEmployee);

        Task<bool> AddPackage(Package newPackage);
        Task<bool> DeletePackageById(int id);
        Task<bool> UpdatePackage(Package package);
    }
}
