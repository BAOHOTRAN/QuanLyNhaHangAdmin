using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using System.Linq;
using System.Threading.Tasks;

namespace QuanLyNhaHangAdmin.Controllers.Admin
{
    public class AdminLoaiMonAnController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public AdminLoaiMonAnController(QuanLyNhaHangContext context)
        {
            _context = context;
        }

        // DANH SÁCH
        public IActionResult DanhSachLoaiMonAn()
        {
            var ds = _context.LoaiMonAns.ToList();
            return View(ds);
        }

        // CHI TIẾT
        public IActionResult ChiTietLoaiMonAn(string id)
        {
            var loai = _context.LoaiMonAns.FirstOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();

            return View(loai);
        }

        [HttpGet]
        public IActionResult ThemLoaiMonAn()
        {
            return View();
        }


        [HttpPost]
        public IActionResult ThemLoaiMonAn(LoaiMonAn model)
        {
            if (_context.LoaiMonAns.Any(x => x.MaLoai == model.MaLoai))
            {
                ModelState.AddModelError("MaLoai", "Mã loại đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                _context.LoaiMonAns.Add(model);
                _context.SaveChanges();
                TempData["Success"] = "Thêm loại món ăn thành công!";
                return RedirectToAction("DanhSachLoaiMonAn");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult SuaLoaiMonAn(string id)
        {
            var loai = _context.LoaiMonAns.FirstOrDefault(x => x.MaLoai == id);
            if (loai == null) return NotFound();

            return View(id);
        }

        [HttpPost]
        public IActionResult SuaLoaiMonAn(LoaiMonAn model)
        {
            if (ModelState.IsValid)
            {
                _context.LoaiMonAns.Update(model);
                _context.SaveChanges();
                TempData["Success"] = "Cập nhật loại món ăn thành công!";
                return RedirectToAction("DanhSachLoaiMonAn");
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult XoaLoaiMonAn(string id)
        {
            if (id == null) return BadRequest();

            var loai = _context.LoaiMonAns.Find(id);
            if (loai == null) return NotFound();

            return View(loai); 
        }

        [HttpPost, ActionName("XoaLoaiMonAn")]
        public IActionResult XacNhanXoaLoaiMonAn(string id)
        {
            var loai = _context.LoaiMonAns.Find(id);
            if (loai == null) return NotFound();

            _context.LoaiMonAns.Remove(loai);
            _context.SaveChanges();
            TempData["Success"] = "Xóa loại món ăn thành công!";
            return RedirectToAction("DanhSachLoaiMonAn");
        }
    }
}
