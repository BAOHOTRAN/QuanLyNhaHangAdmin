using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHangAdmin.Controllers.Admin
{
    public class AdminDatMonController : Controller
    {
        private readonly QuanLyNhaHangContext _db;
        public AdminDatMonController(QuanLyNhaHangContext db) => _db = db;
        private async Task LoadDropDown(DatMon? model = null)
        {
            ViewBag.DatBanList = new SelectList(await _db.DatBans.ToListAsync(), "MaDatBan", "MaDatBan", model?.MaDatBan);
            ViewBag.MonAnList = new SelectList(await _db.MonAns.Include(m => m.LoaiMonAn).ToListAsync(), "MaMon", "TenMon", model?.MaMon);
        }
        public async Task<IActionResult> DanhSachDatMon()
        {
            var list = await _db.DatMons
                .Include(d => d.DatBan)
                .Include(d => d.MonAn)
                .ThenInclude(m => m.LoaiMonAn)
                .OrderByDescending(d => d.MaDatMon)
                .ToListAsync();
            return View(list);
        }
        public async Task<IActionResult> ChiTietDatMon(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var model = await _db.DatMons
                .Include(d => d.DatBan)
                .Include(d => d.MonAn)
                    .ThenInclude(m => m.LoaiMonAn)
                .FirstOrDefaultAsync(d => d.MaDatMon == id);

            if (model == null) return NotFound();
            return View(model);
        }
        public async Task<IActionResult> ThemDatMon()
        {
            await LoadDropDown();

            var last = await _db.DatMons.OrderByDescending(x => x.MaDatMon).FirstOrDefaultAsync();
            string newId = "DM001";
            if (last != null && int.TryParse(last.MaDatMon.Substring(2), out int n))
                newId = $"DM{(n + 1):D3}";

            var model = new DatMon
            {
                MaDatMon = newId,
                SoLuong = 1
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThemDatMon(DatMon model)
        {
            await LoadDropDown(model);

            ModelState.Remove("DatBan");
            ModelState.Remove("MonAn");
            ModelState.Remove("MaDatMon");

            if (model.SoLuong <= 0)
                ModelState.AddModelError("SoLuong", "Số lượng phải lớn hơn 0");

            if (!ModelState.IsValid) return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                // Kiểm tra món đã có trong cùng bàn chưa
                var exist = await _db.DatMons
                    .FirstOrDefaultAsync(d => d.MaDatBan == model.MaDatBan && d.MaMon == model.MaMon);

                if (exist != null)
                {
                    exist.SoLuong += model.SoLuong;
                    if (!string.IsNullOrEmpty(model.GhiChu))
                        exist.GhiChu = (exist.GhiChu ?? "") + " | " + model.GhiChu;
                    _db.Update(exist);
                }
                else
                {
                    // Sinh MaDatMon mới nếu thêm món mới
                    var last = await _db.DatMons.OrderByDescending(x => x.MaDatMon).FirstOrDefaultAsync();
                    string newId = "DM001";
                    if (last != null && int.TryParse(last.MaDatMon.Substring(2), out int n))
                        newId = $"DM{(n + 1):D3}";
                    model.MaDatMon = newId;

                    _db.Add(model);
                }

                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Đặt món thành công!";
                return RedirectToAction(nameof(DanhSachDatMon));
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Lỗi khi lưu dữ liệu");
                return View(model);
            }
        }
        public async Task<IActionResult> SuaDatMon(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();
            var model = await _db.DatMons.FindAsync(id);
            if (model == null) return NotFound();

            await LoadDropDown(model);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SuaDatMon(string id, DatMon model)
        {
            if (id != model.MaDatMon) return BadRequest();

            await LoadDropDown(model);
            ModelState.Remove("DatBan");
            ModelState.Remove("MonAn");

            if (model.SoLuong <= 0)
            {
                ModelState.AddModelError("SoLuong", "Số lượng phải lớn hơn 0");
            }

            if (!ModelState.IsValid) return View(model);

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                var existing = await _db.DatMons.FindAsync(id);
                if (existing == null) return NotFound();

                existing.MaDatBan = model.MaDatBan;
                existing.MaMon = model.MaMon;
                existing.SoLuong = model.SoLuong;
                existing.GhiChu = model.GhiChu;

                _db.Update(existing);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(DanhSachDatMon));
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Lỗi khi cập nhật");
                return View(model);
            }
        }

        public async Task<IActionResult> XoaDatMon(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var model = await _db.DatMons
                .Include(d => d.DatBan)
                .Include(d => d.MonAn)
                .FirstOrDefaultAsync(d => d.MaDatMon == id);

            if (model == null) return NotFound();
            return View(model);
        }

        [HttpPost, ActionName("XoaDatMon")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> XoaDatMonConfirmed(string id)
        {
            if (string.IsNullOrEmpty(id)) return BadRequest();

            var model = await _db.DatMons.Include(d => d.DatBan).FirstOrDefaultAsync(d => d.MaDatMon == id);
            if (model == null) return NotFound();

            using var tx = await _db.Database.BeginTransactionAsync();
            try
            {
                if (!string.IsNullOrEmpty(model.MaDatBan))
                {
                    var ban = await _db.BanAns.FindAsync(model.MaDatBan);
                    if (ban != null)
                    {
                        ban.TrangThai = "Trống";
                        _db.Update(ban);
                        await _db.SaveChangesAsync();
                    }
                }

                _db.Remove(model);
                await _db.SaveChangesAsync();
                await tx.CommitAsync();

                TempData["Success"] = "Xóa đặt món thành công!";
                return RedirectToAction(nameof(DanhSachDatMon));
            }
            catch
            {
                await tx.RollbackAsync();
                ModelState.AddModelError("", "Lỗi khi xóa");
                return View(model);
            }
        }
    }
}
