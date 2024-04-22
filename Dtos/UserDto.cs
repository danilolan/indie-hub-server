using System.ComponentModel.DataAnnotations;

namespace indie_hub_server.Dtos.User
{
    public class UserUpdateDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
    }
}
