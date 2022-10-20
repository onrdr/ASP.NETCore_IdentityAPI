using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email Adress is required")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string EmailAddress { get; set; }
    }
}
