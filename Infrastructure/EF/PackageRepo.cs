using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;
using Microsoft.EntityFrameworkCore;

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

        public Package GetPackageById(int id)
        {
            return _context.Packages.Include(p => p.Products).SingleOrDefault(package => package.Id == id)!;
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

        public async Task DeletePackageById(int id)
        {
            var result = _context.Packages.SingleOrDefault(package => package.Id == id);
            if (result != null)
            {
                _context.Packages.Remove(result);
                await _context.SaveChangesAsync();
            }
        }
    }
}
