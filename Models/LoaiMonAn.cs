using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("LoaiMonAn")]
    public class LoaiMonAn
    {
        [Key, MaxLength(5, ErrorMessage = "Mã loại không quá 5 ký tự")]
        [Required(ErrorMessage = "Vui lòng nhập Mã loại")]
        public string MaLoai { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập Tên loại")]   // bắt buộc → có hint + lỗi
        [MaxLength(50, ErrorMessage = "Tên loại không quá 50 ký tự")]
        public string TenLoai { get; set; } = null!;

        [MaxLength(200)]
        public string? MoTa { get; set; }

        public virtual ICollection<MonAn> MonAns { get; set; } = new List<MonAn>();
    }
}


