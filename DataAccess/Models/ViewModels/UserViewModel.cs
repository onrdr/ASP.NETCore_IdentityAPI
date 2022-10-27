using DataAccess.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Username is required")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required(ErrorMessage = "Phone Number is required")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }


        [Required(ErrorMessage = "Email Adress is required")]
        [Display(Name = "Email Address")]
        [EmailAddress(ErrorMessage = "Email Address is not valid")]
        public string Email { get; set; }

        public string? City { get; set; }
        public string? Picture { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birthday")]
        public DateTime? BirthDay { get ; set; }
        public Gender? Gender { get; set; }
    }
}
