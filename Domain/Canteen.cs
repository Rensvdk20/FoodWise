using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class Canteen
    {
        public int Id { get; set; }
        public List<Package> Packages { get; set; }
        [Required]
        public int Location { get; set; }
        [Required]
        public int City { get; set; }
    }
}