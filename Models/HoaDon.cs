using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("HoaDon")]
    public class HoaDon
    {
        [Key, MaxLength(10)]
        public string MaHD { get; set; } = string.Empty;

        [MaxLength(5)]
        public string? MaKH { get; set; } = string.Empty;

        [Required]
        public DateTime NgayLap { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TongTien { get; set; }

        [MaxLength(30)]
        public string? TrangThai { get; set; } = "Chưa thanh toán";

        [MaxLength(50)]
        public string? PhuongThucThanhToan { get; set; }

        [ForeignKey("MaKH")]
        public virtual KhachHang? KhachHang { get; set; }

        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();
    }

}
