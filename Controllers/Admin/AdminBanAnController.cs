using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;

namespace QuanLyNhaHangAdmin.Controllers.Admin
{
    public class AdminBanAnController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public AdminBanAnController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        private void LoadTrangThai()
        {
            ViewBag.TrangThaiList = new List<string>
            {
                "Trống",
                "Đang phục vụ",
                "Đặt trước"
            };
        }

        public IActionResult DanhSachBanAn()
        {
            var list = _context.BanAns.ToList();
            return View(list);
        }

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var ban = await _context.BanAns.FirstOrDefaultAsync(x => x.MaBan == id);
            if (ban == null)
                return NotFound();

            return View(ban);
        }

        public IActionResult Create()
        {
            LoadTrangThai();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BanAn model)
        {
            LoadTrangThai();

            if (_context.BanAns.Any(x => x.MaBan == model.MaBan))
            {
                ModelState.AddModelError("MaBan", "Mã bàn đã tồn tại.");
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            _context.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction("DanhSachBanAn");

        }

        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var ban = await _context.BanAns.FindAsync(id);
            if (ban == null)
                return NotFound();

            LoadTrangThai();
            return View(ban);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, BanAn model)
        {
            if (id != model.MaBan)
                return NotFound();

            LoadTrangThai();

            if (!ModelState.IsValid)
                return View(model);

            try
            {
                _context.Update(model);
                await _context.SaveChangesAsync();
            }
            catch
            {
                ModelState.AddModelError("", "Không thể cập nhật bàn ăn lúc này.");
                return View(model);
            }

            return RedirectToAction("DanhSachBanAn");
        }

        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var ban = await _context.BanAns.FirstOrDefaultAsync(x => x.MaBan == id);
            if (ban == null)
                return NotFound();

            return View(ban);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var ban = await _context.BanAns.FindAsync(id);
            if (ban == null)
                return NotFound();

            if (_context.DatBans.Any(x => x.MaBan == id))
            {
                TempData["Error"] = "Không thể xóa bàn này vì đang được sử dụng trong đặt bàn.";
                return RedirectToAction("DanhSachBanAn");

            }

            _context.BanAns.Remove(ban);
            await _context.SaveChangesAsync();

            return RedirectToAction("DanhSachBanAn");

        }
    }
}
