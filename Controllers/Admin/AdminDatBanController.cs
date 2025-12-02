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
        private void LoadDropDown(DatBan? model = null)
        {
            ViewBag.BanList = new SelectList(
                _db.BanAns.ToList(), "MaBan", "TenBan", model?.MaBan
            );

            ViewBag.KhachList = new SelectList(
                _db.KhachHangs.ToList(), "MaKH", "HoTen", model?.MaKH
            );
        }

        private void ValidateDatBan(DatBan model, string? oldMaBan = null)
        {
            ModelState.Remove("BanAn");
            ModelState.Remove("KhachHang");
            ModelState.Remove("DatMons");
            ModelState.Remove("NhanVien");

            if (model.SoNguoi <= 0)
                ModelState.AddModelError("SoNguoi", "Số người phải lớn hơn 0.");

            if (model.NgayDat.Date < DateTime.Now.Date)
                ModelState.AddModelError("NgayDat", "Ngày đặt không được ở quá khứ.");

            var min = new TimeSpan(7, 0, 0);
            var max = new TimeSpan(22, 0, 0);
            if (model.GioDat < min || model.GioDat > max)
                ModelState.AddModelError("GioDat", "Giờ đặt phải từ 07:00 đến 22:00.");

            if (string.IsNullOrEmpty(model.MaKH))
                ModelState.AddModelError("MaKH", "Khách hàng không được để trống.");
            else if (_db.KhachHangs.Find(model.MaKH) == null)
                ModelState.AddModelError("MaKH", "Khách hàng không tồn tại.");

            if (!string.IsNullOrEmpty(model.MaBan))
            {
                var ban = _db.BanAns.Find(model.MaBan);
                if (ban == null)
                {
                    ModelState.AddModelError("MaBan", "Bàn không tồn tại.");
                }
                else if (ban.TrangThai != "Trống" && ban.TrangThai != "Đặt trước")
                {
                    if (oldMaBan == null || model.MaBan != oldMaBan)
                        ModelState.AddModelError("MaBan", $"Bàn {ban.TenBan} hiện không khả dụng ({ban.TrangThai}).");
                }
            }
        }
        private string GenerateNewId()
        {
            var last = _db.DatBans.OrderByDescending(x => x.MaDatBan).FirstOrDefault();
            if (last == null) return "DB0001";
            int n = int.Parse(last.MaDatBan.Substring(2));
            return $"DB{(n + 1):D4}";
        }

        public async Task<IActionResult> DanhSachDatBan()
        {
            var list = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .OrderByDescending(x => x.NgayDat)
                .ToListAsync();
            return View(list);
        }

        public async Task<IActionResult> ChiTietDatBan(string id)
        {
            if (id == null) return NotFound();

            var model = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.MaDatBan == id);

            if (model == null) return NotFound();
            return View(model);
        }

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
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDatBan(DatBan model)
        {
            LoadDropDown(model);
            ValidateDatBan(model);

            if (!ModelState.IsValid) return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.DatBans.Add(model);
                await _db.SaveChangesAsync();

                if (!string.IsNullOrEmpty(model.MaBan))
                {
                    var ban = await _db.BanAns.FindAsync(model.MaBan);
                    if (ban != null)
                    {
                        ban.TrangThai = "Đặt trước";
                        _db.Update(ban);
                        await _db.SaveChangesAsync();
                    }
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

        public async Task<IActionResult> SuaDatBan(string id)
        {
            if (id == null) return NotFound();

            var model = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.MaDatBan == id);

            if (model == null) return NotFound();

            LoadDropDown(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDatBan(string id, DatBan model)
        {
            if (id != model.MaDatBan) return BadRequest();

            // Lấy bàn cũ để so sánh
            var old = await _db.DatBans.AsNoTracking().FirstOrDefaultAsync(x => x.MaDatBan == id);
            if (old == null) return NotFound();

            LoadDropDown(model);

            ValidateDatBan(model, old.MaBan);

            if (!ModelState.IsValid)
                return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                _db.Update(model);
                await _db.SaveChangesAsync();

                if (old.MaBan != model.MaBan)
                {
                    if (!string.IsNullOrEmpty(old.MaBan))
                    {
                        var oldBan = await _db.BanAns.FindAsync(old.MaBan);
                        if (oldBan != null)
                        {
                            oldBan.TrangThai = "Trống";
                            _db.Update(oldBan);
                            await _db.SaveChangesAsync();
                        }
                    }

                    if (!string.IsNullOrEmpty(model.MaBan))
                    {
                        var newBan = await _db.BanAns.FindAsync(model.MaBan);
                        if (newBan != null)
                        {
                            newBan.TrangThai = "Đặt trước";
                            _db.Update(newBan);
                            await _db.SaveChangesAsync();
                        }
                    }
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

        [HttpGet]
        public async Task<IActionResult> XoaDatBan(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var model = await _db.DatBans
                .Include(x => x.BanAn)
                .Include(x => x.KhachHang)
                .FirstOrDefaultAsync(x => x.MaDatBan == id);

            if (model == null) return NotFound();

            return View(model);
        }

        [HttpPost, ActionName("XoaDatBan")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDatBanConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var item = await _db.DatBans.FindAsync(id);
            if (item == null) return NotFound();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(item.MaBan))
                {
                    var ban = await _db.BanAns.FindAsync(item.MaBan);
                    if (ban != null)
                    {
                        ban.TrangThai = "Trống";
                        _db.Update(ban);
                        await _db.SaveChangesAsync();
                    }
                }
                _db.Remove(item);
                await _db.SaveChangesAsync();

                await tx.CommitAsync();
                TempData["Success"] = "Xóa đặt bàn thành công!";
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
