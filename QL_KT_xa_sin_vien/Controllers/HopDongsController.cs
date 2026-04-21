using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;
using System.IO;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]
    public class HopDongsController : Controller
    {
        private readonly QLSinhVienContext _context;
        private readonly IConfiguration _configuration;

        public HopDongsController(QLSinhVienContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
         
        // GET: HopDongs
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Index(string? filter = null)
        {
            // return all contracts; filtering by status removed because status is managed separately
            var qLSinhVienContext = _context.HopDongs.Include(h => h.MaGiuongNavigation).Include(h => h.MaPhongNavigation).Include(h => h.MaSvNavigation).AsQueryable();
            return View(await qLSinhVienContext.ToListAsync());
        }

        // Approve/Change status of a contract (BQL)
        [RoleAuthorize("2", "3")]
        [HttpPost]
        public async Task<IActionResult> ChangeStatus(string id, string newStatus, string? reason)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null) return NotFound();
            // validate status code
            if (!(newStatus == "1" || newStatus == "0" || newStatus == "-1" || newStatus == "2"))
            {
                return BadRequest();
            }
            var prevStatus = (string?)null; // previous status not tracked on model
            // if approved, mark bed occupied
            if (newStatus == "1" && !string.IsNullOrEmpty(hopDong.MaGiuong))
            {
                var giu = await _context.Giuongs.FindAsync(hopDong.MaGiuong);
                if (giu != null)
                {
                    giu.OccupiedBy = hopDong.MaSv;
                    giu.TrangThai = "Đã chiếm";
                    _context.Giuongs.Update(giu);

                    // also increment room occupancy
                    if (!string.IsNullOrEmpty(giu.MaPhong))
                    {
                        var phong = await _context.Phongs.FindAsync(giu.MaPhong);
                        if (phong != null)
                        {
                            phong.SoLuongDangO = (phong.SoLuongDangO ?? 0) + 1;
                            // if room reached capacity, optionally mark as full
                            if (phong.SucChua.HasValue && phong.SoLuongDangO >= phong.SucChua)
                            {
                                phong.TrangThai = "Đầy";
                            }
                            _context.Phongs.Update(phong);
                        }
                    }
                }
            }

            // if changing from approved to other statuses (rejected/expired), free the bed and decrement room occupancy
            if ((newStatus == "2" || newStatus == "-1") && !string.IsNullOrEmpty(hopDong.MaGiuong))
            {
                var giu = await _context.Giuongs.FindAsync(hopDong.MaGiuong);
                if (giu != null)
                {
                    // only free if currently occupied by this student
                    if (!string.IsNullOrEmpty(giu.OccupiedBy) && giu.OccupiedBy == hopDong.MaSv)
                    {
                        giu.OccupiedBy = null;
                        giu.TrangThai = "Trống";
                        _context.Giuongs.Update(giu);

                        if (!string.IsNullOrEmpty(giu.MaPhong))
                        {
                            var phong = await _context.Phongs.FindAsync(giu.MaPhong);
                            if (phong != null)
                            {
                                phong.SoLuongDangO = Math.Max(0, (phong.SoLuongDangO ?? 0) - 1);
                                // if room now has space, clear full status
                                if (phong.SucChua.HasValue && phong.SoLuongDangO < phong.SucChua)
                                {
                                    phong.TrangThai = "Có chỗ";
                                }
                                _context.Phongs.Update(phong);
                            }
                        }
                    }
                }
            }

            // reason is only used for notification/logging; not stored on HopDong model

            // notify student about status change
            string? studentAccountId = null;
            if (!string.IsNullOrEmpty(hopDong.MaSv))
            {
                studentAccountId = await _context.SinhViens
                    .Where(s => s.MaSv == hopDong.MaSv)
                    .Select(s => s.MaTaiKhoan)
                    .FirstOrDefaultAsync();
            }

            var tb = new ThongBao
            {
                MaThongBao = Guid.NewGuid().ToString(),
                NguoiGui = HttpContext.Session.GetString("userId") ?? "BQL",
                // set recipient to student's account id (FK to TaiKhoan). If not available, leave null.
                NguoiNhan = string.IsNullOrEmpty(studentAccountId) ? null : studentAccountId,
                LoaiThongBao = "TrangThaiHopDong",
                NoiDung = $"Trạng thái đơn đăng ký (HĐ {hopDong.MaHopDong}) đã được cập nhật. Mã trạng thái: {newStatus}." + (string.IsNullOrEmpty(reason) ? "" : $" Lý do: {reason}"),
                TrangThai = "Chưa đọc",
                ThoiGianGui = DateTime.Now
            };
            _context.ThongBaos.Add(tb);

            // log into NhatKy
            var nk = new NhatKy
            {
                MaLog = Guid.NewGuid().ToString(),
                NguoiThucHien = HttpContext.Session.GetString("userId"),
                HanhDong = "Thay doi trang thai hop dong",
                DoiTuong = hopDong.MaHopDong,
                GiaTriTruoc = prevStatus,
                GiaTriSau = newStatus,
                ThoiGian = DateTime.Now
            };
            _context.NhatKies.Add(nk);

            // Note: HopDong model no longer stores TrangThai; we keep other changes if any
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: HopDongs/UploadDieuKhoan
        // Show upload form for BQL/Admin
        [RoleAuthorize("2", "3")]
        [HttpGet]
        public IActionResult UploadDieuKhoan()
        {
            return View();
        }

        // POST: HopDongs/UploadDieuKhoan
        // Saves a PDF and creates a DieuKhoan record; only BQL/Admin can upload
        [RoleAuthorize("2", "3")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDieuKhoan(Microsoft.AspNetCore.Http.IFormFile? file)
        {
            if (file == null || file.Length == 0) return RedirectToAction(nameof(Index));

            var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploads, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var relative = "/uploads/" + fileName;

            try
            {
                // create DieuKhoan record
                var dk = new DieuKhoan
                {
                    MaDieuKhoan = Guid.NewGuid().ToString(),
                    FilePath = relative,
                    OriginalFileName = file.FileName,
                    UploadedBy = HttpContext.Session.GetString("userId"),
                    UploadedAt = DateTime.Now
                };
                _context.DieuKhoans.Add(dk);

                // create notification for all accounts
                var allAccounts = _context.TaiKhoans.Select(t => t.MaTaiKhoan).ToList();
                foreach (var acc in allAccounts)
                {
                    var tb = new ThongBao
                    {
                        MaThongBao = Guid.NewGuid().ToString(),
                        NguoiGui = HttpContext.Session.GetString("userId") ?? "BQL",
                        NguoiNhan = acc,
                        LoaiThongBao = "DieuKhoanMoi",
                        NoiDung = $"Có file điều khoản mới được tải lên: {relative}",
                        TrangThai = "Chưa đọc",
                        ThoiGianGui = DateTime.Now
                    };
                    _context.ThongBaos.Add(tb);
                }

                // log upload in NhatKy
                var nk = new NhatKy
                {
                    MaLog = Guid.NewGuid().ToString(),
                    NguoiThucHien = HttpContext.Session.GetString("userId"),
                    HanhDong = "Upload dieu khoan",
                    DoiTuong = dk.MaDieuKhoan,
                    GiaTriSau = relative,
                    ThoiGian = DateTime.Now
                };
                _context.NhatKies.Add(nk);

                await _context.SaveChangesAsync();
            }
            catch
            {
                // ignore failures
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: HopDongs/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.MaGiuongNavigation)
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHopDong == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // GET: HopDongs/Create
        [RoleAuthorize("2", "3")]
        public IActionResult Create()
        {
            ViewData["MaGiuong"] = new SelectList(_context.Giuongs, "MaGiuong", "MaGiuong");
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv");
            // provide latest uploaded DieuKhoan (terms) for viewing in the Create view
            var latestDieuKhoan = _context.DieuKhoans.OrderByDescending(d => d.UploadedAt).FirstOrDefault();
            ViewBag.LatestDieuKhoan = latestDieuKhoan?.FilePath;
            ViewBag.LatestDieuKhoanName = latestDieuKhoan?.OriginalFileName;
            return View();
        }

        // POST: HopDongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Create([Bind("MaHopDong,MaSv,MaPhong,MaGiuong,NgayBatDau,NgayKetThuc,Agree,DieuKhoanPdf")] HopDong hopDong, Microsoft.AspNetCore.Http.IFormFile? DieuKhoanPdfFile)
        {
            if (ModelState.IsValid)
            {
                // handle upload
                if (DieuKhoanPdfFile != null && DieuKhoanPdfFile.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                    if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DieuKhoanPdfFile.FileName);
                    var filePath = Path.Combine(uploads, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await DieuKhoanPdfFile.CopyToAsync(stream);
                    }
                    hopDong.DieuKhoanPdf = "/uploads/" + fileName;
                }

                _context.Add(hopDong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaGiuong"] = new SelectList(_context.Giuongs, "MaGiuong", "MaGiuong", hopDong.MaGiuong);
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hopDong.MaSv);
            return View(hopDong);
        }

        // GET: HopDongs/Edit/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong == null)
            {
                return NotFound();
            }
            ViewData["MaGiuong"] = new SelectList(_context.Giuongs, "MaGiuong", "MaGiuong", hopDong.MaGiuong);
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hopDong.MaSv);
            return View(hopDong);
        }

        // POST: HopDongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Edit(string id, [Bind("MaHopDong,MaSv,MaPhong,MaGiuong,NgayBatDau,NgayKetThuc,Agree,DieuKhoanPdf")] HopDong hopDong, Microsoft.AspNetCore.Http.IFormFile? DieuKhoanPdfFile)
        {
            if (id != hopDong.MaHopDong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // handle uploaded pdf
                    if (DieuKhoanPdfFile != null && DieuKhoanPdfFile.Length > 0)
                    {
                        var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                        if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                        var fileName = Guid.NewGuid().ToString() + Path.GetExtension(DieuKhoanPdfFile.FileName);
                        var filePath = Path.Combine(uploads, fileName);
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await DieuKhoanPdfFile.CopyToAsync(stream);
                        }
                        hopDong.DieuKhoanPdf = "/uploads/" + fileName;
                    }

                    _context.Update(hopDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HopDongExists(hopDong.MaHopDong))
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
            ViewData["MaGiuong"] = new SelectList(_context.Giuongs, "MaGiuong", "MaGiuong", hopDong.MaGiuong);
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", hopDong.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hopDong.MaSv);
            return View(hopDong);
        }

        // GET: HopDongs/Delete/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hopDong = await _context.HopDongs
                .Include(h => h.MaGiuongNavigation)
                .Include(h => h.MaPhongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHopDong == id);
            if (hopDong == null)
            {
                return NotFound();
            }

            return View(hopDong);
        }

        // POST: HopDongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hopDong = await _context.HopDongs.FindAsync(id);
            if (hopDong != null)
            {
                _context.HopDongs.Remove(hopDong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HopDongExists(string id)
        {
            return _context.HopDongs.Any(e => e.MaHopDong == id);
        }

        // Scheduled: create notifications for contracts expiring within 1 week
        // This can be called from a background job or manually by an admin.
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> NotifyExpiring()
        {
            var soon = DateTime.Now.Date.AddDays(7);
            var targets = _context.HopDongs.Where(h => h.NgayKetThuc.HasValue && h.NgayKetThuc.Value.ToDateTime(TimeOnly.MinValue) <= soon);
            foreach (var h in targets)
            {
                var studentId = h.MaSv;
                var exists = _context.ThongBaos.Any(t => t.LoaiThongBao == "HopDongSapHet" && t.NguoiNhan == studentId && t.NoiDung.Contains(h.MaHopDong));
                if (exists) continue;
                var tb = new ThongBao
                {
                    MaThongBao = Guid.NewGuid().ToString(),
                    NguoiGui = "System",
                    NguoiNhan = studentId,
                    LoaiThongBao = "HopDongSapHet",
                    NoiDung = $"Hợp đồng {h.MaHopDong} của bạn sẽ hết hạn vào {h.NgayKetThuc}",
                    TrangThai = "Chưa đọc",
                    ThoiGianGui = DateTime.Now
                };
                _context.ThongBaos.Add(tb);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
