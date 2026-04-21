using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
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
            var csv = new System.Text.StringBuilder();
            csv.AppendLine("MaSv,HoTen,Lop,Email");
            csv.AppendLine("sv001,Nguyen Van A,20CT1,a@example.com");
            csv.AppendLine("sv002,Tran Thi B,20CT1,b@example.com");
            var bytes = System.Text.Encoding.UTF8.GetBytes(csv.ToString());
            return File(bytes, "text/csv", "sinh_vien_mau.csv");
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
            using (var stream = file.OpenReadStream())
            using (var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8))
            {
                var header = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(header))
                {
                    TempData["ErrorMessage"] = "File CSV rỗng hoặc định dạng không hợp lệ.";
                    return RedirectToAction(nameof(BulkImport));
                }

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    var maSv = parts.Length > 0 ? parts[0].Trim() : Guid.NewGuid().ToString();
                    var hoTen = parts.Length > 1 ? parts[1].Trim() : "";
                    var lop = parts.Length > 2 ? parts[2].Trim() : "";
                    var email = parts.Length > 3 ? parts[3].Trim() : "";

                    if (string.IsNullOrEmpty(maSv) || string.IsNullOrEmpty(hoTen))
                    {
                        errors.Add($"Dòng không hợp lệ: {line}");
                        continue;
                    }

                    if (_context.SinhViens.Any(s => s.MaSv == maSv))
                    {
                        errors.Add($"Mã sinh viên đã tồn tại: {maSv}");
                        continue;
                    }

                    // create account for student
                    var tk = new TaiKhoan
                    {
                        MaTaiKhoan = Guid.NewGuid().ToString(),
                        TenDangNhap = maSv,
                        Email = email,
                        VaiTro = "1",
                        TrangThai = "0"
                    };
                    // default password is student id (hashed)
                    var hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<TaiKhoan>();
                    tk.MatKhauMh = hasher.HashPassword(tk, maSv);
                    _context.TaiKhoans.Add(tk);

                    var sv = new SinhVien
                    {
                        MaSv = maSv,
                        HoTen = hoTen,
                        Lop = lop,
                        Email = email,
                        MaTaiKhoan = tk.MaTaiKhoan
                    };
                    _context.SinhViens.Add(sv);

                    // log creation
                    var log = new NhatKy
                    {
                        MaLog = Guid.NewGuid().ToString(),
                        NguoiThucHien = HttpContext.Session.GetString("userId"),
                        HanhDong = "BulkImportSinhVien",
                        DoiTuong = sv.MaSv,
                        GiaTriTruoc = null,
                        GiaTriSau = System.Text.Json.JsonSerializer.Serialize(sv),
                        ThoiGian = DateTime.Now
                    };
                    _context.NhatKies.Add(log);

                    processed++;
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
