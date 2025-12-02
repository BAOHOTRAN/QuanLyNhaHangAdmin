using System.ComponentModel.DataAnnotations;

namespace QuanLyNhaHangAdmin.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập họ tên")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tài khoản")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Chọn loại tài khoản")]
        [RegularExpression("NhanVien|KhachHang", ErrorMessage = "Chỉ được chọn NhanVien hoặc KhachHang")]
        public string Role { get; set; }
    }
}

