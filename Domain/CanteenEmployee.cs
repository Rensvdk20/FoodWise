using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class CanteenEmployee
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public int EmployeeNumber { get; set; }
        [Required]
        public int CanteenId { get; set; }
        public Canteen? Canteen { get; set; }
    }
}
