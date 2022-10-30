using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public enum City
    {
        Breda,
        [Display(Name = "Den Bosch")]
        DenBosch,
        Tilburg
    }
}
