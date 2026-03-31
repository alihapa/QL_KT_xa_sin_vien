using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    public class HomeController : Controller
    {
        QLSinhVienContext db = new QLSinhVienContext();
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            //kiểm tra session
            if(string.IsNullOrEmpty(HttpContext.Session.GetString("user")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            return View();
        }
        [HttpPost]
        public IActionResult DangNhap(string username, string password)
        {
            // Thực hiện kiểm tra đăng nhập ở đây (ví dụ: so sánh với dữ liệu trong cơ sở dữ liệu)

            var user = db.TaiKhoans.FirstOrDefault(u => u.TenDangNhap == username && u.MatKhauMh == password);

            if (user != null) 
            {
                HttpContext.Session.SetString("user", username);
                return RedirectToAction("Index");
                
            }
            else
            {
                // Đăng nhập thất bại, hiển thị thông báo lỗi
                ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
                return View();
            }
        }

        [HttpGet]
        public IActionResult TaoTaiKhoan()
        {
            return View();
        }
        [HttpPost]
        public IActionResult TaoTaiKhoan(TaiKhoan taikhoan)
        {
            if(ModelState.IsValid)
            {
                taikhoan.MaTaiKhoan = Guid.NewGuid().ToString();
                taikhoan.VaiTro = "1";
                taikhoan.TrangThai = "0";
                db.TaiKhoans.Add(taikhoan);
                db.SaveChanges();
                return RedirectToAction("DangNhap");
            }
            return View(taikhoan);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
