//using Microsoft.AspNetCore.Mvc;
//using QRCoder;
//using System.Drawing;
//using System.Drawing.Imaging;
//using QuanLyNhaHangAdmin.Data;

//namespace QuanLyNhaHangAdmin.Controllers.Admin
//{
//    public class QRController : Controller
//    {
//        private readonly QuanLyNhaHangContext _context;
//        private readonly IConfiguration _config;

//        public QRController(QuanLyNhaHangContext context, IConfiguration config)
//        {
//            _context = context;
//            _config = config;
//        }

//        public IActionResult TaoMaQR(string maBan)
//        {
//            if (string.IsNullOrEmpty(maBan))
//                return BadRequest("Thiếu mã bàn.");

//            string domain = _config["Domain"] ?? "https://localhost:7201";

//            string url = $"{domain}/KHACHHANGDatBan/DatBanQR?maBan={maBan}";

//            using (QRCodeGenerator qrGenerator = new QRCodeGenerator())
//            using (QRCodeData qrCodeData = qrGenerator.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q))
//            using (QRCode qrCode = new QRCode(qrCodeData))
//            using (Bitmap qrBitmap = qrCode.GetGraphic(20))
//            using (MemoryStream ms = new MemoryStream())
//            {
//                qrBitmap.Save(ms, ImageFormat.Png);
//                return File(ms.ToArray(), "image/png");
//            }
//        }

//        public IActionResult TaoTatCaMaQR()
//        {
//            var listBan = _context.BanAns.ToList();
//            return View(listBan);
//        }
//    }
//}
using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace YourProject.Controllers.Admin
{
    public class AdminQRController : Controller
    {
        // Trang nhập thông tin bàn và tạo QR
        public IActionResult Index()
        {
            return View();
        }

        // Tạo QR Code
        [HttpPost]
        public IActionResult GenerateQR(int SoBan)
        {
            // Link khách hàng sẽ mở khi quét
            string url = $"{Request.Scheme}://{Request.Host}/DatBan/Create?banId={SoBan}";

            QRCodeGenerator qr = new QRCodeGenerator();
            QRCodeData data = qr.CreateQrCode(url, QRCodeGenerator.ECCLevel.Q);
            QRCode code = new QRCode(data);

            Bitmap qrImage = code.GetGraphic(20);

            using var stream = new MemoryStream();
            qrImage.Save(stream, ImageFormat.Png);
            string base64 = Convert.ToBase64String(stream.ToArray());

            ViewBag.QRImg = "data:image/png;base64," + base64;
            ViewBag.SoBan = SoBan;
            ViewBag.Url = url;

            return View("Index");
        }
    }
}


