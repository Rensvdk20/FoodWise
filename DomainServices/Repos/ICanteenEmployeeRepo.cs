using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Repos
{
    public interface ICanteenEmployeeRepo
    {
        IQueryable<CanteenEmployee> GetAllCanteenEmployees();
        CanteenEmployee GetCanteenEmployeeById(int id);

        CanteenEmployee getCanteenEmployeeByEmail(string email);
    }
}
