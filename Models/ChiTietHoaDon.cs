using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("ChiTietHoaDon")]
    public class ChiTietHoaDon
    {
        [Key, Column(Order = 0), MaxLength(10)]
        public string MaHD { get; set; } = string.Empty;

        [Key, Column(Order = 1), MaxLength(5)]
        public string MaMon { get; set; } = string.Empty;

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal SoLuong { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal DonGia { get; set; }

        [ForeignKey("MaHD")]
        public virtual HoaDon? HoaDon { get; set; }

        [ForeignKey("MaMon")]
        public virtual MonAn? MonAn { get; set; }
    }
}
