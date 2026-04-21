using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]
    public class SinhViensController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public SinhViensController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: SinhViens/BulkImport
        [RoleAuthorize("3")]
        public IActionResult BulkImport()
        {
            return View();
        }

        // GET: SinhViens/DownloadSample
        [RoleAuthorize("3")]
        public IActionResult DownloadSample()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Mau");
            ws.Cells[1, 1].Value = "MaSv";
            ws.Cells[1, 2].Value = "HoTen";
            ws.Cells[1, 3].Value = "Lop";
            ws.Cells[1, 4].Value = "Khoa";
            ws.Cells[1, 5].Value = "Email";

            ws.Cells[2, 1].Value = "sv001";
            ws.Cells[2, 2].Value = "Nguyen Van A";
            ws.Cells[2, 3].Value = "20CT1";
            ws.Cells[2, 4].Value = "Computer Science";
            ws.Cells[2, 5].Value = "a@example.com";

            ws.Cells[3, 1].Value = "sv002";
            ws.Cells[3, 2].Value = "Tran Thi B";
            ws.Cells[3, 3].Value = "20CT1";
            ws.Cells[3, 4].Value = "Information Technology";
            ws.Cells[3, 5].Value = "b@example.com";

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "sinh_vien_mau.xlsx");
        }

        // POST: SinhViens/BulkImport
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        public async Task<IActionResult> BulkImport(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file CSV.";
                return RedirectToAction(nameof(BulkImport));
            }

            var processed = 0;
            var errors = new List<string>();

            if (!file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                TempData["ErrorMessage"] = "Vui lòng tải lên file Excel (.xlsx).";
                return RedirectToAction(nameof(BulkImport));
            }

            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using (var stream = file.OpenReadStream())
            using (var package = new OfficeOpenXml.ExcelPackage(stream))
            {
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sheet trong file Excel.";
                    return RedirectToAction(nameof(BulkImport));
                }

                var row = 2;
                while (true)
                {
                    var maSv = ws.Cells[row, 1].GetValue<string>()?.Trim();
                    var hoTen = ws.Cells[row, 2].GetValue<string>()?.Trim();
                    var lop = ws.Cells[row, 3].GetValue<string>()?.Trim();
                    var khoa = ws.Cells[row, 4].GetValue<string>()?.Trim();
                    var email = ws.Cells[row, 5].GetValue<string>()?.Trim();

                    // stop when no MaSv and no HoTen
                    if (string.IsNullOrEmpty(maSv) && string.IsNullOrEmpty(hoTen)) break;

                    if (string.IsNullOrEmpty(maSv)) maSv = Guid.NewGuid().ToString();

                    if (string.IsNullOrEmpty(maSv) || string.IsNullOrEmpty(hoTen))
                    {
                        errors.Add($"Dòng {row} không hợp lệ: MaSv hoặc HoTen bị thiếu.");
                        row++;
                        continue;
                    }

                    if (_context.SinhViens.Any(s => s.MaSv == maSv))
                    {
                        errors.Add($"Mã sinh viên đã tồn tại: {maSv} (dòng {row})");
                        row++;
                        continue;
                    }

                    // create account for student
                    string baseUsername = null;
                    if (!string.IsNullOrEmpty(email) && email.Contains("@"))
                    {
                        baseUsername = email.Split('@')[0].Trim();
                    }
                    if (string.IsNullOrEmpty(baseUsername)) baseUsername = maSv;

                    var username = baseUsername;
                    var suffix = 1;
                    while (_context.TaiKhoans.Any(t => t.TenDangNhap == username))
                    {
                        username = baseUsername + suffix.ToString();
                        suffix++;
                    }

                    var tk = new TaiKhoan
                    {
                        MaTaiKhoan = Guid.NewGuid().ToString(),
                        TenDangNhap = username,
                        Email = email,
                        VaiTro = "1",
                        TrangThai = "0"
                    };
                    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<TaiKhoan>();
                    tk.MatKhauMh = hasher.HashPassword(tk, maSv);
                    _context.TaiKhoans.Add(tk);

                    var sv = new SinhVien
                    {
                        MaSv = maSv,
                        HoTen = hoTen,
                        Lop = lop,
                        Khoa = khoa,
                        Email = email,
                        MaTaiKhoan = tk.MaTaiKhoan
                    };
                    _context.SinhViens.Add(sv);

                    var log = new NhatKy
                    {
                        MaLog = Guid.NewGuid().ToString(),
                        NguoiThucHien = HttpContext.Session.GetString("userId"),
                        HanhDong = "BulkImportSinhVien",
                        DoiTuong = sv.MaSv,
                        GiaTriTruoc = null,
                        // Use ReferenceHandler.Preserve to avoid possible object cycles when serializing EF entities
                        GiaTriSau = System.Text.Json.JsonSerializer.Serialize(sv, new JsonSerializerOptions
                        {
                            ReferenceHandler = ReferenceHandler.Preserve,
                            MaxDepth = 128
                        }),
                        ThoiGian = DateTime.Now
                    };
                    _context.NhatKies.Add(log);

                    processed++;
                    row++;
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu dữ liệu: " + ex.Message;
                return RedirectToAction(nameof(BulkImport));
            }

            TempData["SuccessMessage"] = $"Đã xử lý {processed} sinh viên." + (errors.Any() ? " Có lỗi: " + string.Join("; ", errors.Take(5)) : string.Empty);
            return RedirectToAction(nameof(Index));
        }

        // GET: SinhViens
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Index()
        { 
            var qLSinhVienContext = _context.SinhViens.Include(s => s.MaTaiKhoanNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: SinhViens/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}

            if (id == null)
            {
                return View();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
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

            // If role 1 (student) ensure they can only view their own profile
            var role = HttpContext.Session.GetString("userRole");
            if (role == "1")
            {
                var taiKhoanId = HttpContext.Session.GetString("userId");
                var currentSv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == taiKhoanId);
                if (currentSv == null || currentSv.MaSv != id)
                {
                    TempData["ErrorMessage"] = "Không có quyền xem thông tin này.";
                    return RedirectToAction("Index", "Home");
                }
            }

            return View(sinhVien);
        }

        // GET: SinhViens/Create
        [RoleAuthorize("3")]
        public IActionResult Create()
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan");
            return View();
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        //ghi nhật ký
         
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,Lop,Khoa,SoCmnd,Email,MaTaiKhoan")] SinhVien sinhVien)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}

            //var oldSv = await _context.SinhViens.FindAsync(id);
            //if (oldSv == null) return NotFound();

            //var giaTriTruoc = JsonSerializer.Serialize(oldSv);

            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            //await LogService.GhiNhatKy(HttpContext.Session.GetString("userId"), "Them", "SinhVien", null, sinhVien);
            return View(sinhVien);
        }

        // GET: SinhViens/Edit/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            var oldSv = await _context.SinhViens.FindAsync(id);
            if (oldSv == null) return NotFound();
            var giaTriTruoc = JsonSerializer.Serialize(oldSv);
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            return View(sinhVien);
        }

        // POST: SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,HoTen,Lop,Khoa,SoCmnd,Email,MaTaiKhoan")] SinhVien sinhVien)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            if (id != sinhVien.MaSv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            return View(sinhVien);
        }

        // GET: SinhViens/Delete/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                // Remove or update related entities to avoid FK constraint errors

                // 1) Delete HoaDons related to this student
                var hoaDons = _context.HoaDons.Where(h => h.MaSv == id).ToList();
                if (hoaDons.Any())
                {
                    _context.HoaDons.RemoveRange(hoaDons);
                }

                // 2) Delete HopDongs that reference this student
                var hopDongs = _context.HopDongs.Where(h => h.MaSv == id).ToList();
                if (hopDongs.Any())
                {
                    _context.HopDongs.RemoveRange(hopDongs);
                }

                // 3) Delete PhanAnhs submitted by this student
                var phanAnhs = _context.PhanAnhs.Where(p => p.MaSv == id).ToList();
                if (phanAnhs.Any())
                {
                    _context.PhanAnhs.RemoveRange(phanAnhs);
                }

                // 4) Clear any Giuong references that occupy this student
                var occupiedBeds = _context.Giuongs.Where(g => g.OccupiedBy == id).ToList();
                if (occupiedBeds.Any())
                {
                    foreach (var bed in occupiedBeds)
                    {
                        bed.OccupiedBy = null;
                    }
                    _context.Giuongs.UpdateRange(occupiedBeds);
                }

                // Finally remove the student
                _context.SinhViens.Remove(sinhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}
