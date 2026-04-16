using Microsoft.AspNetCore.Mvc;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]
    public class TimKiemsController : Controller
    {
        private readonly QLSinhVienContext _context;
        public TimKiemsController(QLSinhVienContext context)
        {
            _context = context;
        }
        public IActionResult TimKiem(string searchString, string model, string sortOrder, int page = 1, int pageSize = 5)
        {
           
            // lấy thông tin theo model
            if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(searchString))
            {
                if (model == "SinhVien" && HttpContext.Session.GetString("userRole") == "1") 
                { 
                    var timKiem = _context.SinhViens.Where(s => s.MaSv.Contains(searchString) || s.HoTen.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv).ToList(),
                        "HoTen_asc" => timKiem.OrderBy(s => s.HoTen).ToList(),
                        "HoTen_desc" => timKiem.OrderByDescending(s => s.HoTen).ToList(),
                        _ => timKiem.OrderBy(s => s.MaSv).ToList(),
                    };
                    //phan trang
                    var soluongSV = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongSV / (double)pageSize);

                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.totalPages = totalPages;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "PhanAnh" && HttpContext.Session.GetString("userRole") == "1") 
                { var timKiem = _context.PhanAnhs.Where(s => s.MaPhanAnh.Contains(searchString) || s.MaSv.Contains(searchString)).ToList();
                    //sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaPhanAnh_desc" => timKiem.OrderByDescending(s => s.MaPhanAnh).ToList(),
                        "MaSv_asc" => timKiem.OrderBy(s => s.MaSv).ToList(),
                        "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv).ToList(),
                        _ => timKiem.OrderBy(s => s.MaPhanAnh).ToList(),
                    };
                    //phan trang
                    var soluongPA = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongPA / (double)pageSize);

                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.totalPages = totalPages;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "HopDong" && HttpContext.Session.GetString("userRole") == "1") 
                { var timKiem = _context.HopDongs.Where(s => s.MaHopDong.Contains(searchString) || s.MaSv.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaHopDong_desc" => timKiem.OrderByDescending(s => s.MaHopDong).ToList(),
                        "MaSv_asc" => timKiem.OrderBy(s => s.MaSv).ToList(),
                        "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv).ToList(),
                        _ => timKiem.OrderBy(s => s.MaHopDong).ToList(),
                    };
                    //phan trang
                    var soluongHD = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongHD / (double)pageSize);

                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page; 
                    ViewBag.totalPages = totalPages;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "HoaDon" && HttpContext.Session.GetString("userRole") == "1") 
                { var timKiem = _context.HoaDons.Where(s => s.MaHoaDon.Contains(searchString) || s.MaSv.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaHoaDon_desc" => timKiem.OrderByDescending(s => s.MaHoaDon).ToList(),
                        "MaSv_asc" => timKiem.OrderBy(s => s.MaSv).ToList(),
                        "MaSv_desc" => timKiem.OrderByDescending(s => s.MaSv).ToList(),
                        _ => timKiem.OrderBy(s => s.MaHoaDon).ToList(),
                    };
                    //phan trang
                    var soluongHD = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongHD / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "ThongBao" && HttpContext.Session.GetString("userRole") == "1") 
                { var timKiem = _context.ThongBaos.Where(s => s.MaThongBao.Contains(searchString) || s.NguoiGui.Contains(searchString) || s.NguoiNhan.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaThongBao_desc" => timKiem.OrderByDescending(s => s.MaThongBao).ToList(),
                        "NguoiGui_asc" => timKiem.OrderBy(s => s.NguoiGui).ToList(),
                        "NguoiGui_desc" => timKiem.OrderByDescending(s => s.NguoiGui).ToList(),
                        "NguoiNhan_asc" => timKiem.OrderBy(s => s.NguoiNhan).ToList(),
                        "NguoiNhan_desc" => timKiem.OrderByDescending(s => s.NguoiNhan).ToList(),
                        _ => timKiem.OrderBy(s => s.MaThongBao).ToList(),
                    };
                    //phan trang
                    var soluongTB = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongTB / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "TaiKhoan" && HttpContext.Session.GetString("userRole") == "3") 
                { var timKiem = _context.TaiKhoans.Where(s => s.MaTaiKhoan.Contains(searchString) || s.TenDangNhap.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaTaiKhoan_desc" => timKiem.OrderByDescending(s => s.MaTaiKhoan).ToList(),
                        "TenDangNhap_asc" => timKiem.OrderBy(s => s.TenDangNhap).ToList(),
                        "TenDangNhap_desc" => timKiem.OrderByDescending(s => s.TenDangNhap).ToList(),
                        _ => timKiem.OrderBy(s => s.MaTaiKhoan).ToList(),
                    };
                    //phan trang
                    var soluongTK = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongTK / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "NhatKy" && HttpContext.Session.GetString("userRole") == "3") 
                { var timKiem = _context.NhatKies.Where(s => s.MaLog.Contains(searchString) || s.NguoiThucHien.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaLog_desc" => timKiem.OrderByDescending(s => s.MaLog).ToList(),
                        "NguoiThucHien_asc" => timKiem.OrderBy(s => s.NguoiThucHien).ToList(),
                        "NguoiThucHien_desc" => timKiem.OrderByDescending(s => s.NguoiThucHien).ToList(),
                        _ => timKiem.OrderBy(s => s.MaLog).ToList(),
                    };
                    //phan trang
                    var soluongNK = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongNK / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "Giuong") 
                { var timKiem = _context.Giuongs.Where(s => s.MaGiuong.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaGiuong_desc" => timKiem.OrderByDescending(s => s.MaGiuong).ToList(),
                        _ => timKiem.OrderBy(s => s.MaGiuong).ToList(),
                    };
                    //phan trang
                    var soluongGiuong = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongGiuong / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "Phong") 
                { var timKiem = _context.Phongs.Where(s => s.MaPhong.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaPhong_desc" => timKiem.OrderByDescending(s => s.MaPhong).ToList(),
                        _ => timKiem.OrderBy(s => s.MaPhong).ToList(),
                    };
                    //phan trang
                    var soluongPhong = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongPhong / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "ToaNha") 
                { var timKiem = _context.ToaNhas.Where(s => s.MaToaNha.Contains(searchString)).ToList();
                    //sắp xếp theo sortOrder
                    timKiem = sortOrder switch
                    {
                        "MaToaNha_desc" => timKiem.OrderByDescending(s => s.MaToaNha).ToList(),
                        _ => timKiem.OrderBy(s => s.MaToaNha).ToList(),
                    };
                    //phan trang
                    var soluongToaNha = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongToaNha / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else if (model == "VaiTro" && HttpContext.Session.GetString("userRole") == "3") 
                { var timKiem = _context.VaiTros.Where(s => s.MaVaiTro.Contains(searchString) || s.TenVaiTro.Contains(searchString)).ToList();
                    // sắp xếp theo sortOrder
                    timKiem = sortOrder switch {
                        "MaVaiTro_desc" => timKiem.OrderByDescending(s => s.MaVaiTro).ToList(),
                        _ => timKiem.OrderBy(s => s.MaVaiTro).ToList(),
                    };
                    //phan trang
                    var soluongVaiTro = timKiem.Count();
                    var totalPages = (int)Math.Ceiling(soluongVaiTro / (double)pageSize);
                    var pagedData = timKiem
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();
                    // truyền dữ liệu ra view
                    ViewBag.totalPages = totalPages;
                    ViewBag.kieuSapXep = sortOrder;
                    ViewBag.page = page;
                    ViewBag.searchString = searchString;

                    return View(pagedData);
                }
                else
                {
                    ViewData["ErrorMessage"] = "Model không hợp lệ hoặc bạn không có quyền truy cập.";
                }

            }
            else
            {
                ViewData["ErrorMessage"] = "Vui lòng nhập chuỗi tìm kiếm và chọn model.";
            }
            return View();
        }
    }
}
