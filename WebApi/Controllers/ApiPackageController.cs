using Domain;
using DomainServices.Repos;
using DomainServices.Services.Intf;
using Microsoft.AspNetCore.Mvc;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("/Api/[controller]")]
    [ApiController]
    public class ApiPackageController : ControllerBase
    {
        private readonly IPackageServices _packageServices;

        private readonly IPackageRepo _packageRepo;
        private readonly IStudentRepo _studentRepo;

        public ApiPackageController(
            IPackageServices packageServices,
            IPackageRepo packageRepo,
            IStudentRepo studentRepo)
        {
            _packageServices = packageServices;
            _packageRepo = packageRepo;
            _studentRepo = studentRepo;
        }


        [HttpGet]
        public IEnumerable<Package> GetAllPackages()
        {
            return _packageRepo.GetAllPackages();
        }

        [HttpPost]
        public async Task<PackageViewModel> AddNewReservation(int studentId, int packageId)
        {
            PackageViewModel result = new PackageViewModel();

            switch (await _packageServices.ReservePackageById(studentId, packageId))
            {
                case "success":
                    result.Code = 200;
                    result.Message = "Pakket is succesvol gereserveerd";
                    result.Package = _packageRepo.GetPackageByIdWithProducts(packageId);
                    return result;
                case "user-not-found":
                    result.Code = 404;
                    result.Message = "Deze student bestaat niet";
                    result.Package = null;
                    return result;
                case "package-not-found":
                    result.Code = 404;
                    result.Message = "Dit pakket bestaat niet";
                    result.Package = null;
                    return result;
                case "error-reserved":
                    result.Code = 400;
                    result.Message = "Dit pakket is al gereserveerd";
                    result.Package = null;
                    return result;
                case "not-18":
                    result.Code = 400;
                    result.Message = "Je moet 18+ zijn om dit 18+ pakket te bestellen";
                    result.Package = null;
                    return result;
                case "already-reservation":
                    result.Code = 400;
                    result.Message = "Je hebt al een pakket gereserveerd op deze dag";
                    result.Package = null;
                    return result;
                default:
                    result.Code = 400;
                    result.Message = "Er is iets fout gegaan probeer het later opnieuw";
                    result.Package = null;
                    return result;
            }
        }
    }
}