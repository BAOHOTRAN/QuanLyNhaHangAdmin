using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("MonAn")]
    public class MonAn
    {

        [Key, MaxLength(5)]
        public string MaMon { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập tên món ăn")]
        [MaxLength(50)]
        public string TenMon { get; set; } = null!;

        [Required(ErrorMessage = "Vui lòng nhập giá món ăn")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Gia { get; set; }

        [MaxLength(200)]
        public string? MoTa { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn loại món ăn")]
        [MaxLength(5)]
        public string MaLoai { get; set; } = null!;

        [ForeignKey("MaLoai")]
        public virtual LoaiMonAn LoaiMonAn { get; set; } = null!;

        public virtual ICollection<DatMon> DatMons { get; set; } = new List<DatMon>();
        public virtual ICollection<ChiTietHoaDon> ChiTietHoaDons { get; set; } = new List<ChiTietHoaDon>();

        [MaxLength(200)]
        public string? AnhMon { get; set; }
    }
}

