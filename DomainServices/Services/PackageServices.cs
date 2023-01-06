using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;
using DomainServices.Services.Intf;

namespace DomainServices.Services
{
    public class PackageServices : IPackageServices
    {
        private readonly IPackageRepo _packageRepo;
        private readonly IStudentRepo _studentRepo;

        public PackageServices(IPackageRepo packageRepo, IStudentRepo studentRepo)
        {
            _packageRepo = packageRepo;
            _studentRepo = studentRepo;
        }

        //The value -1 searches for all
        public IEnumerable<Package> FilterPackages(int searchLocation = -1, int searchCategory = -1)
        {
            IQueryable<Package> packages = _packageRepo.GetAllUnreservedPackages();
            if (searchLocation == -1 && searchCategory != -1)
            {
                return packages.Where(p => p.Category == searchCategory).ToList();
            } else if (searchLocation != -1 && searchCategory == -1)
            {
                return packages.Where(p => p.Canteen.Location == searchLocation).ToList();
            } else if (searchLocation != -1 && searchCategory != -1)
            {
                return packages.Where(p => p.Category == searchCategory)
                    .Where(p => p.Canteen.Location == searchLocation).ToList();
            }

            return packages.ToList();
        }

        public async Task<string> ReservePackageById(int studentId, int packageId)
        {
            Student student = _studentRepo.GetStudentById(studentId);
            if (student == null)
            {
                return "user-not-found";
            }

            Package package = _packageRepo.GetPackageById(packageId);
            if (package == null)
            {
                return "package-not-found";
            }

            int compareFor18Plus = DateTime.Compare(student.Birthday.AddYears(18), package.PickupTime);
            if (package.ReservedByStudentId == null)
            {
                if (package.EighteenPlus && compareFor18Plus > 0)
                {
                    return "not-18";
                }

                if (!_packageRepo.GetPackagesFromLoggedInStudent(student.Email).Any(s => s.PickupTime.Date == package.PickupTime.Date))
                {
                    package.ReservedByStudentId = student.Id;
                    await _packageRepo.UpdatePackage(package);
                    return "success";
                }

                return "already-reservation";
            }

            return "error-reserved";
        }
    }
}
