using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace QuanLyNhaHangAdmin.Models
{
    [Table("DatMon")]
    public class DatMon
    {
        [Key]
        [MaxLength(6)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string MaDatMon { get; set; } = null!;


        [Required(ErrorMessage = "Vui lòng chọn đặt bàn")]
        [MaxLength(5)]
        [ForeignKey("DatBan")]
        public string MaDatBan { get; set; } = null!;
        public DatBan DatBan { get; set; } = null!;


        [Required(ErrorMessage = "Vui lòng chọn món ăn")]
        [MaxLength(5)]
        [ForeignKey("MonAn")]
        public string MaMon { get; set; } = null!;
        public MonAn MonAn { get; set; } = null!;


        [Range(1, int.MaxValue, ErrorMessage = "Số lượng món phải lớn hơn hoặc bằng 1")]
        public int SoLuong { get; set; } = 1;
        public string GhiChu { get; set; } = null!;
    }
}

