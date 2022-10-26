using System.Data;
using System.Diagnostics;
using Domain;
using DomainServices.Repos;
using Infrastructure.EF;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Controllers;

public class PackageController : Controller
{
    private readonly IPackageRepo _packageRepo;
    private readonly IStudentRepo _studentRepo;

    private readonly UserManager<IdentityUser> _userManager;

    public PackageController(
        UserManager<IdentityUser> userManager,
        IPackageRepo packageRepo,
        IStudentRepo studentRepo)
    {
        _userManager = userManager;
        _packageRepo = packageRepo;
        _studentRepo = studentRepo;
    }

    public IActionResult Index()
    {
        IEnumerable<Package> packages = _packageRepo.GetAllPackages().ToList();
        return View("Packages", packages);
    }

    //Filter the packages
    // -1 is the filter "all"
    public PartialViewResult FilterPackages(int searchLocation = -1, int searchCategory = -1)
    {
        IQueryable<Package> packages = _packageRepo.GetAllPackages();
        if (searchLocation == -1 && searchCategory != -1)
        {
            return PartialView("_PackagesPartial", packages.Where(p => p.Category == searchCategory).ToList());
        } else if (searchLocation != -1 && searchCategory == -1)
        {
            return PartialView("_PackagesPartial", packages.Where(p => p.Canteen.Location == searchLocation).ToList());
        } else if (searchLocation != -1 && searchCategory != -1)
        {
            return PartialView("_PackagesPartial", packages.Where(p => p.Category == searchCategory).Where(p => p.Canteen.Location == searchLocation).ToList());
        }
        
        return PartialView("_PackagesPartial", packages.ToList());
    }

    public ViewResult Package(int id, string successMessage = null, string errorMessage = null)
    {
        if (successMessage != null)
        {
            ViewBag.SuccessMessage = successMessage;
        } else if (errorMessage != null)
        {
            ViewBag.ErrorMessage = errorMessage;
        }
        Package package = _packageRepo.GetPackageById(id);
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
        if (await _packageRepo.ReservePackageById(student.Id, id))
        {
            successMessage = "Pakket is succesvol gereserveerd";
        }
        else
        {
            errorMessage = "Dit pakket is al gereserveerd";
        }

        return RedirectToAction("Package", "Package", new { id, successMessage, errorMessage });
    }

    [Authorize(Roles = "Student")]
    public async Task<Student> getStudentInfo()
    {
        var userid = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userid);
        return _studentRepo.getStudentByEmail(user.Email);
    }
}