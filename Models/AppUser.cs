using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace LOGWITHGOOFLE.Models
{
    public class AppUser : IdentityUser
    {

        [Column(TypeName = "nvarchar(100)")]
        public required string FullName { get; set; }
        public required string ProfileImge { get; set; }
    }
}
