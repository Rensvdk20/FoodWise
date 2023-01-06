using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace DomainServices.Services.Intf
{
    public interface IPackageServices
    {
        IEnumerable<Package> FilterPackages(int searchLocation, int searchCategory);
        Task<string> ReservePackageById(int studentId, int packageId);
    }
}
