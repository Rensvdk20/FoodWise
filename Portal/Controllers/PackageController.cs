using System.Diagnostics;
using Domain;
using DomainServices.Repos;
using Microsoft.AspNetCore.Mvc;

namespace Portal.Controllers;

public class PackageController : Controller
{
    private readonly ILogger<PackageController> _logger;
    private readonly IPackageRepo _packageRepo;

    public PackageController(ILogger<PackageController> logger, IPackageRepo packageRepo)
    {
        _logger = logger;
        _packageRepo = packageRepo;
    }

    public IActionResult Index()
    {
        IEnumerable<Package> packages = _packageRepo.GetAllPackages().ToList();
        return View("Packages", packages);
    }

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
        
        return PartialView("_PackagesPartial", packages);
        
    }
}