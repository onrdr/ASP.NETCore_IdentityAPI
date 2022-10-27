
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ViewModels
{
    public class PasswordChangeViewModel
    {
        [Display(Name ="Old Password")]
        [Required(ErrorMessage ="Old Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Password must be a minimum of 4 characters")]
        public string PasswordOld { get; set; }

        [Display(Name = "New Password")]
        [Required(ErrorMessage = "New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Password must be a minimum of 4 characters")]
        public string PasswordNew { get; set; }

        [Display(Name = "Confirm New Password")]
        [Required(ErrorMessage = "Confirm New Password is required")]
        [DataType(DataType.Password)]
        [MinLength(4, ErrorMessage = "Password must be a minimum of 4 characters")]
        [Compare("PasswordNew")]
        public string PasswordConfirm { get; set; }
    }
}
