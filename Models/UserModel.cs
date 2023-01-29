using System.ComponentModel.DataAnnotations;

namespace Blockbuster_Rental_Software.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter a password")]
        [MinLength(8, ErrorMessage ="Password too short")]
        public string PasswordHash { get; set; }

        public Boolean RememberMe { get; set; }
        public Boolean isDeveloper { get; set; }

    }
}
