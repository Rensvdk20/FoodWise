using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain;

namespace DomainServices.Repos
{
    public interface IStudentRepo
    {
        IQueryable<Student> GetAllStudents();
        Student GetStudentById(int id);
        Student getStudentByEmail(string email);
    }
}
