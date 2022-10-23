using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;

namespace Infrastructure.EF
{
    public class ProductRepo : IProductRepo
    {
        private readonly FoodWiseDbContext _context;

        public ProductRepo(FoodWiseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Product> GetAllProducts()
        {
            return _context.Products;
        }

        public Product GetProductById(int id)
        {
            return _context.Products.SingleOrDefault(product => product.Id == id)!;
        }
    }
}
