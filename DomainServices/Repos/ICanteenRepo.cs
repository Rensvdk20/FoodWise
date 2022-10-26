﻿using Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainServices.Repos
{
    public interface ICanteenRepo
    {
        IQueryable<Canteen> GetAllCanteens();
        Canteen GetCanteenById(int id);
    }
}