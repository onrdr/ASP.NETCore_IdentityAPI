
using System.ComponentModel.DataAnnotations;

namespace DataAccess.Models.ViewModels
{
    public class RoleViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage ="Role name is required")]
        [Display(Name="Role Name")]
        public string Name { get; set; }
    }
}
