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
            return _context.Packages.Include(p => p.Products).SingleOrDefault(package => package.Id == id)!;
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

        public async Task<string> ReservePackageById(Student student, int packageId)
        {
            Package package = _context.Packages.SingleOrDefault(p => p.Id == packageId);
            int compareFor18Plus = DateTime.Compare(student.Birthday.AddYears(18), package.PickupTime);
            if (package.ReservedByStudentId == null)
            {
                if (package.EighteenPlus && compareFor18Plus > 0)
                {
                    return "not-18";
                }

                if (!GetPackagesFromLoggedInStudent(student.Email).Any(s => s.PickupTime.Date == package.PickupTime.Date))
                {
                    package.ReservedByStudentId = student.Id;
                    _context.Update(package);
                    await _context.SaveChangesAsync();
                    return "success";
                }

                return "already-reservation";
            }

            return "error-reserved";
        }

        public async Task AddPackage(Package newPackage)
        {
            _context.Packages.Add(newPackage);
            await _context.SaveChangesAsync();
        }

        public async Task EditPackage(Package editedPackage)
        {
            var result = _context.Packages.SingleOrDefault(package => package.Id == editedPackage.Id);
            if (result != null)
            {
                result = editedPackage;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<string> DeletePackageById(int id)
        {
            var package = _context.Packages.SingleOrDefault(package => package.Id == id);

            if (package != null)
            {
                if (package.ReservedByStudentId == null)
                {
                    _context.Packages.Remove(package);
                    await _context.SaveChangesAsync();
                    return "success";
                }

                return "already-reserved";
            }

            return "not-found";
        }
    }
}
