using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ViewModels
{
    public class PasswordResetViewModel
    {
        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Email required")]
        [EmailAddress]
        public string EmailAddress { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string PasswordNew { get; set; }
    }
}
