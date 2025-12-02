using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangAdmin.Models
{
    public class User
    {
        [Key]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string Role { get; set; } 

        public string FullName { get; set; }
    }
}
