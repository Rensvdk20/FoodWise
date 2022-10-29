using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EF
{
    public class StudentRepo : IStudentRepo
    {
        private readonly FoodWiseDbContext _context;

        public StudentRepo(FoodWiseDbContext context)
        {
            _context = context;
        }

        public Student GetStudentById(int id)
        {
            return _context.Students.SingleOrDefault(student => student.Id == id);
        }

        public Student GetStudentByEmail(string email)
        {
            return _context.Students.SingleOrDefault(s => s.Email == email);
        }
    }
}