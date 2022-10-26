using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;

namespace Infrastructure.EF
{
    public class CanteenRepo : ICanteenRepo
    {
        private readonly FoodWiseDbContext _context;

        public CanteenRepo(FoodWiseDbContext context)
        {
            _context = context;
        }
        public IQueryable<Canteen> GetAllCanteens()
        {
            return _context.Canteens;
        }

        public Canteen GetCanteenById(int id)
        {
            return _context.Canteens.SingleOrDefault(canteen => canteen.Id == id)!;
        }
    }
}
