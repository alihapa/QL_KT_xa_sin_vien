using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using QL_KT_xa_sin_vien.Services;

namespace QL_KT_xa_sin_vien.Controllers
{
    public class HomeController : Controller
    {
        private readonly QLSinhVienContext db = new QLSinhVienContext();
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration; // thêm dòng này
        private readonly Microsoft.AspNetCore.DataProtection.IDataProtector _protector;

        // Constructor: inject ILogger và IConfiguration
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, Microsoft.AspNetCore.DataProtection.IDataProtectionProvider dataProtectionProvider)
        {
            _logger = logger;
            _configuration = configuration; // gán vào biến private
            _protector = dataProtectionProvider.CreateProtector("AccountActivationProtector");
        }

        [HttpGet]
        public IActionResult KichHoatConfirm()
        {
            var email = TempData["ActivationEmail"] as string;
            var username = TempData["ActivationUsername"] as string;
            var vm = new KichHoatViewModel { MaTaiKhoan = null, Token = null };
            ViewBag.Email = email;
            ViewBag.Username = username;
            return View("KichHoatConfirm", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KichHoatConfirm(string email, string code, string? username)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(code))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập email và mã kích hoạt.";
                return RedirectToAction("KichHoatConfirm");
            }

            var (ok, msg) = ActivationService.VerifyCode(email, code, username);
            if (!ok)
            {
                TempData["ErrorMessage"] = msg;
                return RedirectToAction("KichHoatConfirm");
            }

