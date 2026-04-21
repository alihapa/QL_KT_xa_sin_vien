using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]

    public class PhongsController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public PhongsController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: Phongs
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.Phongs.Include(p => p.MaToaNhaNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }
        // GET: DangKy
        [HttpGet]
        public async Task<IActionResult> DangKy(string id, string? selectedToaNha = null, string? selectedPhong = null)
        {
            var dk = new DangKyPhong
            {
                MaSv = id,
                HoTen = "",
                ToaNhaList = await _context.ToaNhas.Select(t => t.MaToaNha).ToListAsync(),
                PhongList = new List<string>(), // sẽ load theo tòa nhà nếu có
                GiuongList = new List<string>() // sẽ load theo phòng nếu có
            };

            if (!string.IsNullOrEmpty(selectedToaNha))
            {
                dk.SelectedToaNha = selectedToaNha;
                dk.PhongList = await _context.Phongs
                    .Where(p => p.MaToaNha == selectedToaNha)
                    .Select(p => p.MaPhong)
                    .ToListAsync();
            }

            if (!string.IsNullOrEmpty(selectedPhong))
            {
                dk.SelectedPhong = selectedPhong;
                dk.GiuongList = await _context.Giuongs
                    .Where(g => g.MaPhong == selectedPhong)
                    .Select(g => g.MaGiuong)
                    .ToListAsync();
            }

            return View(dk);
        }

        // Return rooms for a building (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetPhongs(string toaNha)
        {
            if (string.IsNullOrEmpty(toaNha)) return Json(new List<string>());
            var phongs = await _context.Phongs
                .Where(p => p.MaToaNha == toaNha)
                .Select(p => p.MaPhong)
                .ToListAsync();
            return Json(phongs);
        }

        // Return beds for a room (AJAX)
        [HttpGet]
        public async Task<JsonResult> GetGiuongs(string phong)
        {
            if (string.IsNullOrEmpty(phong)) return Json(new List<string>());
            var giuongs = await _context.Giuongs
                .Where(g => g.MaPhong == phong)
                .Select(g => g.MaGiuong)
                .ToListAsync();
            return Json(giuongs);
        }

        // POST: DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyPhong dk)
        {
            if (!ModelState.IsValid)
            {
                var dk1 = new DangKyPhong
                {
                    MaSv = dk.MaSv,
                    HoTen = dk.HoTen,

                    LyDo = dk.LyDo,

                    ToaNhaList = await _context.ToaNhas.Select(t => t.MaToaNha).ToListAsync(),
                    PhongList = new List<string>(), // sẽ load theo tòa nhà
                    GiuongList = new List<string>() // sẽ load theo phòng
                };
                return View(dk1);
            }

            // Ensure we have a SinhVien record to reference in HopDong.MaSv
            var accountId = HttpContext.Session.GetString("userId");
            // prefer explicit MaSv from form, otherwise try to find by accountId
            SinhVien? sv = null;
            if (!string.IsNullOrEmpty(dk.MaSv))
            {
                sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaSv == dk.MaSv);
            }
            if (sv == null && !string.IsNullOrEmpty(accountId))
            {
                sv = await _context.SinhViens.FirstOrDefaultAsync(s => s.MaTaiKhoan == accountId || s.MaSv == accountId);
            }
            // if still null, create a minimal SinhVien using accountId as MaSv (so FK will succeed)
            if (sv == null)
            {
                var newMaSv = !string.IsNullOrEmpty(dk.MaSv) ? dk.MaSv : (accountId ?? Guid.NewGuid().ToString());
                sv = new SinhVien
                {
                    MaSv = newMaSv,
                    HoTen = dk.HoTen ?? "chưa có tên",
                    Email = null,
                    MaTaiKhoan = accountId
                };
                _context.SinhViens.Add(sv);
                await _context.SaveChangesAsync();
            }

            // check if student already has active or pending contract
            var existing = _context.HopDongs.Any(h => h.MaSv == sv.MaSv && (h.TrangThai == "1" || h.TrangThai == "0"));
            if (existing)
            {
                TempData["ErrorMessage"] = "Bạn đã có hợp đồng đang hoạt động hoặc chờ xét duyệt. Không thể đăng ký thêm.";
                return RedirectToAction("Index");
            }

            // check if bed is already occupied
            if (!string.IsNullOrEmpty(dk.SelectedGiuong))
            {
                var bed = await _context.Giuongs.FindAsync(dk.SelectedGiuong);
                if (bed != null && (!string.IsNullOrEmpty(bed.OccupiedBy) || (bed.TrangThai != null && bed.TrangThai.Contains("chiếm", StringComparison.OrdinalIgnoreCase))))
                {
                    TempData["ErrorMessage"] = "Giường đã bị chiếm. Vui lòng chọn giường khác.";
                    return RedirectToAction("Index");
                }
            }

            // Handle uploaded PDF (if any) and save to wwwroot/uploads
            string? pdfPath = null;
            if (dk.DieuKhoanPdfFile != null && dk.DieuKhoanPdfFile.Length > 0)
            {
                var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploads)) Directory.CreateDirectory(uploads);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(dk.DieuKhoanPdfFile.FileName);
                var filePath = Path.Combine(uploads, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await dk.DieuKhoanPdfFile.CopyToAsync(stream);
                }
                pdfPath = "/uploads/" + fileName;
            }

            // Tạo hợp đồng mới nhưng ở trạng thái chờ duyệt
            var hopDong = new HopDong
            {
                MaHopDong = Guid.NewGuid().ToString(),
                MaSv = sv.MaSv,
                MaPhong = dk.SelectedPhong,
                MaGiuong = dk.SelectedGiuong,
                // Ly do và thời gian sẽ được gán theo yêu cầu
                NgayBatDau = dk.NgayBatDau ?? DateOnly.FromDateTime(DateTime.Now),
                NgayKetThuc = dk.NgayKetThuc ?? DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                // use status codes: "0" = chờ xét duyệt
                TrangThai = "0",
                DieuKhoan = string.IsNullOrEmpty(dk.LyDo) ? "Theo quy định ký túc xá" : dk.LyDo,
                Agree = dk.Agree,
                DieuKhoanPdf = pdfPath
            };

            _context.HopDongs.Add(hopDong);
            // tạo thông báo cho tất cả BQL (vai trò 2)
            var bqls = _context.TaiKhoans.Where(t => t.VaiTro == "2").ToList();
            var sender = HttpContext.Session.GetString("userId") ?? "System";
            foreach (var b in bqls)
            {
                var tb = new ThongBao
                {
                    MaThongBao = Guid.NewGuid().ToString(),
                    NguoiGui = sender,
                    NguoiNhan = b.MaTaiKhoan,
                    LoaiThongBao = "DuyetDangKy",
                    NoiDung = $"Yêu cầu đăng ký phòng: HopDong={hopDong.MaHopDong}, Phong={hopDong.MaPhong}, Giuong={hopDong.MaGiuong}, Người đăng ký={hopDong.MaSv}",
                    TrangThai = "Chưa đọc",
                    ThoiGianGui = DateTime.Now
                };
                _context.ThongBaos.Add(tb);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Yêu cầu đăng ký đã được gửi tới BQL. Vui lòng chờ xét duyệt.";
            return RedirectToAction("Index");
        }

        // GET: Phongs/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            var phongs = new Phong();
            if (id == null)
            {
                phongs.MaPhong = "";
                phongs.MaToaNha = "";
                phongs.Tang = 0;
                phongs.LoaiPhong = "";
                phongs.SucChua = 0;
                phongs.SoLuongDangO = 0;
                phongs.GioiTinh = "";
                phongs.TrangThai = "";
            }

            var phong = await _context.Phongs
                .Include(p => p.MaToaNhaNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);
            if (phong == null)
            {
                phong = phongs;
            }

            return View(phong);
        }

        // GET: Phongs/Create
        [RoleAuthorize( "2", "3")]
        public IActionResult Create()
        {
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha");
            return View();
        }

        // POST: Phongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Create([Bind("MaPhong,ToaNha,Tang,LoaiPhong,SucChua,SoLuongDangO,GioiTinh,TrangThai")] Phong phong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // GET: Phongs/Edit/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
            {
                return RedirectToAction(nameof(Index));
            }
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // POST: Phongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
         
        public async Task<IActionResult> Edit(string id, [Bind("MaPhong,ToaNha,Tang,LoaiPhong,SucChua,SoLuongDangO,GioiTinh,TrangThai")] Phong phong)
        {
            if (id != phong.MaPhong)
            {
                return RedirectToAction(nameof(Index));
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.MaPhong))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // GET: Phongs/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return RedirectToAction(nameof(Index));
            }

            var phong = await _context.Phongs
                .Include(p => p.MaToaNhaNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);
            if (phong == null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(phong);
        }

        // POST: Phongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongExists(string id)
        {
            return _context.Phongs.Any(e => e.MaPhong == id);
        }
    }
}
