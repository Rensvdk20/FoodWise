using System.ComponentModel.DataAnnotations;

namespace Portal.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Gebruikersnaam is verplicht")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        public string Password { get; set; }
        public string ReturnUrl { get; set; } = "/";

    }
}
