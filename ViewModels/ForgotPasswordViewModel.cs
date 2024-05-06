using System.ComponentModel.DataAnnotations;

namespace LOGWITHGOOFLE.ViewModels
{
   
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
