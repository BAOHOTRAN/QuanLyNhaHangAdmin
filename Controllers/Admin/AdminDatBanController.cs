using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;

namespace QuanLyNhaHangAdmin.Controllers.Admin
{
    public class AdminDatBanController : Controller
    {
        private readonly QuanLyNhaHangContext _db;
        public AdminDatBanController(QuanLyNhaHangContext db) => _db = db;

        // ========================================
        // LOAD DROPDOWN
        // ========================================
        private void LoadDropDown(DatBan? model = null)
        {
            ViewBag.BanList = new SelectList(
                _db.BanAns.OrderBy(x => x.TenBan).ToList(),
                "MaBan", "TenBan", model?.MaBan
            );

            ViewBag.KhachList = new SelectList(
                _db.KhachHangs.OrderBy(x => x.HoTen).ToList(),
                "MaKH", "HoTen", model?.MaKH
            );
        }

        // ========================================
        // TRẠNG THÁI
        // ========================================
        private void LoadTrangThai(string? selected = null)
        {
            ViewBag.TrangThaiList = new List<SelectListItem>
            {
                new("Chờ xác nhận","Chờ xác nhận"),
                new("Đặt trước","Đặt trước"),
                new("Đang phục vụ","Đang phục vụ"),
                new("Hoàn thành","Hoàn thành")
            };

            foreach (var item in ViewBag.TrangThaiList)
                item.Selected = item.Value == selected;
        }

        // ========================================
        // UPDATE TRẠNG THÁI BÀN
        // ========================================
        private async Task CapNhatTrangThaiBan(string? maBan, string trangThai)
        {
            if (string.IsNullOrEmpty(maBan)) return;
            var ban = await _db.BanAns.FindAsync(maBan);
            if (ban == null) return;

            ban.TrangThai = trangThai switch
            {
                "Chờ xác nhận" => "Đặt trước",
                "Đặt trước" => "Đặt trước",
                "Đang phục vụ" => "Đang phục vụ",
                "Hoàn thành" => "Trống",
                _ => ban.TrangThai
            };

            await _db.SaveChangesAsync();
        }

        // ========================================
        // KIỂM TRA TRÙNG GIỜ ĐẶT BÀN
        // ========================================
        private bool IsBanTrungGio(string maBan, DateTime ngay, TimeSpan gio, string? excludeId = null)
        {
            return _db.DatBans.Any(x =>
                x.MaBan == maBan &&
                x.NgayDat == ngay &&
                x.GioDat == gio &&
                x.MaDatBan != excludeId
            );
        }

        private bool IsKhachTrungGio(string maKH, DateTime ngay, TimeSpan gio, string? excludeId = null)
        {
            return _db.DatBans.Any(x =>
                x.MaKH == maKH &&
                x.NgayDat == ngay &&
                x.GioDat == gio &&
                x.MaDatBan != excludeId
            );
        }

        // ========================================
        // VALIDATE
        // ========================================
        private void ValidateDatBan(DatBan model, DatBan? old = null)
        {
            ModelState.Remove("BanAn");
            ModelState.Remove("KhachHang");
            ModelState.Remove("DatMons");
            ModelState.Remove("NhanVien");

            // RÀNG BUỘC SỐ NGƯỜI
            if (model.SoNguoi <= 0)
                ModelState.AddModelError("SoNguoi", "Số người phải lớn hơn 0.");

            // NGÀY KHÔNG ĐƯỢC QUÁ KHỨ
            if (model.NgayDat.Date < DateTime.Now.Date)
                ModelState.AddModelError("NgayDat", "Ngày đặt phải từ hôm nay trở đi.");

            // GIỜ VALID
            var min = new TimeSpan(7, 0, 0);
            var max = new TimeSpan(22, 0, 0);
            if (model.GioDat < min || model.GioDat > max)
                ModelState.AddModelError("GioDat", "Giờ đặt phải từ 07:00 đến 22:00.");

            // KIỂM TRA KHÁCH
            if (string.IsNullOrEmpty(model.MaKH))
                ModelState.AddModelError("MaKH", "Khách hàng không được để trống.");
            else if (_db.KhachHangs.Find(model.MaKH) == null)
                ModelState.AddModelError("MaKH", "Khách hàng không tồn tại.");

            // KIỂM TRA BÀN
            if (!string.IsNullOrEmpty(model.MaBan))
            {
                var ban = _db.BanAns.Find(model.MaBan);
                if (ban == null)
                {
                    ModelState.AddModelError("MaBan", "Bàn không tồn tại.");
                }
            }

            string? excludeId = old?.MaDatBan;

            // ---- KIỂM TRA TRÙNG GIỜ ----
            if (!string.IsNullOrEmpty(model.MaBan) &&
                IsBanTrungGio(model.MaBan, model.NgayDat, model.GioDat, excludeId))
                ModelState.AddModelError("MaBan", "Bàn này đã có người đặt cùng giờ.");

            if (!string.IsNullOrEmpty(model.MaKH) &&
                IsKhachTrungGio(model.MaKH, model.NgayDat, model.GioDat, excludeId))
                ModelState.AddModelError("MaKH", "Khách hàng này đã đặt bàn khác cùng giờ.");
        }

