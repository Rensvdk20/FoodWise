using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;
using DomainServices.Repos;

namespace Infrastructure.EF
{
    public class StudentEFRepo : IStudentRepo
    {
        private readonly FoodWiseDbContext _context;

        public StudentEFRepo(FoodWiseDbContext context)
        {
            _context = context;
        }

        public IQueryable<Student> GetAllStudents()
        {
            return _context.Students;
        }

        public Student GetStudentById(int id)
        {
            return _context.Students.SingleOrDefault(student => student.Id == id);
        }
    }
}