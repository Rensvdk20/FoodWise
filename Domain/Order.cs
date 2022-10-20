using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    class Order
    {
        public int Id { get; set; }
        [Required]
        public Student Student { get; set; }
        [Required]
        public Package Package { get; set; }
    }
}
