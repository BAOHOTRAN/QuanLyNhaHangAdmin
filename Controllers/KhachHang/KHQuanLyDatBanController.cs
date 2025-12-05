using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;

namespace QuanLyNhaHangAdmin.Controllers
{
    public class KHQuanLyDatBanController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public KHQuanLyDatBanController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        private bool CheckQuyen()
        {
            return HttpContext.Session.GetString("Role") == "KhachHang";
        }
        public IActionResult DanhSachDatBan()
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            string? maKH = HttpContext.Session.GetString("Username");

            var list = _context.DatBans
                .Include(x => x.BanAn)
                .Where(x => x.MaKH == maKH)
                .OrderByDescending(x => x.NgayDat)
                .ToList();

            return View(list);
        }
        [HttpGet]
        public IActionResult DatBanThuCong()
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            ViewBag.DanhSachBan = _context.BanAns.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult DatBanThuCong(DatBan model)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            string? maKH = HttpContext.Session.GetString("Username");

            ModelState.Remove("BanAn");
            ModelState.Remove("KhachHang");
            ModelState.Remove("NhanVien");
            ModelState.Remove("DatMons");
            ModelState.Remove("MaDatBan");

            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachBan = _context.BanAns.ToList();
                return View(model);
            }

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }
            model.MaKH = maKH;

            var last = _context.DatBans.OrderByDescending(x => x.MaDatBan).FirstOrDefault();
            int next = 1;
            if (last != null)
                next = int.Parse(last.MaDatBan.Replace("DB", "")) + 1;

            model.MaDatBan = "DB" + next.ToString("D4");

            bool trung = _context.DatBans.Any(x =>
                x.MaBan == model.MaBan &&
                x.NgayDat == model.NgayDat &&
                x.GioDat == model.GioDat
            );

            if (trung)
            {
                TempData["Error"] = "Bàn này đã có người đặt trong khung giờ đó!";
                ViewBag.DanhSachBan = _context.BanAns.ToList();
                return View(model);
            }

            _context.DatBans.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đặt bàn thành công!";
            return RedirectToAction("DanhSachDatBan");
        }

        [HttpGet]
        public IActionResult DatBanQR(string maBan)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            ViewBag.MaBan = maBan;

            return View();
        }

        [HttpPost]
        public IActionResult DatBanQR(DatBan model)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            string? maKH = HttpContext.Session.GetString("Username");

            ModelState.Remove("BanAn");
            ModelState.Remove("KhachHang");
            ModelState.Remove("NhanVien");
            ModelState.Remove("DatMons");
            ModelState.Remove("MaDatBan");

            if (!ModelState.IsValid)
            {
                ViewBag.MaBan = model.MaBan;
                return View(model);
            }
            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }
            model.MaKH = maKH;

            // Auto sinh mã
            var last = _context.DatBans.OrderByDescending(x => x.MaDatBan).FirstOrDefault();
            int next = last == null ? 1 : int.Parse(last.MaDatBan.Replace("DB", "")) + 1;
            model.MaDatBan = "DB" + next.ToString("D4");

            // Check trùng giờ
            bool trung = _context.DatBans.Any(x =>
                x.MaBan == model.MaBan &&
                x.NgayDat == model.NgayDat &&
                x.GioDat == model.GioDat
            );

            if (trung)
            {
                TempData["Error"] = "Khung giờ này đã có người đặt!";
                ViewBag.MaBan = model.MaBan;
                return View(model);
            }

            _context.DatBans.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đặt bàn qua QR thành công!";
            return RedirectToAction("DanhSachDatBan");
        }
    }
}
