using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
    public class EditUserViewModel
    {
        [Required]
        [Display(Name = "UserID")]
        public string UserID { get; set; }

        [Display(Name = "Username")]
        [Required]
       
        public string UserName { get; set; }

        [Display(Name = "Full tName")]
        [Required]
        public string FullName { get; set; }

   

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

       
    }
}
