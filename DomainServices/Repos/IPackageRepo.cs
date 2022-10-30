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
        IQueryable<Package> GetAllUnreservedPackages();
        IQueryable<Package> GetAllPackagesBasic();
        IQueryable<Package> GetAllReservedPackages();
        Task<string> ReservePackageById(Student studentId, int packageId);
        IQueryable<Package> GetPackagesFromLoggedInStudent(string email);

        Task AddPackage(Package newPackage);
        Task<string> DeletePackageById(int id);
    }
}
