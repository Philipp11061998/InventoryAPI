using System.ComponentModel.DataAnnotations;

namespace InventoryAPI.DTOs
{
    public class Login
    {
        [Required, MaxLength(50)]
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
    }
}
