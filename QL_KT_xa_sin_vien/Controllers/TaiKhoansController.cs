using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]
    public class TaiKhoansController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public TaiKhoansController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: TaiKhoans/BulkEdit
        [RoleAuthorize("3")]
        public IActionResult BulkEdit()
        {
            return View();
        }

        // GET: TaiKhoans/DownloadSample (xlsx)
        [RoleAuthorize("3")]
        public IActionResult DownloadSample()
        {
            OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new OfficeOpenXml.ExcelPackage();
            var ws = package.Workbook.Worksheets.Add("Mau");
            ws.Cells[1, 1].Value = "MaTaiKhoan";
            ws.Cells[1, 2].Value = "TenDangNhap";
            ws.Cells[1, 3].Value = "TrangThai";
            ws.Cells[1, 4].Value = "ThoiHan (yyyy-MM-dd)";
            ws.Cells[1, 5].Value = "Delete (1 to delete)";
            ws.Cells[2, 2].Value = "sv001";
            ws.Cells[2, 3].Value = "0";
            ws.Cells[2, 4].Value = "2026-12-31";
            ws.Cells[3, 2].Value = "sv002";
            ws.Cells[3, 3].Value = "1";
            ws.Cells[4, 2].Value = "sv003";
            ws.Cells[4, 5].Value = "1";

            var bytes = package.GetAsByteArray();
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "tai_khoan_mau.xlsx");
        }

        // POST: TaiKhoans/BulkEdit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        public async Task<IActionResult> BulkEdit(Microsoft.AspNetCore.Http.IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file để tải lên.";
                return RedirectToAction(nameof(BulkEdit));
            }

            var processed = 0;
            var errors = new List<string>();

            if (file.FileName.EndsWith(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                OfficeOpenXml.ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
                using var stream = file.OpenReadStream();
                using var package = new OfficeOpenXml.ExcelPackage(stream);
                var ws = package.Workbook.Worksheets.FirstOrDefault();
                if (ws == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy sheet trong file Excel.";
                    return RedirectToAction(nameof(BulkEdit));
                }

                var row = 2;
                while (true)
                {
                    var ten = ws.Cells[row, 2].GetValue<string>()?.Trim();
                    if (string.IsNullOrEmpty(ten)) break;
                    var ma = ws.Cells[row, 1].GetValue<string>()?.Trim();
                    var trangThai = ws.Cells[row, 3].GetValue<string>()?.Trim();
                    var thoiHanStr = ws.Cells[row, 4].GetValue<string>()?.Trim();
                    var del = ws.Cells[row, 5].GetValue<string>()?.Trim();

                    TaiKhoan tk = null;
                    if (!string.IsNullOrEmpty(ma)) tk = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == ma);
                    if (tk == null && !string.IsNullOrEmpty(ten)) tk = _context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == ten);
                    if (tk == null)
                    {
                        errors.Add($"Không tìm thấy tài khoản: Ma='{ma}' TenDangNhap='{ten}' (row {row})");
                        row++;
                        continue;
                    }

                    var before = tk.TrangThai;
                    try
                    {
                        if (del == "1" || del?.Equals("true", StringComparison.OrdinalIgnoreCase) == true)
                        {
                            _context.TaiKhoans.Remove(tk);
                            _context.NhatKies.Add(new NhatKy { MaLog = Guid.NewGuid().ToString(), NguoiThucHien = HttpContext.Session.GetString("userId"), HanhDong = "BulkDelete", DoiTuong = tk.TenDangNhap ?? tk.MaTaiKhoan, GiaTriTruoc = before, GiaTriSau = "(deleted)", ThoiGian = DateTime.Now });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(trangThai))
                            {
                                var allowed = new[] { "1", "0", "-1" };
                                if (!allowed.Contains(trangThai))
                                {
                                    errors.Add($"Giá trị trạng thái không hợp lệ cho {tk.TenDangNhap} (row {row}): '{trangThai}'");
                                }
                                else
                                {
                                    tk.TrangThai = trangThai;
                                }
                            }

                            if (!string.IsNullOrEmpty(thoiHanStr) && DateTime.TryParse(thoiHanStr, out var th))
                            {
                                tk.ThoiHan = th;
                            }

                            _context.TaiKhoans.Update(tk);
                            _context.NhatKies.Add(new NhatKy { MaLog = Guid.NewGuid().ToString(), NguoiThucHien = HttpContext.Session.GetString("userId"), HanhDong = "BulkUpdateStatusOrExpiry", DoiTuong = tk.TenDangNhap ?? tk.MaTaiKhoan, GiaTriTruoc = before, GiaTriSau = tk.TrangThai + "|" + (tk.ThoiHan?.ToString("o") ?? ""), ThoiGian = DateTime.Now });
                        }
                        processed++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi xử lý tài khoản {tk.TenDangNhap} (row {row}): {ex.Message}");
                    }

                    row++;
                }
            }
            else
            {
                // CSV
                using var stream = file.OpenReadStream();
                using var reader = new System.IO.StreamReader(stream, System.Text.Encoding.UTF8);
                var header = await reader.ReadLineAsync();
                if (string.IsNullOrWhiteSpace(header))
                {
                    TempData["ErrorMessage"] = "File CSV rỗng hoặc định dạng không hợp lệ.";
                    return RedirectToAction(nameof(BulkEdit));
                }

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();
                    if (string.IsNullOrWhiteSpace(line)) continue;
                    var parts = line.Split(',');
                    var ma = parts.Length > 0 ? parts[0].Trim() : string.Empty;
                    var ten = parts.Length > 1 ? parts[1].Trim() : string.Empty;
                    var trangThai = parts.Length > 2 ? parts[2].Trim() : string.Empty;
                    var thoiHan = parts.Length > 3 ? parts[3].Trim() : string.Empty;
                    var del = parts.Length > 4 ? parts[4].Trim() : string.Empty;

                    TaiKhoan tk = null;
                    if (!string.IsNullOrEmpty(ma)) tk = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == ma);
                    if (tk == null && !string.IsNullOrEmpty(ten)) tk = _context.TaiKhoans.FirstOrDefault(t => t.TenDangNhap == ten);
                    if (tk == null)
                    {
                        errors.Add($"Không tìm thấy tài khoản: Ma='{ma}' TenDangNhap='{ten}'");
                        continue;
                    }

                    var before = tk.TrangThai;
                    try
                    {
                        if (del == "1" || del.Equals("true", StringComparison.OrdinalIgnoreCase))
                        {
                            _context.TaiKhoans.Remove(tk);
                            _context.NhatKies.Add(new NhatKy { MaLog = Guid.NewGuid().ToString(), NguoiThucHien = HttpContext.Session.GetString("userId"), HanhDong = "BulkDelete", DoiTuong = tk.TenDangNhap ?? tk.MaTaiKhoan, GiaTriTruoc = before, GiaTriSau = "(deleted)", ThoiGian = DateTime.Now });
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(trangThai))
                            {
                                var allowed = new[] { "1", "0", "-1" };
                                if (!allowed.Contains(trangThai))
                                {
                                    errors.Add($"Giá trị trạng thái không hợp lệ cho {tk.TenDangNhap}: '{trangThai}'");
                                }
                                else
                                {
                                    tk.TrangThai = trangThai;
                                }
                            }

                            if (!string.IsNullOrEmpty(thoiHan) && DateTime.TryParse(thoiHan, out var th)) tk.ThoiHan = th;
                            _context.TaiKhoans.Update(tk);
                            _context.NhatKies.Add(new NhatKy { MaLog = Guid.NewGuid().ToString(), NguoiThucHien = HttpContext.Session.GetString("userId"), HanhDong = "BulkUpdateStatusOrExpiry", DoiTuong = tk.TenDangNhap ?? tk.MaTaiKhoan, GiaTriTruoc = before, GiaTriSau = tk.TrangThai + "|" + (tk.ThoiHan?.ToString("o") ?? ""), ThoiGian = DateTime.Now });
                        }
                        processed++;
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Lỗi xử lý tài khoản {tk.TenDangNhap}: {ex.Message}");
                    }
                }
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "Lỗi khi lưu thay đổi: " + ex.Message;
                return RedirectToAction(nameof(BulkEdit));
            }

            TempData["SuccessMessage"] = $"Đã xử lý {processed} dòng." + (errors.Any() ? " Có lỗi: " + string.Join("; ", errors.Take(5)) : string.Empty);
            return RedirectToAction(nameof(Index));
        }

        // GET: TaiKhoans
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.TaiKhoans.Include(t => t.VaiTroNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: TaiKhoans/Details/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.VaiTroNavigation)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // GET: TaiKhoans/Create
        [RoleAuthorize( "3")]
        public IActionResult Create()
        {
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "MaVaiTro");
            return View();
        }

        // POST: TaiKhoans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        public async Task<IActionResult> Create([Bind("MaTaiKhoan,TenDangNhap,MatKhauMh,Email,Sdt,VaiTro,TrangThai")] TaiKhoan taiKhoan)
        {
            // ensure primary key
            if (string.IsNullOrEmpty(taiKhoan.MaTaiKhoan))
            {
                taiKhoan.MaTaiKhoan = Guid.NewGuid().ToString();
            }

            // Hash password before saving
            if (!string.IsNullOrEmpty(taiKhoan.MatKhauMh))
            {
                var hasher = new PasswordHasher<TaiKhoan>();
                taiKhoan.MatKhauMh = hasher.HashPassword(taiKhoan, taiKhoan.MatKhauMh);
            }

            // default status to "0" (Chưa kích hoạt) when creating via admin unless explicitly set to active
            if (string.IsNullOrEmpty(taiKhoan.TrangThai))
            {
                taiKhoan.TrangThai = "0";
            }

            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException ex)
                {
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    if (msg != null && (msg.Contains("UQ_TaiKhoan_tenDangNhap") || msg.Contains("duplicate key") || msg.Contains("tenDangNhap")))
                    {
                        ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại. Vui lòng chọn tên khác.");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lỗi khi lưu dữ liệu. Vui lòng thử lại hoặc liên hệ quản trị viên.");
                    }
                }
            }
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "MaVaiTro", taiKhoan.VaiTro);
            return View(taiKhoan);
        }

        // GET: TaiKhoans/Edit/5
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null)
            {
                return NotFound();
            }
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "MaVaiTro", taiKhoan.VaiTro);
            return View(taiKhoan);
        }

        // POST: TaiKhoans/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        public async Task<IActionResult> Edit(string id, Microsoft.AspNetCore.Http.IFormCollection form)
        {
            // Bind form values manually to avoid exposing hashed password back to the view
            var tenDangNhap = form["TenDangNhap"].ToString();
            var newPassword = form["MatKhauMh"].ToString(); // may be empty if no change
            var email = form["Email"].ToString();
            var sdt = form["Sdt"].ToString();
            var vaiTro = form["VaiTro"].ToString();
            var trangThai = form["TrangThai"].ToString();

            if (id == null) return NotFound();

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan == null) return NotFound();

            // update fields
            taiKhoan.TenDangNhap = tenDangNhap;
            taiKhoan.Email = email;
            taiKhoan.Sdt = sdt;
            taiKhoan.VaiTro = vaiTro;
            // Validate status values: only allow "1", "0" or "-1"
            var allowedStatuses = new[] { "1", "0", "-1" };
            if (allowedStatuses.Contains(trangThai))
            {
                taiKhoan.TrangThai = trangThai;
            }
            else
            {
                ModelState.AddModelError("TrangThai", "Giá trị trạng thái không hợp lệ.");
            }

            // if new password provided, hash and update
            if (!string.IsNullOrEmpty(newPassword))
            {
                var hasher = new PasswordHasher<TaiKhoan>();
                taiKhoan.MatKhauMh = hasher.HashPassword(taiKhoan, newPassword);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taiKhoan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaiKhoanExists(taiKhoan.MaTaiKhoan))
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

            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "MaVaiTro", taiKhoan.VaiTro);
            return View(taiKhoan);
        }

        // GET: TaiKhoans/Delete/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taiKhoan = await _context.TaiKhoans
                .Include(t => t.VaiTroNavigation)
                .FirstOrDefaultAsync(m => m.MaTaiKhoan == id);
            if (taiKhoan == null)
            {
                return NotFound();
            }

            return View(taiKhoan);
        }

        // POST: TaiKhoans/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                _context.TaiKhoans.Remove(taiKhoan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaiKhoanExists(string id)
        {
            return _context.TaiKhoans.Any(e => e.MaTaiKhoan == id);
        }
    }
}
