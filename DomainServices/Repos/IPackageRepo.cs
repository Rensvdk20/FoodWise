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

        Task AddPackage(Package newPackage);
        Task EditPackage(Package editedPackage);
        Task DeletePackageById(int id);
    }
}
