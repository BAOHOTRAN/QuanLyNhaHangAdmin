using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;

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

        public IActionResult DanhSachMonAn(string tenMon = "", decimal? giaTu = null, decimal? giaDen = null, string maLoai = "")
        {
            // Load danh sách loại món
            ViewBag.LoaiMonList = _context.LoaiMonAns.ToList();

            // Query chính
            var ds = _context.MonAns
                             .Include(m => m.LoaiMonAn)
                             .AsQueryable();

            // Lọc giá
            if (giaTu.HasValue)
                ds = ds.Where(m => m.Gia >= giaTu.Value);

            if (giaDen.HasValue)
                ds = ds.Where(m => m.Gia <= giaDen.Value);

            // Lọc theo loại
            if (!string.IsNullOrWhiteSpace(maLoai))
                ds = ds.Where(m => m.MaLoai == maLoai);

            // Tìm kiếm KHÔNG dấu + không phân biệt hoa thường
            if (!string.IsNullOrWhiteSpace(tenMon))
            {
                string key = tenMon.Trim().ToLower();

                ds = ds.Where(m =>
                    EF.Functions.Collate(m.TenMon, "SQL_Latin1_General_CP1253_CI_AI")
                    .ToLower()
                    .Contains(key)
                );
            }

            // Gửi lại giá trị lên View (giữ trạng thái)
            ViewBag.TenMon = tenMon;
            ViewBag.GiaTu = giaTu;
            ViewBag.GiaDen = giaDen;
            ViewBag.MaLoai = maLoai;

            return View(ds.OrderBy(m => m.TenMon).ToList());
        }
        [HttpGet]
        public IActionResult GoiY(string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return Json(new List<string>());

            string key = term.Trim().ToLower();

            var result = _context.MonAns
                .Where(m =>
                    EF.Functions.Collate(m.TenMon, "SQL_Latin1_General_CP1253_CI_AI")
                    .ToLower()
                    .Contains(key)
                )
                .Select(m => m.TenMon)
                .Distinct()
                .Take(8)
                .ToList();

            return Json(result);
        }
    }
}

