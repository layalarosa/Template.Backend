using System.ComponentModel.DataAnnotations;

namespace Template.Backend.Dtos
{
    public class UserCredentialsDto
    {
        [EmailAddress]
        public required string Email { get; set; }
        public required string Password { get; set; }
    }
}
