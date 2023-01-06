using System.Data;
using System.Diagnostics;
using Domain;
using DomainServices.Repos;
using DomainServices.Services.Intf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Controllers;

public class PackageController : Controller
{
    private readonly IPackageServices _packageServices;

    private readonly IPackageRepo _packageRepo;
    private readonly IStudentRepo _studentRepo;

    private readonly UserManager<IdentityUser> _userManager;

    public PackageController(
        IPackageServices packageServices,
        UserManager<IdentityUser> userManager,
        IPackageRepo packageRepo,
        IStudentRepo studentRepo)
    {
        _packageServices = packageServices;
        _userManager = userManager;
        _packageRepo = packageRepo;
        _studentRepo = studentRepo;
    }

    public IActionResult Index()
    {
        IEnumerable<Package> packages = _packageRepo.GetAllUnreservedPackages().OrderBy(a => a.AvailableTill);
        return View("Packages", packages);
    }

    // -1 is the filter "all"
    public PartialViewResult FilterPackages(int searchLocation, int searchCategory)
    {
        IEnumerable<Package> packages = _packageServices.FilterPackages(searchLocation, searchCategory);
        return PartialView("_PackagesPartial", packages);
    }

    public ViewResult Package(int id, string successMessage = null, string errorMessage = null)
    {
        if (successMessage != null)
        {
            ViewBag.SuccessMessage = successMessage;
        }
        else if (errorMessage != null)
        {
            ViewBag.ErrorMessage = errorMessage;
        }

        Package package = _packageRepo.GetPackageByIdWithProducts(id);
        return View(package);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> MyPackages()
    {
        Student student = await getStudentInfo();
        IEnumerable<Package> packages = _packageRepo.GetPackagesFromLoggedInStudent(student.Email).ToList();
        return View(packages);
    }

    [Authorize(Roles = "Student")]
    public async Task<IActionResult> ReservePackage(int id)
    {
        Student student = await getStudentInfo();
        string successMessage = null;
        string errorMessage = null;

        switch (await _packageServices.ReservePackageById(student.Id, id))
        {
            case "success":
                successMessage = "Pakket is succesvol gereserveerd";
                break;
            case "user-not-found":
                errorMessage = "Deze student bestaat niet";
                break;
            case "package-not-found":
                errorMessage = "Dit pakket bestaat niet";
                break;
            case "error-reserved":
                errorMessage = "Dit pakket is al gereserveerd";
                break;
            case "not-18":
                errorMessage = "Je moet 18+ zijn om dit 18+ pakket te bestellen";
                break;
            case "already-reservation":
                errorMessage = "Je hebt al een pakket gereserveerd op deze dag";
                break;
            default:
                errorMessage = "Er is iets fout gegaan probeer het later opnieuw";
                break;
        }

        return RedirectToAction("Package", "Package", new { id, successMessage, errorMessage });
    }

    [Authorize(Roles = "Student")]
    public async Task<Student> getStudentInfo()
    {
        var userid = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userid);
        return _studentRepo.GetStudentByEmail(user.Email);
    }
}