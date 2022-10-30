using System.Text.Json.Serialization;
using DomainServices.Repos;
using Infrastructure.EF;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WebApi.GraphQL;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddScoped<IStudentRepo, StudentRepo>();
builder.Services.AddScoped<IPackageRepo, PackageRepo>();
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<ICanteenRepo, CanteenRepo>();
builder.Services.AddScoped<ICanteenEmployeeRepo, CanteenEmployeeRepo>();

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

builder.Services.AddDbContext<FoodWiseDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration["ConnectionStrings:EFConnection"]);
});

builder.Services.AddGraphQLServer().RegisterDbContext<FoodWiseDbContext>().AddQueryType<PackageQuery>().AddProjections();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapGraphQL();

app.Run();
