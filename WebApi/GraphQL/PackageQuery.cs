using Domain;
using DomainServices.Repos;
using Infrastructure.EF;
using Microsoft.EntityFrameworkCore;

namespace WebApi.GraphQL
{
    public class PackageQuery
    {
        [UseProjection]
        public IQueryable<Package> GetAllPackages(FoodWiseDbContext context)
        {
            return context.Packages.Include(p => p.Products).Include(c => c.Canteen);
        }
    }
}
