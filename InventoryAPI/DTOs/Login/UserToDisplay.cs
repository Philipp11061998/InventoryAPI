using InventoryAPI.Models;

namespace InventoryAPI.DTOs
{
    public class UserToDisplay
    {
        public int Id {get;}
        public string Username {get;}
        public string Role {get;}
        public DateTime CreatedAt {get;}

        public UserToDisplay(User user)
        {
            Id = user.Id;
            Username = user.Username;
            Role = user.Role;
            CreatedAt = user.CreatedAt;
        }
    }
}