            // find account and activate
            TaiKhoan? taiKhoan;
            if (!string.IsNullOrEmpty(username))
            {
                taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.Email == email && t.TenDangNhap == username);
            }
            else
            {
                // if only one account exists with the email, activate that; if multiple exist this branch should not be used
                taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.Email == email);
            }
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản tương ứng.";
                return RedirectToAction("DangNhap");
            }
            taiKhoan.TrangThai = "1";
            await db.SaveChangesAsync();
            TempData["SuccessMessage"] = "Kích hoạt thành công. Bạn có thể đăng nhập bây giờ.";
            return RedirectToAction("DangNhap");
        }

        // GET: KichHoatTaiKhoan
        [HttpGet]
        public IActionResult KichHoatTaiKhoan(string? token, string? id)
        {
            // If token and account id provided (from activation email link), show confirmation view
            if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(id))
            {
                var vm = new Models.KichHoatViewModel { Token = token, MaTaiKhoan = id };
                return View("KichHoatTaiKhoan", vm);
            }

            return View("KichHoatRequest", new Models.KichHoatRequestModel());
        }

        // POST: KichHoatTaiKhoan (request activation link by email)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult KichHoatTaiKhoan(Models.KichHoatRequestModel model)
        {
            var email = model?.Email;
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập email để nhận liên kết kích hoạt.";
                return View(new Models.KichHoatRequestModel());
            }

            try
            {
                // find accounts by email
                var accounts = db.TaiKhoans.Where(t => t.Email == email).ToList();
                if (accounts == null || !accounts.Any())
                {
                    TempData["ErrorMessage"] = "Không tìm thấy tài khoản tương ứng với email này.";
                    return View(new Models.KichHoatRequestModel());
                }

                // if multiple accounts share the email and no username was chosen, prompt user to pick
                if (accounts.Count > 1 && string.IsNullOrEmpty(model?.SelectedUsername))
                {
                    var vm = new Models.KichHoatRequestModel
                    {
                        Email = email,
                        Usernames = accounts.Select(a => a.TenDangNhap).ToList()
                    };
                    TempData["ErrorMessage"] = "Có nhiều tài khoản sử dụng email này. Vui lòng chọn tên đăng nhập để gửi mã.";
                    return View("KichHoatRequest", vm);
                }

                var chosenUsername = model?.SelectedUsername;

                // send 6-digit activation code via ActivationService, include username when available
                var (success, message, wait, code) = ActivationService.TrySendCode(email, TimeSpan.FromMinutes(15), 10, 30, chosenUsername);
                if (!success)
                {
                    TempData["ErrorMessage"] = message + (wait > 0 ? $" Vui lòng chờ {wait}s." : "");
                    return View("KichHoatRequest", new Models.KichHoatRequestModel { Email = email, SelectedUsername = chosenUsername });
                }

                var smtpSettings = _configuration.GetSection("Smtp").Get<SmtpSettings>();
                var smtpClient = new SmtpClient(smtpSettings.Host)
                {
                    Port = smtpSettings.Port,
                    Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Password),
                    EnableSsl = smtpSettings.EnableSsl,
                };

                var mail = new MailMessage
                {
                    From = new MailAddress(smtpSettings.User),
                    Subject = "Mã kích hoạt tài khoản",
                    Body = $"Mã kích hoạt của bạn là: {code}\nMã có hiệu lực trong 15 phút.",
                    IsBodyHtml = false,
                };
                mail.To.Add(email);
                smtpClient.Send(mail);

                TempData["SuccessMessage"] = "Mã kích hoạt đã được gửi tới email. Vui lòng kiểm tra hộp thư và nhập mã.";
                TempData["ActivationEmail"] = email;
                if (!string.IsNullOrEmpty(model?.SelectedUsername))
                    TempData["ActivationUsername"] = model.SelectedUsername;
                TempData["ResendSeconds"] = wait;
                return RedirectToAction("KichHoatConfirm");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể gửi email kích hoạt. Lỗi: " + ex.Message;
            }

            return View(new Models.KichHoatRequestModel());
        }

        // POST: KichHoatTaiKhoan/Confirm
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> KichHoatTaiKhoanConfirm(string token, string maTaiKhoan)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(maTaiKhoan))
            {
                TempData["ErrorMessage"] = "Liên kết kích hoạt không hợp lệ.";
                return RedirectToAction("DangNhap");
            }

            var sv = await db.SinhViens.FirstOrDefaultAsync(s => s.ResetToken == token && s.ResetTokenExpiry > DateTime.Now && s.MaTaiKhoan == maTaiKhoan);
            if (sv == null)
            {
                TempData["ErrorMessage"] = "Liên kết kích hoạt không hợp lệ hoặc đã hết hạn.";
                return RedirectToAction("DangNhap");
            }

            var taiKhoan = await db.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == maTaiKhoan);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản.";
                return RedirectToAction("DangNhap");
            }

            taiKhoan.TrangThai = "1"; // activate
            sv.ResetToken = null;
            sv.ResetTokenExpiry = null;
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Kích hoạt thành công. Bạn có thể đăng nhập bây giờ.";
            return RedirectToAction("DangNhap");
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

            // try to get any current contract (by date) for the student
            var hopDong = await db.HopDongs
                .Where(h => h.MaSv == sinhVien.MaSv)
                .OrderByDescending(h => h.NgayKetThuc)
                .FirstOrDefaultAsync();

            if (hopDong == null)
            {
                hopDong = new HopDong
                {
                    MaHopDong = "chưa có hợp đồng",
                    NgayBatDau = null,
                    NgayKetThuc = null
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
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
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
                    new HopDong { MaHopDong = "chưa có mã hợp đồng", MaSv = "chưa có mã sinh viên", MaPhong = "chưa có mã phòng", MaGiuong = "chưa có mã giường" , NgayBatDau = null, NgayKetThuc = null }
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
                    new ThongBao { MaThongBao = "chưa có mã thông báo",NguoiGui ="Chưa có Người Gửi" , NguoiNhan = "chưa có người nhận" ,LoaiThongBao = "chưa có loại thông báo", NoiDung = "chưa có nội dung", ThoiGianGui = null , TrangThai = "chưa có trạng thái" }
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

        [RoleAuthorize("4", "3")]
        public IActionResult KeToan()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            {
                return RedirectToAction("DangNhap");
            }
            if (HttpContext.Session.GetString("userRole") != "4" && HttpContext.Session.GetString("userRole") != "3")
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                return RedirectToAction("Index");
            }

            // Thống kê cho kế toán: phân tách hóa đơn đã thanh toán và chưa thanh toán
            var allHoaDons = db.HoaDons.ToList();
            var paid = allHoaDons.Where(h => !string.IsNullOrEmpty(h.TrangThai) && h.TrangThai.Contains("Đã thanh toán")).OrderByDescending(h => h.NgayXuat).ToList();
            var unpaid = allHoaDons.Where(h => string.IsNullOrEmpty(h.TrangThai) || h.TrangThai.Contains("Chưa thanh toán") || (!h.TrangThai.Contains("Đã thanh toán") && !string.IsNullOrEmpty(h.TrangThai))).OrderByDescending(h => h.NgayXuat).ToList();

            ViewBag.SoLuongHoaDon = allHoaDons.Count;
            ViewBag.SoLuongDaThanhToan = paid.Count;
            ViewBag.SoLuongChuaThanhToan = unpaid.Count;

            // Tổng doanh thu tính theo các hóa đơn đã thanh toán
            decimal tongDoanhThu = paid.Where(h => h.SoTien.HasValue).Sum(h => h.SoTien.Value);
            ViewBag.TongDoanhThu = tongDoanhThu;

            // Lấy 5 hóa đơn gần nhất mỗi nhóm để hiển thị
            ViewBag.RecentPaid = paid.Take(5).ToList();
            ViewBag.RecentUnpaid = unpaid.Take(5).ToList();

            return View();
        }

        public IActionResult IndexBQL()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            {
                // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("DangNhap");
            }
            else if (HttpContext.Session.GetString("userRole") != "2" && HttpContext.Session.GetString("userRole") != "3")
            {
                // Nếu không phải nhân viên, chuyển hướng về trang chính
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này.";
                return RedirectToAction("Index");
            }

            var viewModel = new DashboardBQLViewModel();
            viewModel.Sinh_Vien = db.SinhViens.Include(s => s.MaTaiKhoanNavigation).ToList();
            viewModel.SoLuongSinhVien = viewModel.Sinh_Vien.Count;
            viewModel.Phan_Anh = db.PhanAnhs.ToList();
            viewModel.SoLuongPhanAnh = viewModel.Phan_Anh.Count;
            viewModel.Hop_Dong = db.HopDongs.ToList();
            viewModel.SoLuongHopDong = viewModel.Hop_Dong.Count;
            viewModel.Hoa_Don = db.HoaDons.ToList();
            viewModel.SoLuongHoaDon = viewModel.Hoa_Don.Count;
            viewModel.Thong_Bao = db.ThongBaos.ToList();
            viewModel.SoLuongThongBao = viewModel.Thong_Bao.Count;
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult DangNhap()
        {
            // If middleware set an error message in session, move it to TempData for the view
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString("ErrorMessage")))
            {
                TempData["ErrorMessage"] = HttpContext.Session.GetString("ErrorMessage");
                HttpContext.Session.Remove("ErrorMessage");
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
                    // Respect account status values:
                    // "1" = active, "0" = not activated (awaiting email confirmation), "-1" = banned
                    if (user.TrangThai == "-1")
                    {
                        TempData["ErrorMessage"] = "Tài khoản đã bị cấm. Vui lòng liên hệ quản trị viên.";
                        ViewBag.Username = username;
                        return View();
                    }

                    if (user.TrangThai == "0")
                    {
                        TempData["ErrorMessage"] = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản hoặc <a href=\"/Home/KichHoatTaiKhoan\">yêu cầu gửi lại liên kết</a>.";
                        ViewBag.Username = username;
                        return View();
                    }

                    // Only allow login when TrangThai == "1"
                    if (user.TrangThai == "1")
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

                        if (HttpContext.Session.GetString("userRole") == "3")
                        {
                            //Nếu là admin thì chuyển hướng đến trang quản lý
                            return RedirectToAction("IndexAdmin");
                        }
                        if(HttpContext.Session.GetString("userRole") == "2")
                        {
                            //Nếu là nhân viên thì chuyển hướng đến trang quản lý
                            return RedirectToAction("IndexBQL");
                        }
                        if (HttpContext.Session.GetString("userRole") == "1")
                        {
                            //Nếu là sinh viên thì chuyển hướng đến trang chính
                            return RedirectToAction("Index");
                        }
                        if (HttpContext.Session.GetString("userRole") == "4")
                        {
                            //Nếu là kế toán thì chuyển hướng đến trang kế toán
                            return RedirectToAction("KeToan");
                        }
                    }
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
            //var taikhoan = new TaiKhoan();
            //taikhoan.MaTaiKhoan = Guid.NewGuid().ToString();
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult TaoTaiKhoan(TaiKhoan taikhoan)
        //{
        //    try
        //    {
        //        // kiểm tra vai trò "1" có tồn tại không
        //        var role = db.VaiTros.FirstOrDefault(v => v.MaVaiTro == "1");
        //        if (role == null)
        //        {
        //            ModelState.AddModelError("", "Vai trò mặc định không tồn tại trong hệ thống.");
        //            return View(taikhoan);
        //        }
        //        taikhoan.VaiTro = role.MaVaiTro;
        //        taikhoan.TrangThai = "0";
        //        var hasher = new PasswordHasher<TaiKhoan>();
        //        taikhoan.MatKhauMh = hasher.HashPassword(taikhoan, taikhoan.MatKhauMh);

        //    }
        //    catch (Exception ex)
        //    {
        //        // log ex nếu có
        //        ModelState.AddModelError("", "Lỗi khi tạo tài khoản: " + ex.Message);
        //        return View(taikhoan);
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        db.TaiKhoans.Add(taikhoan);
        //        try
        //        {
        //            db.SaveChanges();
        //        }
        //        catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
        //        {
        //            var msg = ex.InnerException?.Message ?? ex.Message;
        //            if (msg != null && (msg.Contains("UQ_TaiKhoan_tenDangNhap") || msg.Contains("duplicate key") || msg.Contains("tenDangNhap")))
        //            {
        //                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
        //                return View(taikhoan);
        //            }
        //            throw;
        //        }
        //        //try
        //        //{
        //        //    var sv = db.SinhViens.FirstOrDefault(s => s.Email == taikhoan.Email);
        //        //    var token = Guid.NewGuid().ToString();
        //        //    if (sv == null)
        //        //    {
        //        //        sv = new SinhVien
        //        //        {
        //        //            MaSv = Guid.NewGuid().ToString(),
        //        //            HoTen = "chưa có tên",
        //        //            Email = taikhoan.Email,
        //        //            MaTaiKhoan = taikhoan.MaTaiKhoan,
        //        //            ResetToken = token,
        //        //            ResetTokenExpiry = DateTime.Now.AddDays(1)
        //        //        };
        //        //        db.SinhViens.Add(sv);
        //        //        db.SaveChanges();
        //        //    }
        //        //    else
        //        //    {
        //        //        sv.ResetToken = token;
        //        //        sv.ResetTokenExpiry = DateTime.Now.AddDays(1);
        //        //        db.SaveChanges();
        //        //    }

        //        //    var activationLink = Url.Action("KichHoatTaiKhoan", "Home", new { token = token, id = taikhoan.MaTaiKhoan }, Request.Scheme);

        //        //    var smtpSettings = _configuration.GetSection("Smtp").Get<SmtpSettings>();
        //        //    var smtpClient = new SmtpClient(smtpSettings.Host)
        //        //    {
        //        //        Port = smtpSettings.Port,
        //        //        Credentials = new NetworkCredential(smtpSettings.User, smtpSettings.Password),
        //        //        EnableSsl = smtpSettings.EnableSsl,
        //        //    };

        //        //    var mail = new MailMessage
        //        //    {
        //        //        From = new MailAddress(smtpSettings.User),
        //        //        Subject = "Kích hoạt tài khoản",
        //        //        Body = $"Xin chào,\n\nVui lòng nhấp vào liên kết sau để kích hoạt tài khoản của bạn:\n{activationLink}\n\nLiên kết có hiệu lực trong 24 giờ.",
        //        //        IsBodyHtml = false,
        //        //    };
        //        //    if (!string.IsNullOrEmpty(taikhoan.Email))
        //        //        mail.To.Add(taikhoan.Email);

        //        //    smtpClient.Send(mail);
        //        //}
        //        //catch
        //        //{
                   
        //        //}

        //        return RedirectToAction("KichHoatTaiKhoan");
        //    }
        //    else {
        //        // Ghi log hoặc lấy chi tiết lỗi để hiển thị
        //        var errors = ModelState
        //            .Where(ms => ms.Value.Errors.Count > 0)
        //            .Select(ms => new {
        //                Key = ms.Key,
        //                Errors = ms.Value.Errors.Select(e => e.ErrorMessage).ToArray()
        //            }).ToList();

        //        // Ví dụ: lưu vào TempData để hiển thị trên View (chỉ dev)
        //        TempData["ModelErrors"] = System.Text.Json.JsonSerializer.Serialize(errors);

        //        return View(taikhoan);
        //    }
        //}

        public IActionResult DangXuat()
        {
            var user = db.TaiKhoans.FirstOrDefault(u => u.MaTaiKhoan == HttpContext.Session.GetString("userId"));
            if (user != null)
            {
                // Do not change account activation status on logout.
                // Previously this set TrangThai = "0" which incorrectly deactivated accounts.
            }
            HttpContext.Session.Remove("users");
            HttpContext.Session.Remove("userId");
            HttpContext.Session.Remove("userRole");
            SignOut();
            TempData["SuccessMessage"] = "Đăng xuất thành công.";
            return RedirectToAction("DangNhap");
        }

        // GET: QuenMatKhau
        [HttpGet]
        public IActionResult QuenMatKhau()
        {
            // Use the same simple request model as activation flow so we can prompt for username when email maps to multiple accounts
            return View(new Models.KichHoatRequestModel());
        }

        // POST: QuenMatKhau
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(Models.KichHoatRequestModel model)
        {
            var email = model?.Email;
            var selectedUsername = model?.SelectedUsername;

            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập email.";
                return View(new Models.KichHoatRequestModel());
            }

            var sinhViens = await db.SinhViens.Where(s => s.Email == email).ToListAsync();
            if (sinhViens == null || !sinhViens.Any())
            {
                TempData["ErrorMessage"] = "Email không tồn tại trong hệ thống.";
                return View(new Models.KichHoatRequestModel());
            }

            // If multiple students share same email, require the user to choose the corresponding account (username)
            if (sinhViens.Count > 1 && string.IsNullOrEmpty(selectedUsername))
            {
                var usernames = new List<string>();
                foreach (var sv in sinhViens)
                {
                    if (!string.IsNullOrEmpty(sv.MaTaiKhoan))
                    {
                        var acc = await db.TaiKhoans.FirstOrDefaultAsync(t => t.MaTaiKhoan == sv.MaTaiKhoan);
                        if (acc != null && !string.IsNullOrEmpty(acc.TenDangNhap)) usernames.Add(acc.TenDangNhap);
                        else usernames.Add(sv.MaSv ?? sv.HoTen ?? "(không xác định)");
                    }
                    else
                    {
                        usernames.Add(sv.MaSv ?? sv.HoTen ?? "(không xác định)");
                    }
                }

                var vm = new Models.KichHoatRequestModel
                {
                    Email = email,
                    Usernames = usernames
                };
                TempData["ErrorMessage"] = "Có nhiều tài khoản liên kết với email này. Vui lòng chọn tên đăng nhập để tiếp tục.";
                return View("QuenMatKhau", vm);
            }

            // locate the exact SinhVien to reset
            SinhVien svTarget = null;
            if (!string.IsNullOrEmpty(selectedUsername))
            {
                var account = await db.TaiKhoans.FirstOrDefaultAsync(t => t.TenDangNhap == selectedUsername);
                if (account != null)
                {
                    svTarget = await db.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == account.MaTaiKhoan && s.Email == email);
                }
            }

            if (svTarget == null)
            {
                svTarget = await db.SinhViens.FirstOrDefaultAsync(s => s.Email == email);
            }

            if (svTarget == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sinh viên để đặt lại mật khẩu.";
                return View(new Models.KichHoatRequestModel());
            }

            var token = Guid.NewGuid().ToString();
            svTarget.ResetToken = token;
            svTarget.ResetTokenExpiry = DateTime.Now.AddHours(1);
            await db.SaveChangesAsync();

            var resetLink = Url.Action("DatLaiMatKhau", "Home", new { token = token }, Request.Scheme);

            try
            {
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
                    Body = $@"
                        <p>Xin chào {svTarget.HoTen},</p>
                        <p>Vui lòng nhấn vào liên kết sau để đặt lại mật khẩu:</p>
                        <p><a href=""{resetLink}"">Đặt lại mật khẩu</a></p>
                        <p>Liên kết có hiệu lực trong 1 giờ.</p>
                    ",
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await smtpClient.SendMailAsync(mailMessage);

                TempData["SuccessMessage"] = "Yêu cầu đã được gửi. Vui lòng kiểm tra email để đặt lại mật khẩu.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Không thể gửi email. Lỗi: " + ex.Message;
            }

            return View(new Models.KichHoatRequestModel());
        }


        //GET: DatLaiMatKhau
        [HttpGet]
        public IActionResult DatLaiMatKhau(string token)
        {
            var sv = db.SinhViens.FirstOrDefault(s => s.ResetToken == token && s.ResetTokenExpiry > DateTime.Now);
            if (sv == null)
            {// Thay đổi từ RedirectToAction
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
