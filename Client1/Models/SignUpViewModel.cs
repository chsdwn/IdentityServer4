using System.ComponentModel.DataAnnotations;

namespace Client1.Models
{
    public class SignUpViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        public string City { get; set; }
    }
}