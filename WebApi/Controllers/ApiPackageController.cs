using Domain;
using DomainServices.Repos;
using Microsoft.AspNetCore.Mvc;
using WebApi.Model;

namespace WebApi.Controllers
{
    [Route("/Api/[controller]")]
    [ApiController]
    public class ApiPackageController : ControllerBase
    {
        private readonly IPackageRepo _packageRepo;
        private readonly IStudentRepo _studentRepo;

        public ApiPackageController(
            IPackageRepo packageRepo,
            IStudentRepo studentRepo)
        {
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
            Student student = _studentRepo.GetStudentById(studentId);
            if (student == null)
            {
                result.Code = 400;
                result.Message = "User does not exist";
                result.Package = null;
                return result;
            }

            switch (await _packageRepo.ReservePackageById(student, packageId))
            {
                case "success":
                    result.Code = 404;
                    result.Message = "Pakket is succesvol gereserveerd";
                    result.Package = _packageRepo.GetPackageById(packageId);
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