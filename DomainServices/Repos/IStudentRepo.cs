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
        Student GetStudentById(int id);
        Student GetStudentByEmail(string email);
    }
}
