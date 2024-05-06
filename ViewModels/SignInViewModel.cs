using LOGWITHGOOFLE.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
    public class SignInViewModel
    {
        [Display(Name = "User Name")]
        [Required]
        [Remote(action: "IsUserUnique", controller: "Validation")]
        public string UserName { get; set; }

        
        [Required]
        [Display(Name = "Full Name")]
        public string FullName { get; set; }
        

        [Display(Name = "Email")]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        //[Required]
        //[Display(Name = "Profile Image")]
        //public IFormFile ProfileImage { get; set; }


        [Display(Name = "RememberMe")]
        public bool RememberMe { get; set; }

     
    }

}
