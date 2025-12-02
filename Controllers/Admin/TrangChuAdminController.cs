using Microsoft.AspNetCore.Mvc;

namespace QuanLyNhaHangAdmin.Controllers
{
    public class TrangChuAdminController : Controller
    {
        public IActionResult TrangGioiThieuAdmin()
        {
            ViewData["Title"] = "Trang quản trị";
            return View();
        }
    }
}
