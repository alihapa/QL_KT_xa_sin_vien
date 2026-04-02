using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;
using Microsoft.AspNetCore.Identity;

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

        public async Task<IActionResult> Index()
        {
            //kiểm tra session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }

            // Lấy sinh viên theo tài khoản
            var sinhVien = await db.SinhViens
                .FirstOrDefaultAsync(s => s.MaTaiKhoan == HttpContext.Session.GetString("userId"));

            if (sinhVien == null)
            {
                sinhVien = new SinhVien
                {
                    HoTen = "chưa có tên",
                    MaSv = "chưa có mã sinh viên",
                    Lop = "chưa có lớp",
                    Email = "chưa có email"
                };
            }

            // Lấy hóa đơn
            var hoaDons = await db.HoaDons
                .Where(hd => hd.MaSv == sinhVien.MaSv)
                .OrderByDescending(hd => hd.NgayXuat)
                .ToListAsync();
            
            // Lấy phản ánh
            var phanAnhs = await db.PhanAnhs
                .Where(pa => pa.MaSv == sinhVien.MaSv)
                .OrderByDescending(pa => pa.ThoiGianTao)
                .ToListAsync();

            if (phanAnhs == null || !phanAnhs.Any())
            {
                phanAnhs = new List<PhanAnh>
                {
                    new PhanAnh { MaPhanAnh = "chưa có phản ánh", TrangThai = "chưa có thông tin" }
                };
            }

            // Lấy thông báo
            var thongBaos = await db.ThongBaos
                .Where(tb => tb.NguoiNhan == HttpContext.Session.GetString("userId"))
                .OrderByDescending(tb => tb.ThoiGianGui)
                .ToListAsync();

            if (thongBaos == null || !thongBaos.Any())
            {
                thongBaos = new List<ThongBao>
                {
                    new ThongBao { MaThongBao = "chưa có thông báo", NoiDung = "chưa có thông tin" }
                };
            }

            var hopDong = await db.HopDongs
            .FirstOrDefaultAsync(h => h.MaSv == sinhVien.MaSv && h.TrangThai == "1");

            if (hopDong == null)
            {
                hopDong = new HopDong
                {
                    MaHopDong = "chưa có hợp đồng",
                    NgayBatDau = null,
                    NgayKetThuc = null,
                    TrangThai = "chưa có thông tin"
                };
            }

            Phong phong = null;
            if (hopDong != null)
            {
                phong = await db.Phongs.FirstOrDefaultAsync(p => p.MaPhong == hopDong.MaPhong);
            }

            if (phong == null)
            {
                phong = new Phong
                {
                    MaPhong = "chưa có phòng",
                    LoaiPhong = "chưa có thông tin",
                    TrangThai = "chưa có thông tin"
                };
            }

            var vm = new DashboardViewModel
            {
                SinhVien = sinhVien,
                Phong = phong,
                HopDong = hopDong,
                HoaDons = hoaDons,
                PhanAnhs = phanAnhs,
                ThongBaos = thongBaos
            };

            return View(vm);
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

            var user = db.TaiKhoans.SingleOrDefault(u => u.TenDangNhap == username);
            if (user != null) 
            {
                var hasher = new PasswordHasher<TaiKhoan>();
                var result = hasher.VerifyHashedPassword(user, user.MatKhauMh, password);

                if (result == PasswordVerificationResult.Success)
                {
                    var userId = user.MaTaiKhoan;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var tenSinhVien = db.SinhViens
                            .Where(s => s.MaTaiKhoan == userId)
                            .Select(s => s.HoTen)
                            .FirstOrDefault();
                        if (tenSinhVien == null)
                            tenSinhVien = "chưa có tên";
                        // tenSinhVien có thể null nếu chưa có sinh viên liên kết
                        HttpContext.Session.SetString("users", tenSinhVien);
                        HttpContext.Session.SetString("userId", userId); // Lưu MaTaiKhoan vào session để sử dụng sau này
                    }
                    user.TrangThai = "1"; // Cập nhật trạng thái đăng nhập
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
                // Đăng nhập thất bại, hiển thị thông báo lỗi
            ViewBag.ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng.";
            return View();
            
        }

        [HttpGet]
        public IActionResult TaoTaiKhoan()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult TaoTaiKhoan(TaiKhoan taikhoan)
        {
            

            try
            {
                // kiểm tra vai trò "1" có tồn tại không
                var role = db.VaiTros.FirstOrDefault(v => v.MaVaiTro == "1");
                if (role == null)
                {
                    ModelState.AddModelError("", "Vai trò mặc định không tồn tại trong hệ thống.");
                    return View(taikhoan);
                }

                // gán khóa chính
                taikhoan.MaTaiKhoan = Guid.NewGuid().ToString();

                // gán vai trò (phải khớp MaVaiTro)
                taikhoan.VaiTro = role.MaVaiTro;

                // trạng thái mặc định
                taikhoan.TrangThai = "0";

                // (tùy chọn) hash mật khẩu trước khi lưu
                // taikhoan.MatKhauMh = HashPassword(taikhoan.MatKhauMh);
                // Hash mật khẩu
                var hasher = new PasswordHasher<TaiKhoan>();
                taikhoan.MatKhauMh = hasher.HashPassword(taikhoan, taikhoan.MatKhauMh);

            }
            catch (Exception ex)
            {
                // log ex nếu có
                ModelState.AddModelError("", "Lỗi khi tạo tài khoản: " + ex.Message);
                return View(taikhoan);
            }

            if (ModelState.IsValid)
            {
                db.TaiKhoans.Add(taikhoan);
                db.SaveChanges();

                return RedirectToAction("DangNhap");
            }
            else {
                // Ghi log hoặc lấy chi tiết lỗi để hiển thị
                var errors = ModelState
                    .Where(ms => ms.Value.Errors.Count > 0)
                    .Select(ms => new {
                        Key = ms.Key,
                        Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    }).ToList();

                // Ví dụ: lưu vào TempData để hiển thị trên View (chỉ dev)
                TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(errors);

                return View(taikhoan);
            }
        }

        public IActionResult DangXuat()
        {
            var user = db.TaiKhoans.FirstOrDefault(u => u.MaTaiKhoan == HttpContext.Session.GetString("userId"));
            if (user != null)
            {
                user.TrangThai = "0"; // Cập nhật trạng thái đăng xuất
                db.SaveChanges();
            }
            HttpContext.Session.Remove("user");
            HttpContext.Session.Remove("userId");
            SignOut();
            
            return RedirectToAction("DangNhap");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
