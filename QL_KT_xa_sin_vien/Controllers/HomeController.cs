using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;

namespace QL_KT_xa_sin_vien.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLSinhVienContext db = new QLSinhVienContext();
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration; // thêm dòng này

        // Constructor: inject ILogger và IConfiguration
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration; // gán vào biến private
        }

        public async Task<IActionResult> Index()
        {
            //kiểm tra session
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }
           
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"];
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
        [RoleAuthorize("3")]
        public IActionResult IndexAdmin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }
            if (HttpContext.Session.GetString("userRole") != "3")
            {
                // Nếu không phải admin, chuyển hướng về trang chính
                return RedirectToAction("Index");
            }

            var viewModel = new DashboardADMINViewModel();

            var sinhViens = db.SinhViens.Include(s => s.MaTaiKhoanNavigation).ToList();
            viewModel.SoLuongSinhVien = sinhViens.Count;
            if (sinhViens == null || !sinhViens.Any())
            {
                sinhViens = new List<SinhVien>
                {
                    new SinhVien { MaSv = "chưa có mã sinh viên", HoTen = "chưa có tên", Lop = "chưa có lớp", Khoa = "chưa có khoa", SoCmnd = "chưa có số chứng minh nhân dân", Email = "chưa có email", MaTaiKhoan = "chưa có mã tài khoản"}
                };
            }

            var taikhoans = db.TaiKhoans.ToList();
            if (taikhoans == null || !taikhoans.Any())
            {
                taikhoans = new List<TaiKhoan>
                {
                    new TaiKhoan { MaTaiKhoan = "chưa có mã tài khoản", TenDangNhap = "chưa có tên đăng nhập", MatKhauMh = "chưa có mật khẩu" , Email = "chưa có email" , Sdt = "chưa có số điện thoại", VaiTro = "chưa có vai trò", TrangThai = "chưa có trạng thái" }
                };
            }

            var giuongs = db.Giuongs.ToList();
            if (giuongs == null || !giuongs.Any())
            {
                giuongs = new List<Giuong>
                {
                    new Giuong { MaGiuong = "chưa có mã giường", MaPhong = "chưa có mã phòng", SoGiuong = "chưa có số giường" , OccupiedBy = "chưa có người sở hữu" , TrangThai = "chưa có trạng thái" }
                };
            }

            var phongs = db.Phongs.ToList();
            viewModel.SoLuongPhong = phongs.Count;
            if (phongs == null || !phongs.Any())
            {
                phongs = new List<Phong>
                {
                    new Phong { MaPhong = "chưa có mã phòng", MaToaNha = "chưa có mã tòa nhà", Tang = 0 , LoaiPhong = "chưa có loại phòng", SucChua = 0 , SoLuongDangO = 0 , GioiTinh = "chưa có giới tính" , TrangThai = "chưa có trạng thái" }
                };
            }

            var toaNhas = db.ToaNhas.ToList();
            if (toaNhas == null || !toaNhas.Any())
            {
                toaNhas = new List<ToaNha>
                {
                    new ToaNha { MaToaNha = "chưa có mã tòa nhà", TenToaNha = "chưa có tên tòa nhà", DiaChi = "chưa có địa chỉ" }
                };
            }

            var hopDongs = db.HopDongs.ToList();
            if (hopDongs == null || !hopDongs.Any())
            {
                hopDongs = new List<HopDong>
                {
                    new HopDong { MaHopDong = "chưa có mã hợp đồng", MaSv = "chưa có mã sinh viên", MaPhong = "chưa có mã phòng", MaGiuong = "chưa có mã giường" , NgayBatDau = null, NgayKetThuc = null, TrangThai = "chưa có trạng thái" , DieuKhoan = "chưa có điều khoản"}
                };
            }

            var hoaDons = db.HoaDons.ToList();
            viewModel.SoLuongHoaDon = hoaDons.Count;
            if (hoaDons == null || !hoaDons.Any())
            {
                hoaDons = new List<HoaDon>
                {
                    new HoaDon { MaHoaDon = "chưa có mã hóa đơn", MaHopDong = "chưa có mã hợp đồng", MaSv = "chưa có mã sinh viên" , SoTien = 0 , NgayXuat = null , TrangThai = "chưa có trạng thái" }
                };
            }

            var phanAnhs = db.PhanAnhs.ToList();
            viewModel.SoLuongPhanAnh = phanAnhs.Count;
            if (phanAnhs == null || !phanAnhs.Any())
            {
                phanAnhs = new List<PhanAnh>
                {
                    new PhanAnh { MaPhanAnh = "chưa có mã phản ánh", MaSv = "chưa có mã sinh viên",MaPhong = "chưa có mã phòng" , MoTa = "chưa có mô tả", MucDoUuTien = "chưa có mức độ ưu tiên" , TrangThai = "chưa có trạng thái" , NguoiXuLy = "chưa có người xử lý" , ThoiGianTao = null , ThoiGianCapNhat = null}
                };
            }

            var nhatKis = db.NhatKies.ToList();
            if (nhatKis == null || !nhatKis.Any())
            {
                nhatKis = new List<NhatKy>
                {
                    new NhatKy { MaLog = "chưa có mã nhật ký", NguoiThucHien = "chưa có người thực hiện" , HanhDong = "chưa có hành động", DoiTuong = "chưa có đối tượng", GiaTriTruoc = "chưa có giá trị trước", GiaTriSau = "chưa có giá trị sau" , ThoiGian = null}
                };
            }

            var thongBaos = db.ThongBaos.ToList();
            if (thongBaos == null || !thongBaos.Any())
            {
                thongBaos = new List<ThongBao>
                {
                    new ThongBao { MaThongBao = "chưa có mã thông báo", NguoiNhan = "chưa có người nhận" ,LoaiThongBao = "chưa có loại thông báo", NoiDung = "chưa có nội dung", ThoiGianGui = null , TrangThai = "chưa có trạng thái" }
                };
            }

            var vaiTros = db.VaiTros.ToList();
            if (vaiTros == null || !vaiTros.Any())
            {
                vaiTros = new List<VaiTro>
                {
                    new VaiTro { MaVaiTro = "chưa có mã vai trò", TenVaiTro = "chưa có tên vai trò" , QuyenHan = "chưa có quyền hạn"}
                };
            }

            viewModel.HopDongs = hopDongs;
            viewModel.PhanAnhs = phanAnhs;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            if (HttpContext.Items.ContainsKey("ErrorMessage"))
            {
                TempData["ErrorMessage"] = HttpContext.Items["ErrorMessage"];
            }
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
                        HttpContext.Session.SetString("userRole", user.VaiTro); // Lưu vai trò vào session nếu cần thiết
                    }
                    user.TrangThai = "1"; // Cập nhật trạng thái đăng nhập
                    db.SaveChanges();

                    if (HttpContext.Session.GetString("userRole") == "3")
                    {
                        //Nếu là admin thì chuyển hướng đến trang quản lý
                        return RedirectToAction("IndexAdmin");
                    }

                    return RedirectToAction("Index");
                }
            }
                // Đăng nhập thất bại, hiển thị thông báo lỗi
            TempData["ErrorMessage"] = "Tên đăng nhập hoặc mật khẩu không đúng!";
            ViewBag.Username = username; // Giữ lại tên đăng nhập đã nhập để người dùng không phải gõ lại
            return View();
            
        }

        [HttpGet]
        public IActionResult TaoTaiKhoan()
        {
            var taikhoan = new TaiKhoan();
            taikhoan.MaTaiKhoan = Guid.NewGuid().ToString();
            return View(taikhoan);
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
            HttpContext.Session.Remove("userRole");
            SignOut();
            
            return RedirectToAction("DangNhap");
        }

        // GET: QuenMatKhau
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            return View();
        }

        // POST: QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập email.";
                return View();
            }

            var sv = await db.SinhViens.FirstOrDefaultAsync(s => s.Email == email);
            if (sv == null)
            {
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống.";
                return View();
            }

            var token = Guid.NewGuid().ToString();
            sv.ResetToken = token;
            sv.ResetTokenExpiry = DateTime.Now.AddHours(1);
            await db.SaveChangesAsync();

            var resetLink = Url.Action("DatLaiMatKhau", "Home", new { token = token }, Request.Scheme);

            try
            {
                // Lấy cấu hình SMTP từ appsettings + secrets
                var smtpSettings = _configuration.GetSection("Smtp").Get<SmtpSettings>();

                var smtpClient = new SmtpClient(smtpSettings.Host)
                {
                    Port = smtpSettings.Port,
                    Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpSettings.User),
                    Subject = "Đặt lại mật khẩu",
                    Body = $"Xin chào {sv.HoTen},\n\nVui lòng nhấn vào liên kết sau để đặt lại mật khẩu:\n{resetLink}\n\nLiên kết có hiệu lực trong 1 giờ.",
                    IsBodyHtml = false,
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);

                TempData["SuccessMessage"] = "Yêu cầu đã được gửi. Vui lòng kiểm tra email để đặt lại mật khẩu.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể gửi email. Lỗi: " + ex.Message;
            }

            return View();
        }


        //GET: DatLaiMatKhau
        [HttpGet]
        public IActionResult DatLaiMatKhau(string token)
        {
            var sv = db.SinhViens.FirstOrDefault(s => s.ResetToken == token && s.ResetTokenExpiry > DateTime.Now);
            if (sv == null)
            {
                TempData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("QuenMatKhau");
            }

            return View(new DatLaiMatKhauViewModel { Token = token });
        }

        // POST: DatLaiMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DatLaiMatKhau(DatLaiMatKhauViewModel model)
        {
            var sv = await db.SinhViens
                .FirstOrDefaultAsync(s => s.ResetToken == model.Token && s.ResetTokenExpiry > DateTime.Now);

            if (sv == null)
            {
                TempData["ErrorMessage"] = "Token không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("QuenMatKhau");
            }

            // Lấy tài khoản từ mã sinh viên
            var taiKhoan = await db.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == sv.MaTaiKhoan);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("QuenMatKhau");
            }

            // Hash mật khẩu mới
            var hasher = new PasswordHasher<TaiKhoan>();
            taiKhoan.MatKhauMh = hasher.HashPassword(taiKhoan, model.NewPassword);

            // Xóa token reset
            sv.ResetToken = null;
            sv.ResetTokenExpiry = null;

            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Mật khẩu đã được đặt lại thành công!";
            return RedirectToAction("DangNhap");
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
