using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.Models
{
    public class CreateRoleViewModel
    {
        [Required]
        [Display(Name = "Role Name")]
        public string RoleName { get; set; }
    }
}
