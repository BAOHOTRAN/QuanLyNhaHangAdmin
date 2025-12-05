using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using QRCoder;
using System.Text;

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

        public IActionResult ThemBanAn()
        {
            LoadTrangThai();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemBanAn(BanAn model)
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
            TempData["Success"] = $"Thêm bàn {model.TenBan} thành công!";
            return RedirectToAction("DanhSachBanAn");

        }

        public async Task<IActionResult> SuaBanAn(string id)
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
        public async Task<IActionResult> SuaBanAn(string id, BanAn model)
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
                TempData["Success"] = $"Cập nhật bàn {model.TenBan} thành công!";
            }
            catch
            {
                ModelState.AddModelError("", "Không thể cập nhật bàn ăn lúc này.");
                return View(model);
            }

            return RedirectToAction("DanhSachBanAn");
        }

        public async Task<IActionResult> XoaBanAn(string id)
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
        public async Task<IActionResult> XoaBanAnConfirmed(string id)
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
            TempData["Success"] = $"Xóa bàn {ban.TenBan} thành công!";
            return RedirectToAction("DanhSachBanAn");

        }
        public IActionResult TaoQR(string id)
        {
            if (id == null)
                return NotFound();

            // Tạo URL để khách quét sẽ mở trang thông tin bàn
            string url = $"{Request.Scheme}://{Request.Host}/Admin/AdminBanAn/ThongTinBan/{id}";

            // Tạo QR
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrCode = new PngByteQRCode(qrCodeData);
            byte[] qrBytes = qrCode.GetGraphic(20);

            // Convert sang Base64 để hiển thị trên View
            string qrBase64 = "data:image/png;base64," + Convert.ToBase64String(qrBytes);
            ViewBag.QR = qrBase64;
            ViewBag.MaBan = id;

            return View();
        }

        public async Task<IActionResult> ThongTinBan(string id)
        {
            if (id == null)
                return NotFound();

            var ban = await _context.BanAns.FirstOrDefaultAsync(x => x.MaBan == id);
            if (ban == null)
                return NotFound();

            return View(ban);
        }
    }
}
