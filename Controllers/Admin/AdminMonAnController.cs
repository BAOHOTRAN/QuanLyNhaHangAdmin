using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;

namespace QuanLyNhaHangAdmin.Controllers.Admin
{
    public class AdminMonAnController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public AdminMonAnController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult DanhSachMonAn()
        {
            var ds = _context.MonAns
                .Include(x => x.LoaiMonAn)
                .ToList();

            return View(ds);
        }

        [HttpGet]
        public async Task<IActionResult> ChiTietMonAn(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound();

            var mon = await _context.MonAns
                .Include(x => x.LoaiMonAn)
                .FirstOrDefaultAsync(x => x.MaMon == id);

            if (mon == null) return NotFound();

            return View(mon);
        }

        [HttpGet]
        public IActionResult ThemMonAn()
        {
            ViewBag.DsLoai = _context.LoaiMonAns.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ThemMonAn(MonAn model, IFormFile AnhMonFile)
        {
            ModelState.Remove("LoaiMonAn");

            if (ModelState.IsValid)
            {
                if (AnhMonFile != null && AnhMonFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/monan");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    var fileName = Path.GetFileName(AnhMonFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhMonFile.CopyToAsync(stream);
                    }

                    model.AnhMon = fileName;
                }

                _context.MonAns.Add(model);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Thêm món ăn thành công";
                return RedirectToAction("DanhSachMonAn");
            }

            ViewBag.DsLoai = _context.LoaiMonAns.ToList();
            return View(model);
        }


        [HttpGet]
        public IActionResult SuaMonAn(string id)
        {
            var mon = _context.MonAns.FirstOrDefault(x => x.MaMon == id);
            if (mon == null) return NotFound();

            ViewBag.DsLoai = _context.LoaiMonAns.ToList();
            return View(mon);
        }


        [HttpPost]
        public async Task<IActionResult> SuaMonAn(MonAn model, IFormFile AnhMonFile)
        {
            ModelState.Remove("LoaiMonAn");

            if (ModelState.IsValid)
            {
                var existing = await _context.MonAns.FindAsync(model.MaMon);
                if (existing == null) return NotFound();

                existing.TenMon = model.TenMon;
                existing.Gia = model.Gia;
                existing.MaLoai = model.MaLoai;
                existing.MoTa = model.MoTa;

                // Xử lý upload ảnh nếu có
                if (AnhMonFile != null && AnhMonFile.Length > 0)
                {
                    var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/monan");
                    if (!Directory.Exists(folderPath))
                        Directory.CreateDirectory(folderPath);

                    // Lưu file
                    var fileName = Path.GetFileName(AnhMonFile.FileName);
                    var filePath = Path.Combine(folderPath, fileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await AnhMonFile.CopyToAsync(stream);
                    }

                    existing.AnhMon = fileName;
                }

                _context.MonAns.Update(existing);
                await _context.SaveChangesAsync();

                TempData["Success"] = "Cập nhật món ăn thành công";
                return RedirectToAction("DanhSachMonAn");
            }

            ViewBag.DsLoai = _context.LoaiMonAns.ToList();
            return View(model);
        }


        [HttpGet]
        public IActionResult XoaMonAn(string id)
        {
            var mon = _context.MonAns.FirstOrDefault(x => x.MaMon == id);
            if (mon == null) return NotFound();
            return View(mon);
        }

        [HttpPost, ActionName("XoaMonAn")]
        public async Task<IActionResult> XacNhanXoaMonAn(string id)
        {
            var mon = await _context.MonAns.FindAsync(id);
            if (mon == null) return NotFound();

            if (!string.IsNullOrEmpty(mon.AnhMon))
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/monan", mon.AnhMon);
                if (System.IO.File.Exists(filePath))
                    System.IO.File.Delete(filePath);
            }

            _context.MonAns.Remove(mon);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa món ăn thành công";
            return RedirectToAction("DanhSachMonAn");
        }

    }
}
