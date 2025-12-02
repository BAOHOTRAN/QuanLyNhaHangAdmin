using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;

namespace QuanLyNhaHangAdmin.Controllers
{
    public class KHQuanLyDatMonController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public KHQuanLyDatMonController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        private bool CheckQuyen()
        {
            return HttpContext.Session.GetString("Role") == "KhachHang";
        }

        public IActionResult DanhSachDatMon()
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            var maKH = HttpContext.Session.GetString("Username");

            var list = _context.DatMons
                .Include(d => d.MonAn)
                .Include(d => d.DatBan)  
                .Where(d => d.DatBan.MaKH == maKH)
                .ToList();

            return View(list);
        }

        [HttpGet]
        public IActionResult ThemDatMon(string maDatBan)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            ViewBag.DanhSachMon = _context.MonAns.ToList();
            ViewBag.DanhSachDatBan = _context.DatBans.ToList();
            ViewBag.MaDatBan = maDatBan;
            return View();
        }


        [HttpPost]
        public IActionResult ThemDatMon(DatMon model)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");
            
            ModelState.Remove("DatBan");
            ModelState.Remove("MonAn");
            ModelState.Remove("MaDatMon");
            
            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachMon = _context.MonAns.ToList();
                ViewBag.DanhSachDatBan = _context.DatBans.ToList();
                return View(model);
            }

            if (_context.DatBans.Find(model.MaDatBan) == null)
            {
                TempData["Error"] = "Đặt bàn không tồn tại!";
                ViewBag.DanhSachMon = _context.MonAns.ToList();
                return View(model);
            }

            if (_context.MonAns.Find(model.MaMon) == null)
            {
                TempData["Error"] = "Món ăn không tồn tại!";
                ViewBag.DanhSachMon = _context.MonAns.ToList();
                return View(model);
            }

            var last = _context.DatMons
                .OrderByDescending(x => x.MaDatMon)
                .FirstOrDefault();

            int next = 1;
            if (last != null)
                next = int.Parse(last.MaDatMon.Replace("DM", "")) + 1;

            model.MaDatMon = "DM" + next.ToString("D4");

            _context.DatMons.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Đặt món thành công!";
            return RedirectToAction("DanhSachDatMon");
        }

        [HttpGet]
        public IActionResult SuaDatMon(string id)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");


            var datMon = _context.DatMons.FirstOrDefault(x => x.MaDatMon == id);
            if (datMon == null) return NotFound();

            ViewBag.DanhSachMon = new SelectList(
                _context.MonAns,
                "MaMon",
                "TenMon",
                datMon.MaMon
            );

            return View(datMon);
        }

        [HttpPost]
        public IActionResult SuaDatMon(DatMon model)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");
            
            ModelState.Remove("DatBan");
            ModelState.Remove("MonAn");
            ModelState.Remove("MaDatMon");

            if (!ModelState.IsValid)
            {
                ViewBag.DanhSachMon = _context.MonAns.ToList();
                return View(model);
            }

            var dat = _context.DatMons.Find(model.MaDatMon);
            if (dat == null)
            {
                TempData["Error"] = "Đặt món không tồn tại!";
                return RedirectToAction("DanhSachDatMon");
            }

            dat.MaMon = model.MaMon;
            dat.SoLuong = model.SoLuong;
            dat.GhiChu = model.GhiChu;

            _context.SaveChanges();

            TempData["Success"] = "Cập nhật thành công!";
            return RedirectToAction("DanhSachDatMon");
        }

        [HttpPost]
        public IActionResult XoaDatMon(string id)
        {
            if (!CheckQuyen()) return RedirectToAction("Login", "Account");

            var dat = _context.DatMons.Find(id);

            if (dat == null)
            {
                TempData["Error"] = "Đặt món không tồn tại!";
            }
            else
            {
                _context.DatMons.Remove(dat);
                _context.SaveChanges();
                TempData["Success"] = "Xóa thành công!";
            }

            return RedirectToAction("DanhSachDatMon");
        }
    }
}
