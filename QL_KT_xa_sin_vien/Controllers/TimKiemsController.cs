using Microsoft.AspNetCore.Mvc;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3", "4")]
    public class TimKiemsController : Controller
    {
        private readonly QLSinhVienContext _context;
        public TimKiemsController(QLSinhVienContext context)
        {
            _context = context;
        }
        public IActionResult TimKiem(string searchString, string model, string sortOrder, int page = 1, int pageSize = 5)
        {
            // Normalize search string (allow empty to mean 'all')
            searchString = searchString ?? string.Empty;
            var userRole = HttpContext.Session.GetString("userRole");

            // Determine allowed models per role
            var allModels = new[] { "SinhVien", "PhanAnh", "HopDong", "HoaDon", "ThongBao", "TaiKhoan", "NhatKy", "VaiTro", "Giuong", "Phong", "ToaNha" };
            string[] allowedModels;
            // Role mapping: 1 = SinhVien, 2 = BQL, 3 = Admin, 4 = KeToan
            if (userRole == "3") // Admin: full access
            {
                allowedModels = allModels;
            }
            else if (userRole == "2") // BQL: management allowed models (tune as needed)
            {
                allowedModels = new[] { "SinhVien", "PhanAnh", "HopDong", "HoaDon", "ThongBao", "Giuong", "Phong", "ToaNha" };
            }
            else if (userRole == "4") // KeToan: accountant - access to financial/contract related models
            {
                allowedModels = new[] { "HoaDon", "HopDong" };
            }
            else if (userRole == "1") // Sinh viên: limited models
            {
                allowedModels = new[] { "SinhVien", "PhanAnh", "HopDong", "HoaDon", "ThongBao" };
            }
            else
            {
                allowedModels = Array.Empty<string>();
            }

            // expose available models to view so it shows correct options
            ViewBag.availableModels = allowedModels;
            ViewBag.model = model;
            ViewBag.searchString = searchString;
            ViewBag.kieuSapXep = sortOrder;

            // Validate model selection
            if (string.IsNullOrWhiteSpace(model))
            {
                ViewData["ErrorMessage"] = "Vui lòng chọn model để tìm kiếm.";
                return View();
            }

            if (!allowedModels.Contains(model))
            {
                ViewData["ErrorMessage"] = "Bạn không có quyền truy cập model này hoặc model không hợp lệ.";
                return View();
            }

            // SinhVien
            if (model == "SinhVien" && allowedModels.Contains("SinhVien"))
            {
                var timKiem = _context.SinhViens.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaSv.Contains(searchString) || s.HoTen.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv),
                    "HoTen_asc" => timKiem.OrderBy(s => s.HoTen),
                    "HoTen_desc" => timKiem.OrderByDescending(s => s.HoTen),
                    _ => timKiem.OrderBy(s => s.MaSv),
                };

                var soluongSV = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongSV / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.totalPages = totalPages;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // PhanAnh
            if (model == "PhanAnh" && allowedModels.Contains("PhanAnh"))
            {
                var timKiem = _context.PhanAnhs.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaPhanAnh.Contains(searchString) || s.MaSv.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaPhanAnh_desc" => timKiem.OrderByDescending(s => s.MaPhanAnh),
                    "MaSv_asc" => timKiem.OrderBy(s => s.MaSv),
                    "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv),
                    _ => timKiem.OrderBy(s => s.MaPhanAnh),
                };

                var soluongPA = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongPA / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.totalPages = totalPages;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // HopDong
            if (model == "HopDong" && allowedModels.Contains("HopDong"))
            {
                var timKiem = _context.HopDongs.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaHopDong.Contains(searchString) || s.MaSv.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaHopDong_desc" => timKiem.OrderByDescending(s => s.MaHopDong),
                    "MaSv_asc" => timKiem.OrderBy(s => s.MaSv),
                    "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv),
                    _ => timKiem.OrderBy(s => s.MaHopDong),
                };

                var soluongHD = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongHD / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.totalPages = totalPages;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // HoaDon
            if (model == "HoaDon" && allowedModels.Contains("HoaDon"))
            {
                var timKiem = _context.HoaDons.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaHoaDon.Contains(searchString) || s.MaSv.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaHoaDon_desc" => timKiem.OrderByDescending(s => s.MaHoaDon),
                    "MaSv_asc" => timKiem.OrderBy(s => s.MaSv),
                    "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv),
                    _ => timKiem.OrderBy(s => s.MaHoaDon),
                };

                var soluongHD = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongHD / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // ThongBao
            if (model == "ThongBao" && allowedModels.Contains("ThongBao"))
            {
                var timKiem = _context.ThongBaos.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaThongBao.Contains(searchString) || s.NguoiGui.Contains(searchString) || s.NguoiNhan.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaThongBao_desc" => timKiem.OrderByDescending(s => s.MaThongBao),
                    "NguoiGui_asc" => timKiem.OrderBy(s => s.NguoiGui),
                    "NguoiGui_desc" => timKiem.OrderByDescending(s => s.NguoiGui),
                    "NguoiNhan_asc" => timKiem.OrderBy(s => s.NguoiNhan),
                    "NguoiNhan_desc" => timKiem.OrderByDescending(s => s.NguoiNhan),
                    _ => timKiem.OrderBy(s => s.MaThongBao),
                };

                var soluongTB = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongTB / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // TaiKhoan (role 3)
            if (model == "TaiKhoan" && allowedModels.Contains("TaiKhoan"))
            {
                var timKiem = _context.TaiKhoans.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaTaiKhoan.Contains(searchString) || s.TenDangNhap.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaTaiKhoan_desc" => timKiem.OrderByDescending(s => s.MaTaiKhoan),
                    "TenDangNhap_asc" => timKiem.OrderBy(s => s.TenDangNhap),
                    "TenDangNhap_desc" => timKiem.OrderByDescending(s => s.TenDangNhap),
                    _ => timKiem.OrderBy(s => s.MaTaiKhoan),
                };

                var soluongTK = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongTK / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // NhatKy (role 3)
            if (model == "NhatKy" && allowedModels.Contains("NhatKy"))
            {
                var timKiem = _context.NhatKies.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaLog.Contains(searchString) || s.NguoiThucHien.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaLog_desc" => timKiem.OrderByDescending(s => s.MaLog),
                    "NguoiThucHien_asc" => timKiem.OrderBy(s => s.NguoiThucHien),
                    "NguoiThucHien_desc" => timKiem.OrderByDescending(s => s.NguoiThucHien),
                    _ => timKiem.OrderBy(s => s.MaLog),
                };

                var soluongNK = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongNK / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // Giuong
            if (model == "Giuong" && allowedModels.Contains("Giuong"))
            {
                var timKiem = _context.Giuongs.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaGiuong.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaGiuong_desc" => timKiem.OrderByDescending(s => s.MaGiuong),
                    _ => timKiem.OrderBy(s => s.MaGiuong),
                };

                var soluongGiuong = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongGiuong / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // Phong
            if (model == "Phong" && allowedModels.Contains("Phong"))
            {
                var timKiem = _context.Phongs.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaPhong.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaPhong_desc" => timKiem.OrderByDescending(s => s.MaPhong),
                    _ => timKiem.OrderBy(s => s.MaPhong),
                };

                var soluongPhong = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongPhong / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // ToaNha
            if (model == "ToaNha" && allowedModels.Contains("ToaNha"))
            {
                var timKiem = _context.ToaNhas.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaToaNha.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaToaNha_desc" => timKiem.OrderByDescending(s => s.MaToaNha),
                    _ => timKiem.OrderBy(s => s.MaToaNha),
                };

                var soluongToaNha = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongToaNha / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // VaiTro (role 3)
            if (model == "VaiTro" && allowedModels.Contains("VaiTro"))
            {
                var timKiem = _context.VaiTros.AsQueryable();
                if (!string.IsNullOrWhiteSpace(searchString))
                    timKiem = timKiem.Where(s => s.MaVaiTro.Contains(searchString) || s.TenVaiTro.Contains(searchString));

                timKiem = sortOrder switch
                {
                    "MaVaiTro_desc" => timKiem.OrderByDescending(s => s.MaVaiTro),
                    _ => timKiem.OrderBy(s => s.MaVaiTro),
                };

                var soluongVaiTro = timKiem.Count();
                var totalPages = (int)Math.Ceiling(soluongVaiTro / (double)pageSize);

                var pagedData = timKiem
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.totalPages = totalPages;
                ViewBag.kieuSapXep = sortOrder;
                ViewBag.page = page;
                ViewBag.searchString = searchString;

                return View(pagedData);
            }

            // If we reach here, model is invalid or user lacks permission
            ViewData["ErrorMessage"] = "Model không hợp lệ hoặc bạn không có quyền truy cập.";
            return View();
        }
    }
}
