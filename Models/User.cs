using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangAdmin.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public string Role { get; set; } = string.Empty;

        public string FullName { get; set; } = string.Empty;
    }
}
