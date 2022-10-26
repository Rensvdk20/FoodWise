using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Repos
{
    public interface IProductRepo
    {
        IQueryable<Product> GetAllProducts();
        Product GetProductById(int id);
    }
}
