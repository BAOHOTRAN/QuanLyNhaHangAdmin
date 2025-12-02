using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("BanAn")]
    public class BanAn
    {
        [Key, MaxLength(5)]
        public string MaBan { get; set; }

        [Required, MaxLength(50)]
        public string TenBan { get; set; }

        [Required]
        public int SoCho { get; set; }

        [Required, MaxLength(20)]
        public string TrangThai { get; set; } = "Trống";
        
        // Navigation
        public ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
    }
}
