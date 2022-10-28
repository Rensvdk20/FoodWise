using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using Microsoft.AspNetCore.Authorization;
using DomainServices.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Portal.Controllers;

public class AccountController : Controller
{
    private readonly IPackageRepo _packageRepo;
    private readonly ICanteenEmployeeRepo _canteenEmployeeRepo;
    private readonly IProductRepo _productRepo;
    private readonly ICanteenRepo _canteenRepo;

    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        IPackageRepo packageRepo,
        ICanteenEmployeeRepo canteenEmployeeRepo,
        IProductRepo productRepo,
        ICanteenRepo canteenRepo)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _packageRepo = packageRepo;
        _canteenEmployeeRepo = canteenEmployeeRepo;
        _productRepo = productRepo;
        _canteenRepo = canteenRepo;
    }

    public IActionResult Index()
    {
        return View("Login"); 
    }

    public IActionResult AccessDenied()
    {
        return View();
    }

    // ##### Reservation list #####
    [Authorize(Roles = "CanteenEmployee")]
    public IActionResult CanteenReservations()
    {
        return View(_packageRepo.GetAllReservedPackages().OrderBy(a => a.AvailableTill).ToList());
    }

    //##### All packages list #####
    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenPackages(string successMessage = null, string errorMessage = null)
    {
        if (successMessage != null)
        {
            ViewBag.SuccessMessage = successMessage;
        }
        else if (errorMessage != null)
        {
            ViewBag.ErrorMessage = errorMessage;
        }

        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployeeDefaultCity = canteenEmployee.Canteen.City;
        ViewBag.CanteenEmployeeDefaultLocation = canteenEmployee.Canteen.Location;

        return View(_packageRepo.GetAllPackagesBasic().Include(c => c.Canteen)
            .Where(c => c.Canteen.Location == canteenEmployee.Canteen.Location)
            .Where(c => c.Canteen.City == canteenEmployee.Canteen.City).OrderBy(a => a.AvailableTill).ToList());
    }

    [Authorize(Roles = "CanteenEmployee")]
    public PartialViewResult FilterCanteenPackages(int searchCity = -1, int searchLocation = -1)
    {
        IQueryable<Package> packages = _packageRepo.GetAllPackagesBasic().Include(c => c.Canteen).OrderBy(a => a.AvailableTill);

        //The value -1 searches for all
        if (searchCity == -1 && searchLocation == -1)
        {
            return PartialView("_CanteenLocationPackagesPartial", packages.ToList());
        }
        else if (searchCity != -1 && searchLocation == -1)
        {
            return PartialView("_CanteenLocationPackagesPartial", packages.Where(l => l.Canteen.City == searchCity).ToList());
        }
        else if (searchCity == -1 && searchLocation != -1)
        {
            return PartialView("_CanteenLocationPackagesPartial", packages.Where(l => l.Canteen.Location == searchLocation).ToList());
        }

        return PartialView("_CanteenLocationPackagesPartial", packages.Where(l => l.Canteen.Location == searchLocation).Where(l => l.Canteen.City == searchCity).ToList());
    }

    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenEditPackage(int id)
    {
        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployee = canteenEmployee;
        ViewBag.Package = _packageRepo.GetPackageById(id);
        ViewBag.Products = _productRepo.GetAllProducts().ToList();
        ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenEditPackage(AddPackageViewModel packageVM)
    {
        int errorCount = 0;
        int comparePickupAndAvailableTill = DateTime.Compare(packageVM.PickupTime, packageVM.AvailableTill);
        if (comparePickupAndAvailableTill > 0)
        {
            ModelState.AddModelError("",
                "De datum/tijd van het veld '\''Beschikbaar tot'\'' moet na de datum/tijd van het '\'ophaalmoment'\' zijn");
            errorCount++;
        }

        DateTime now = DateTime.Now;
        if (packageVM.AvailableTill < now)
        {
            ModelState.AddModelError("", "Een pakket moet in de toekomst liggen");
            errorCount++;
        }

        if (now.AddDays(2) < packageVM.AvailableTill)
        {
            ModelState.AddModelError("", "Een pakket kan pas maximaal 2 dagen van te voren worden aangeboden");
            errorCount++;
        }

        if (_packageRepo.GetPackageById(packageVM.Id).ReservedByStudentId != null)
        {
            ModelState.AddModelError("", "Dit pakket is momenteel gereserveerd door een student");
            errorCount++;
        }

        if (ModelState.IsValid && errorCount == 0)
        {
            var products = new List<Product>();
            foreach (int productId in packageVM.SelectedProducts)
            {
                products.Add(_productRepo.GetProductById(productId));
            }

            bool eighteenPlus = false;
            var eighteenPlusPackages = products.Where(p => p.containsAlcohol == true);
            if (eighteenPlusPackages.Count() > 0)
            {
                eighteenPlus = true;
            }

            Package package = new Package
            {
                Name = packageVM.Name,
                Description = packageVM.Description,
                Products = products,
                CanteenId = packageVM.CanteenId,
                PickupTime = packageVM.PickupTime,
                AvailableTill = packageVM.AvailableTill,
                EighteenPlus = eighteenPlus,
                Price = packageVM.Price,
                Category = packageVM.Category
            };

            ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();

            await _packageRepo.DeletePackageById(packageVM.Id);
            await _packageRepo.AddPackage(package);
        }
        else
        {
            CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
            ViewBag.CanteenEmployee = canteenEmployee;
            ViewBag.Package = _packageRepo.GetPackageById(packageVM.Id);
            ViewBag.Products = _productRepo.GetAllProducts().ToList();
            ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();
            return View();
        }

        string successMessage = "Pakket is succesvol aangepast";
        return RedirectToAction("CanteenPackages", "Account", new { successMessage });
    }

    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenAddPackage()
    {
        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployee = canteenEmployee;
        ViewBag.Products = _productRepo.GetAllProducts().ToList();
        ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenAddPackage(AddPackageViewModel packageVM)
    {
        int errorCount = 0;
        int comparePickupAndAvailableTill = DateTime.Compare(packageVM.PickupTime, packageVM.AvailableTill);
        if (comparePickupAndAvailableTill > 0)
        {
            ModelState.AddModelError("", "De datum/tijd van het veld '\''Beschikbaar tot'\'' moet na de datum/tijd van het '\'ophaalmoment'\' zijn");
            errorCount++;
        }

        DateTime now = DateTime.Now;
        if (packageVM.AvailableTill < now)
        {
            ModelState.AddModelError("", "Een pakket moet in de toekomst liggen");
            errorCount++;
        }

        if (now.AddDays(2) < packageVM.AvailableTill)
        {
            ModelState.AddModelError("", "Een pakket kan pas maximaal 2 dagen van te voren worden aangeboden");
            errorCount++;
        }

        if (ModelState.IsValid && errorCount == 0)
        {
            var products = new List<Product>();
            foreach (int productId in packageVM.SelectedProducts)
            {
                products.Add(_productRepo.GetProductById(productId));
            }

            bool eighteenPlus = false;
            var eighteenPlusPackages = products.Where(p => p.containsAlcohol == true);
            if (eighteenPlusPackages.Count() > 0)
            {
                eighteenPlus = true;
            }

            Package package = new Package
            {
                Name = packageVM.Name,
                Description = packageVM.Description,
                Products = products,
                CanteenId = packageVM.CanteenId,
                PickupTime = packageVM.PickupTime,
                AvailableTill = packageVM.AvailableTill,
                EighteenPlus = eighteenPlus,
                Price = packageVM.Price,
                Category = packageVM.Category
            };

            ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();

            await _packageRepo.AddPackage(package);
            ViewBag.Message = "Pakket is succesvol toegevoegd";
        }

        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployee = canteenEmployee;
        ViewBag.Products = _productRepo.GetAllProducts().ToList();
        ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();
        return View();
    }

    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> DeletePackage(int id)
    {
        string successMessage = null;
        string errorMessage = null;
        switch (await _packageRepo.DeletePackageById(id))
        {
            case "success":
                successMessage = "Het pakket is succesvol verwijderd";
                break;
            case "already-reserved":
                errorMessage = "Dit pakket is momenteel gereserveerd";
                break;
            case "not-found":
                errorMessage = "Het pakket is niet gevonden";
                break;
            default:
                errorMessage = "Er is iets fout gegaan probeer het later opnieuw";
                break;

        }
        return RedirectToAction("CanteenPackages", "Account", new { successMessage, errorMessage  });
    }

    // ##### Login ######
    public IActionResult Login()
    {
        return View("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginVM)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(loginVM.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                if ((await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false)).Succeeded)
                {
                    if (await _userManager.IsInRoleAsync(user, "CanteenEmployee"))
                    {
                        return RedirectToAction("CanteenReservations", "Account");
                    }

                    return RedirectToAction("Index", "Home");
                }
            }
        }

        if (loginVM.Email != null && loginVM.Password != null)
        {
            ModelState.AddModelError("", "Gebruikersnaam of wachtwoord is niet correct");
        }

        return View("Login");
    }

    public async Task<RedirectResult> Logout(string returnUrl = "/")
    {
        await _signInManager.SignOutAsync();
        return Redirect(returnUrl);
    }

    [Authorize(Roles = "CanteenEmployee")]
    public async Task<CanteenEmployee> getCanteenEmployeeInfo()
    {
        var userid = _userManager.GetUserId(HttpContext.User);
        var user = await _userManager.FindByIdAsync(userid);
        return _canteenEmployeeRepo.getCanteenEmployeeByEmail(user.Email);
    }
}