        // ========================================
        // AUTO ID
        // ========================================
        private string GenerateNewId()
        {
            var last = _db.DatBans.OrderByDescending(x => x.MaDatBan).FirstOrDefault();
            if (last == null) return "DB0001";

            int n = int.Parse(last.MaDatBan.Substring(2));
            return $"DB{(n + 1):D4}";
        }

        // ========================================
        // LIST
        // ========================================
        public async Task<IActionResult> DanhSachDatBan()
        {
            var list = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
            return View(list);
        }

        // ========================================
        // CHI TIẾT
        // ========================================
        public async Task<IActionResult> ChiTietDatBan(string id)
        {
            if (id == null) return NotFound();

            var model = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.MaDatBan == id);

            return model == null ? NotFound() : View(model);
        }

        // ========================================
        // THÊM
        // ========================================
        public IActionResult ThemDatBan()
        {
            var model = new DatBan
            {
                MaDatBan = GenerateNewId(),
                NgayDat = DateTime.Now.Date,
                GioDat = DateTime.Now.TimeOfDay,
                TrangThai = "Chờ xác nhận"
            };

            LoadDropDown(model);
            LoadTrangThai(model.TrangThai);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDatBan(DatBan model)
        {
            LoadDropDown(model);
            LoadTrangThai(model.TrangThai);
            ValidateDatBan(model);

            if (!ModelState.IsValid) return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.DatBans.Add(model);
                await _db.SaveChangesAsync();

                if (!string.IsNullOrEmpty(model.TrangThai))
                {
                    await CapNhatTrangThaiBan(model.MaBan, model.TrangThai);
                }
                else
                {
                    await CapNhatTrangThaiBan(model.MaBan, "Trống");
                }

                await tx.CommitAsync();
                TempData["Success"] = "Tạo đặt bàn thành công!";
                return RedirectToAction(nameof(DanhSachDatBan));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View(model);
            }
        }

        // ========================================
        // SỬA
        // ========================================
        [HttpGet]
        public async Task<IActionResult> SuaDatBan(string id)
        {
            if (id == null) return BadRequest();

            var model = await _db.DatBans.AsNoTracking().FirstOrDefaultAsync(x => x.MaDatBan == id);
            if (model == null) return NotFound();

            LoadDropDown(model);
            LoadTrangThai(model.TrangThai);

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> SuaDatBan(string id, DatBan model)
        {
            if (id != model.MaDatBan) return BadRequest();

            var old = await _db.DatBans.AsNoTracking().FirstOrDefaultAsync(x => x.MaDatBan == id);
            if (old == null) return NotFound();

            LoadDropDown(model);
            LoadTrangThai(model.TrangThai);
            ValidateDatBan(model, old);

            if (!ModelState.IsValid) return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Update(model);
                await _db.SaveChangesAsync();

                if (old.MaBan != model.MaBan && old.MaBan != null)
                    await CapNhatTrangThaiBan(old.MaBan, "Hoàn thành");

                if (!string.IsNullOrEmpty(model.TrangThai))
                {
                    await CapNhatTrangThaiBan(model.MaBan, model.TrangThai);
                }
                else
                {
                    await CapNhatTrangThaiBan(model.MaBan, "Trống");
                }

                await tx.CommitAsync();
                TempData["Success"] = "Cập nhật đặt bàn thành công!";
                return RedirectToAction(nameof(DanhSachDatBan));
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Lỗi hệ thống: " + ex.Message);
                return View(model);
            }
        }

        // ========================================
        // XÓA
        // ========================================
        [HttpGet]
        public async Task<IActionResult> XoaDatBan(string id)
        {
            if (id == null) return BadRequest();

            var model = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.MaDatBan == id);

            return model == null ? NotFound() : View(model);
        }

        [HttpPost, ActionName("XoaDatBan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDatBanConfirmed(string id)
        {
            var item = await _db.DatBans.FindAsync(id);
            if (item == null) return NotFound();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                await CapNhatTrangThaiBan(item.MaBan, "Hoàn thành");
                _db.Remove(item);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                TempData["Success"] = "Xóa thành công!";
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                TempData["Error"] = "Không thể xóa: " + ex.Message;
            }

            return RedirectToAction(nameof(DanhSachDatBan));
        }
    }
}
