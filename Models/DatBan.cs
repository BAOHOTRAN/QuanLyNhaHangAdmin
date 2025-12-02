using QuanLyNhaHangAdmin.Models;
using QuanLyNhaHangAdmin.Validator;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static QuanTriNhaHangAdmin;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("DatBan")]
    public class DatBan
    {
        [Key, MaxLength(10)]
        public string MaDatBan { get; set; } = null!;

        [Required, MaxLength(5)]
        public string MaKH { get; set; } = null!;

        [MaxLength(5)]
        public string? MaNV { get; set; }

        [Required, MaxLength(5)]
        public string MaBan { get; set; } = null!;


        [Required(ErrorMessage = "Vui lòng chọn ngày đặt")]
        [DataType(DataType.Date)]
        [Display(Name = "Ngày đặt")]
        [FutureDate(ErrorMessage = "Ngày đặt không được là ngày trong quá khứ")]
        public DateTime NgayDat { get; set; }


        [Required(ErrorMessage = "Vui lòng chọn giờ đặt")]
        [DataType(DataType.Time)]
        [Display(Name = "Giờ đặt")]
        [ValidTimeAttribute("07:00", "22:00", ErrorMessage = "Giờ đặt phải từ 07:00 đến 22:00")]
        public TimeSpan GioDat { get; set; }


        [Range(1, int.MaxValue, ErrorMessage = "Số người phải lớn hơn hoặc bằng 1")]
        public int SoNguoi { get; set; }

        [MaxLength(30)]
        public string? TrangThai { get; set; } = "Chờ xác nhận";

        [MaxLength(300)]
        public string? GhiChu { get; set; }

        [ForeignKey("MaBan")]
        public virtual BanAn BanAn { get; set; } = null!;

        [ForeignKey("MaKH")]
        public virtual KhachHang KhachHang { get; set; } = null!;
        public NhanVien NhanVien { get; set; } = null!;
        public virtual ICollection<DatMon> DatMons { get; set; } = new List<DatMon>();
    }
}

