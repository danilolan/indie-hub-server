using System.ComponentModel.DataAnnotations;

namespace indie_hub_server.Dtos.User
{
    public class UserResponseDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
    }

    public class UpdateUserDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }
    }
}
