using System.ComponentModel.DataAnnotations;

namespace AppointAid.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "National Number is required.")]
        [Display(Name = "National Number")]
        [StringLength(10, ErrorMessage = "The National Number must be less than 10 characters.")]
        public string NationalNumber { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; }
    }
}