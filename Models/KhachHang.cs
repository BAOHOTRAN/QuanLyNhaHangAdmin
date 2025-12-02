using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
        [Table("KhachHang")]
        public class KhachHang
        {
            [Key, MaxLength(5)]
            public string MaKH { get; set; }

            [Required, MaxLength(50)]
            public string HoTen { get; set; }

            [MaxLength(10)]
            public string? GioiTinh { get; set; }

            public DateTime? NgaySinh { get; set; }

            [MaxLength(50)]
            public string? Email { get; set; }

            [Required, MaxLength(15)]
            public string SoDienThoai { get; set; }

            [MaxLength(200)]
            public string? DiaChi { get; set; }

            [MaxLength(30)]
            public string? TenDangNhap { get; set; }

            [MaxLength(30)]
            public string? MatKhau { get; set; }

            public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
            public virtual ICollection<HoaDon> HoaDons { get; set; } = new List<HoaDon>();
        }
}
