using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using System.Linq;

namespace QuanLyNhaHangAdmin.Controllers.KhachHang
{
    public class KHQuanLyMonAnController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public KHQuanLyMonAnController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        public IActionResult TrangGioiThieuUser()
        {
            ViewData["Title"] = "Trang Khách Hàng";
            return View();
        }

        public IActionResult DanhSachMonAn(string timKiem = "")
        {
            var ds = _context.MonAns
                             .Include(m => m.LoaiMonAn)
                             .AsQueryable();

            if (!string.IsNullOrEmpty(timKiem))
            {
                ds = ds.Where(m => m.TenMon.Contains(timKiem));
            }

            return View("DanhSachMonAn", ds.ToList());
        }

        public IActionResult TimKiemMonAn(string tenMon = "", decimal? giaTu = null, decimal? giaDen = null)
        {
            var ds = _context.MonAns
                             .Include(m => m.LoaiMonAn)
                             .AsQueryable();

            if (!string.IsNullOrEmpty(tenMon))
                ds = ds.Where(m => m.TenMon.Contains(tenMon));

            if (giaTu.HasValue)
                ds = ds.Where(m => m.Gia >= giaTu.Value);

            if (giaDen.HasValue)
                ds = ds.Where(m => m.Gia <= giaDen.Value);

            return View(ds.ToList());
        }

        public IActionResult ChiTietMonAn(string id)
        {
            var mon = _context.MonAns
                              .Include(m => m.LoaiMonAn)
                              .FirstOrDefault(m => m.MaMon == id);

            if (mon == null) return NotFound();

            return View("ChiTietMonAn", mon);
        }
    }
}
