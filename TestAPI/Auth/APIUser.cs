using System.ComponentModel.DataAnnotations;

namespace TestAPI.Auth
{
    public record APIUserDto(string username,string password);
    public class APIUser
    {
        [Required]
        public string UserName { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
