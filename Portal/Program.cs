using DomainServices.Repos;
using Infrastructure.EF;
using Infrastructure.IF;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddScoped<IStudentRepo, StudentEFRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICanteenRepo, CanteenRepo>();
builder.Services.AddScoped<ICanteenEmployeeRepo, CanteenEmployeeRepo>();

builder.Services.AddDbContext<FoodWiseDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:EFConnection"]);
});

builder.Services.AddDbContext<FoodWiseIdentityDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:IFConnection"]);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

//app.MapControllerRoute(
//    name: "Responses",
//    pattern: "/Package}/{id?}",
//    defaults: new { controller = "Package", action = "Package" });

app.MapControllerRoute(
    name: "Package",
    pattern: "Package{id}",
    defaults: new { Controller = "Package", action = "Package" }
);

app.Run();
