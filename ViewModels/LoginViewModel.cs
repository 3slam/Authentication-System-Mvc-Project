﻿using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "User Name")]
      
        public string UserName { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
