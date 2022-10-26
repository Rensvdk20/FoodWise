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

        public IQueryable<CanteenEmployee> GetAllCanteenEmployees()
        {
            return _context.CanteenEmployees.Include(c => c.Canteen);
        }

        public CanteenEmployee GetCanteenEmployeeById(int id)
        {
            return _context.CanteenEmployees.SingleOrDefault(canteenEmployee => canteenEmployee.Id == id);
        }

        public CanteenEmployee getCanteenEmployeeByEmail(string email)
        {
            return _context.CanteenEmployees.Include(c => c.Canteen).SingleOrDefault(canteenEmployee => canteenEmployee.Email == email);
        }
    }
}
