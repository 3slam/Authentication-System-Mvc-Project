using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
    public class UpdateUserInfoViewModel
    {
        [Display(Name = "User Name")]
        
       
        public string UserName { get; set; }


         
        [Display(Name = "Full Name")]
        public string FullName { get; set; }

         [Display(Name = "Profile Imge")]
        public IFormFile ProfileImge { get; set; }
    }
}
