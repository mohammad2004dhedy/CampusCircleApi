using CampusCircleApi.Enums;
using System.ComponentModel.DataAnnotations;
namespace CampusCircleApi.Dtos
{
    public class CreateUserDto
    {
        [Required, StringLength(30, MinimumLength = 2)]
        public string FirstName{set;get;}

        [Required, StringLength(30, MinimumLength = 2)]
        public string LastName{set;get;}
        [Required]
        [EmailAddress] 
        [RegularExpression(
        @"^(?![._%+-])[a-zA-Z0-9._%+-]{1,64}(?<![._%+-])@(?:student\.)?aaup\.edu$",
        ErrorMessage = "Email must be an AAUP email (student.aaup.edu or aaup.edu)"
        )]
        public string Email{set;get;}

        [Required, StringLength(64, MinimumLength = 6)]
        public string Password{set;get;}

        [Required]
        public DateTime BirthDate{set;get;}

        [Required]
        public College College{set;get;}
    }
}