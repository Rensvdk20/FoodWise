﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace DomainServices.Repos
{
    public interface IPackageRepo
    {
        IQueryable<Package> GetAllPackages();
        Package GetPackageById(int id);
        IQueryable<Package> GetAllUnreservedPackages();
        IQueryable<Package> GetAllPackagesBasic();
        IQueryable<Package> GetAllReservedPackages();
        Task<bool> ReservePackageById(int studentId, int packageId);
        IQueryable<Package> GetPackagesFromLoggedInStudent(string email);

        Task AddPackage(Package newPackage);
        Task EditPackage(Package editedPackage);
        Task DeletePackageById(int id);
    }
}