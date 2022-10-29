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
    public class CanteenEmployeeRepo : ICanteenEmployeeRepo
    {
        private readonly FoodWiseDbContext _context;

        public CanteenEmployeeRepo(FoodWiseDbContext context)
        {
            _context = context;
        }

        public CanteenEmployee GetCanteenEmployeeByEmail(string email)
        {
            return _context.CanteenEmployees.Include(c => c.Canteen).SingleOrDefault(canteenEmployee => canteenEmployee.Email == email);
        }
    }
}
