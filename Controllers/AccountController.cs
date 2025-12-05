using Microsoft.AspNetCore.Mvc;
using QuanLyNhaHangAdmin.Data;
using QuanLyNhaHangAdmin.Models;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Text;


namespace QuanLyNhaHangAdmin.Controllers
{
    public class AccountController : Controller
    {
        private readonly QuanLyNhaHangContext _context;

        public AccountController(QuanLyNhaHangContext context)
        {
            _context = context;
        }
        // Dùng tạm, không đủ bảo mật trong thực tế, sau này có thời gian thì nên bảo mật bằng phương pháp thêm mắm thêm muối 
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder sb = new StringBuilder();
                foreach (byte b in bytes)
                {
                    sb.Append(b.ToString("x2"));
                }
                return sb.ToString();
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (model.Role == "Admin")
            {
                ModelState.AddModelError("", "Không được tạo tài khoản Admin!");
                return View(model);
            }

            if (_context.Users.Any(u => u.Username == model.Username))
            {
                ModelState.AddModelError("", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            var user = new User
            {
                Username = model.Username,
                Password = HashPassword(model.Password),   
                FullName = model.FullName,
                Role = model.Role
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Tạo tài khoản thành công, hãy đăng nhập!";
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            string hashed = HashPassword(password);
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.Password == hashed);
            if (user == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View();
            }

            // Lưu giá trị
            HttpContext.Session.SetString("Role", user.Role);
            HttpContext.Session.SetString("Username", user.Username);

            // Điều hướng theo role
            if (user.Role == "Admin")
                return RedirectToAction("TrangGioiThieuAdmin", "TrangChuAdmin");
            else if (user.Role == "KhachHang")
                return RedirectToAction("TrangGioiThieuKhachHang", "TrangChuKhachHang");
            else // KhachHang
                return RedirectToAction("Index", "Home", new { area = "User" });
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
