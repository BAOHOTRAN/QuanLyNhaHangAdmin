using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHangAdmin.Controllers.KhachHang
{
    public class UserHomeController : Controller
    {
        public IActionResult TrangLoginKhachHang()
        {
            return View();
        }
    }
}
