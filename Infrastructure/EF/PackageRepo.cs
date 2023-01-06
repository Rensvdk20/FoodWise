using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Infrastructure.EF
{
    public class PackageRepo : IPackageRepo
    {
        private readonly FoodWiseDbContext _context;

        public PackageRepo(FoodWiseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Package> GetAllPackages()
        {
            return _context.Packages.Include(c => c.Canteen).Include(p => p.Products);
        }

        public IQueryable<Package> GetAllPackagesBasic()
        {
            return _context.Packages;
        }

        public Package GetPackageById(int id)
        {
            return _context.Packages.SingleOrDefault(package => package.Id == id);
        }

        public Package GetPackageByIdWithProducts(int id)
        {
            return _context.Packages.Include(p => p.Products).SingleOrDefault(package => package.Id == id);
        }

        public IQueryable<Package> GetAllUnreservedPackages()
        {
            return _context.Packages.Where(p => p.ReservedByStudentId == null).Include(p => p.Products);
        }

        public IQueryable<Package> GetAllReservedPackages()
        {
            return _context.Packages.Where(p => p.ReservedByStudentId != null).Include(p => p.Products).Include(s => s.ReservedByStudent);
        }

        public IQueryable<Package> GetPackagesFromLoggedInStudent(string email)
        {
            return _context.Packages.Include(s => s.ReservedByStudent).Where(p => p.ReservedByStudent.Email == email);
        }

        public IQueryable<Package> GetAllCanteenPackages(CanteenEmployee canteenEmployee)
        {
            return _context.Packages.Include(c => c.Canteen)
                .Where(c => c.Canteen.Location == canteenEmployee.Canteen.Location)
                .Where(c => c.Canteen.City == canteenEmployee.Canteen.City).OrderBy(a => a.AvailableTill);
        }

        public async Task<bool> AddPackage(Package newPackage)
        {
            _context.Packages.Add(newPackage);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> DeletePackageById(int packageId)
        {
            Package package = GetPackageById(packageId);
            _context.Packages.Remove(package);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }

        public async Task<bool> UpdatePackage(Package package)
        {
            _context.Update(package);
            if (await _context.SaveChangesAsync() > 0)
            {
                return true;
            }

            return false;
        }
    }
}
