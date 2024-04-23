using System.ComponentModel.DataAnnotations;
using indie_hub_server.Models;

namespace indie_hub_server.Dtos.Playtest
{
    public class PlaytestCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? GameFileUrl { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class PlaytestUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [Required]
        public string? GameFileUrl { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }
    }

    public class PlaytestResponseDTO
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GameFileUrl { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public PlaytestUserDTO Author { get; set; }
    }

    public class PlaytestUserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
    }

}
