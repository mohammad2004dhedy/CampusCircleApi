
using System.ComponentModel.DataAnnotations;
namespace CampusCircleApi.Dtos
{
    public class UserLoginDto
    {
        [Required]
        [EmailAddress]
        [RegularExpression(
        @"^(?![._%+-])[a-zA-Z0-9._%+-]{1,64}(?<![._%+-])@(?:student\.)?aaup\.edu$",
        ErrorMessage = "Email must be an AAUP email"
        )]      
        public string Email{set;get;}
        [Required, StringLength(64, MinimumLength = 6)]
        public string Password{set;get;}
    }
}