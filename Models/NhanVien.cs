using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("NhanVien")]
    public class NhanVien
    {
        [Key, MaxLength(5)]
        public string MaNV { get; set; } = string.Empty;

        [Required, MaxLength(50)]
        public string HoTen { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? GioiTinh { get; set; } 

        public DateTime? NgaySinh { get; set; } 

        [MaxLength(15)]
        public string? SDT { get; set; } 

        [MaxLength(200)]
        public string? DiaChi { get; set; } 

        [MaxLength(50)]
        public string? Email { get; set; } 

        [MaxLength(30)]
        public string? MatKhau { get; set; } 

        // Navigation property
        public virtual ICollection<DatBan> DatBans { get; set; } = new List<DatBan>();
    }
}
