using System.Diagnostics;
using Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Portal.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using DomainServices.Repos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Infrastructure;

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
        return View("MyReservations"); 
    }

    // ##### Reservation list #####
    [Authorize(Roles = "CanteenEmployee")]
    public IActionResult CanteenReservations()
    {
        return View(_packageRepo.GetAllReservedPackages().OrderBy(a => a.AvailableTill).ToList());
    }

    //##### All packages list #####
    [Authorize(Roles = "CanteenEmployee")]
    public async Task<IActionResult> CanteenPackages()
    {
        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployeeDefaultCity = canteenEmployee.Canteen.City;
        ViewBag.CanteenEmployeeDefaultLocation = canteenEmployee.Canteen.Location;

        return View(_packageRepo.GetAllPackagesBasic().Include(c => c.Canteen)
            .Where(c => c.Canteen.Location == canteenEmployee.Canteen.Location)
            .Where(c => c.Canteen.City == canteenEmployee.Canteen.City).OrderBy(a => a.AvailableTill).ToList());
    }

    [Authorize(Roles = "CanteenEmployee")]
    public async Task<PartialViewResult> FilterCanteenPackages(int searchCity = -1, int searchLocation = -1)
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
        if (packageVM.PickupTime > packageVM.AvailableTill)
        {
            ModelState.AddModelError("", "De datum/tijd van het veld '\''Beschikbaar tot'\'' moet na de datum/tijd van het '\'ophaalmoment'\' zijn");
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

            _packageRepo.AddPackage(package);
            ViewBag.Message = "Pakket is succesvol toegevoegd";
        }

        CanteenEmployee canteenEmployee = await getCanteenEmployeeInfo();
        ViewBag.CanteenEmployee = canteenEmployee;
        ViewBag.Products = _productRepo.GetAllProducts().ToList();
        ViewBag.Canteen = _canteenRepo.GetAllCanteens().ToList();
        return View();
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
            var user = await _userManager.FindByNameAsync(loginVM.Name);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                if ((await _signInManager.PasswordSignInAsync(user, loginVM.Password, false, false)).Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
        }

        if (loginVM.Name != null && loginVM.Password != null)
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

