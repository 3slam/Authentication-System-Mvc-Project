using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
    public class CreateUserViewModel
    {

        [Required]
        [Display(Name = "Full name")]
        public string FullName { get; set; }


        [Required]
        [Display(Name = "User name")]
        [Remote(action: "IsUserUnique", controller: "Validation")]
        public string UserName { get; set; }

        [Required]
      


        [Display(Name = "Email")]
  
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [Display(Name = "Phone number")]
        [Phone]
        public string PhoneNumber { get; set; }


        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }



    }
}
