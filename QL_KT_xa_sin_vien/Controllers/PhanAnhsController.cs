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
    public class PhanAnhsController : Controller
    {
        private readonly QLSinhVienContext _context;

        public PhanAnhsController(QLSinhVienContext context)
        {
            _context = context;
        }
         

        // GET: PhanAnhs
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Index()
        {
            var phanAnhs = await _context.PhanAnhs.Include(p => p.MaPhongNavigation).Include(p => p.MaSvNavigation).Include(p => p.NguoiXuLyNavigation).ToListAsync();

            // build sender display map
            var senders = new Dictionary<string, string>();
            var currentRole = HttpContext.Session.GetString("userRole");
            var currentUserId = HttpContext.Session.GetString("userId");
            foreach (var p in phanAnhs)
            {
                string display = null;
                if (!string.IsNullOrEmpty(p.NguoiGoi))
                {
                    // try map account -> student name
                    var tk = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == p.NguoiGoi);
                    var sv = _context.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == p.NguoiGoi);
                    if (sv != null)
                    {
                        display = sv.HoTen;
                    }
                    else if (tk != null)
                    {
                        // if there is no linked SinhVien, show the account username
                        display = tk.TenDangNhap ?? tk.MaTaiKhoan;
                    }
                    else
                    {
                        display = p.NguoiGoi;
                    }

                    // apply anonymity: if AnDanh true and viewer is student (role 1) and not sender
                    if (p.AnDanh == true && currentRole == "1" && currentUserId != p.NguoiGoi)
                    {
                        display = "Ẩn danh";
                    }
                }
                else
                {
                    display = p.MaSvNavigation?.HoTen ?? p.MaSv;
                }
                senders[p.MaPhanAnh] = display;
            }

            ViewBag.SenderDisplay = senders;
            return View(phanAnhs);
        }

        // GET: PhanAnhs/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanAnh = await _context.PhanAnhs
                .Include(p => p.MaPhongNavigation)
                .Include(p => p.MaSvNavigation)
                .Include(p => p.NguoiXuLyNavigation)
                .FirstOrDefaultAsync(m => m.MaPhanAnh == id);
            if (phanAnh == null)
            {
                return NotFound();
            }

            // prepare sender display for details
            string senderDisplay = null;
            if (!string.IsNullOrEmpty(phanAnh.NguoiGoi))
            {
                var sv = _context.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == phanAnh.NguoiGoi);
                if (sv != null) senderDisplay = sv.HoTen;
                else
                {
                    var tk = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == phanAnh.NguoiGoi);
                    senderDisplay = tk != null ? (tk.TenDangNhap ?? tk.MaTaiKhoan) : phanAnh.NguoiGoi;
                }

                var currentRole = HttpContext.Session.GetString("userRole");
                var currentUserId = HttpContext.Session.GetString("userId");
                if (phanAnh.AnDanh == true && currentRole == "1" && currentUserId != phanAnh.NguoiGoi)
                {
                    senderDisplay = "Ẩn danh";
                }
            }
            else
            {
                senderDisplay = phanAnh.MaSvNavigation?.HoTen ?? phanAnh.MaSv;
            }
            ViewBag.SenderDisplay = senderDisplay;

            return View(phanAnh);
        }

        // GET: PhanAnhs/Create
        [RoleAuthorize("1", "2", "3")]
        public IActionResult Create()
        {
            var phanAnh = new PhanAnh
            {
                MaPhanAnh = Guid.NewGuid().ToString(),
                MucDoUuTien = "1",
                TrangThai = "đang xác minh",
                ThoiGianTao = DateTime.Now,
                ThoiGianCapNhat = DateTime.Now
            };
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong");
            // Always fix MaSv / sender for create: derive from current account (do not allow selecting)
            var dsSv = _context.SinhViens.Select(s => new { MaSv = s.MaSv, HoTen = s.HoTen + " (" + s.MaSv + ")" }).ToList();
            var currentUserId = HttpContext.Session.GetString("userId");
            ViewBag.IsMaSvFixed = true;
            if (!string.IsNullOrEmpty(currentUserId))
            {
                var sv = _context.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == currentUserId);
                if (sv != null)
                {
                    ViewBag.FixedMaSv = sv.MaSv;
                    ViewBag.FixedHoTen = sv.HoTen;
                }
                else
                {
                    // no linked student -> do not set MaSv (leave null) and show username for display
                    var tk = _context.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == currentUserId);
                    ViewBag.FixedMaSv = null;
                    ViewBag.FixedHoTen = tk != null ? (tk.TenDangNhap ?? tk.MaTaiKhoan) : currentUserId;
                }
            }
            else
            {
                // fallback: leave fixed fields empty
                ViewBag.FixedMaSv = string.Empty;
                ViewBag.FixedHoTen = "(không xác định)";
            }
            var dsNguoiXuLy = _context.TaiKhoans
                .Where(t => t.VaiTro == "2")
                .Join(_context.SinhViens,
                      tk => tk.MaTaiKhoan,
                      sv => sv.MaTaiKhoan,
                      (tk, sv) => new {
                          MaTaiKhoan = tk.MaTaiKhoan, // value
                          HoTen = sv.HoTen + " (" + tk.MaTaiKhoan + ")" 
                      })
                .ToList();

            ViewData["NguoiXuLy"] = new SelectList(dsNguoiXuLy, "MaTaiKhoan", "HoTen");
            
            return View(phanAnh);
        }

        // POST: PhanAnhs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Create([Bind("MaPhanAnh,MaSv,MaPhong,MucDoUuTien,TrangThai,MoTa,NguoiXuLy,ThoiGianTao,ThoiGianCapNhat,AnDanh")] PhanAnh phanAnh)
        {
            //phanAnh.MaPhanAnh = Guid.NewGuid().ToString();
            //phanAnh.MucDoUuTien = "1";
            //phanAnh.TrangThai = "đang xác minh";

            // set sender account from session
            var taiKhoanId = HttpContext.Session.GetString("userId");
            if (!string.IsNullOrEmpty(taiKhoanId))
            {
                phanAnh.NguoiGoi = taiKhoanId;
                // ensure MaSv is set from session mapping (if linked)
                var sv = _context.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == taiKhoanId);
                if (sv != null)
                {
                    phanAnh.MaSv = sv.MaSv;
                }
                else
                {
                    // if no linked SinhVien, do not set MaSv to avoid foreign key constraint
                    phanAnh.MaSv = null;
                }
            }

            if (ModelState.IsValid)
            {
                _context.Add(phanAnh);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }

            // Nếu ModelState không hợp lệ → log lỗi để kiểm tra
            var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
            foreach (var error in errors)
            {
                Console.WriteLine(error.ErrorMessage); // hoặc log ra file
            }

            // Load lại danh sách để hiển thị dropdown
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", phanAnh.MaPhong);
            var dsSv2 = _context.SinhViens.Select(s => new { MaSv = s.MaSv, HoTen = s.HoTen + " (" + s.MaSv + ")" }).ToList();
            ViewData["MaSv"] = new SelectList(dsSv2, "MaSv", "HoTen", phanAnh.MaSv);

            var dsNguoiXuLy = _context.TaiKhoans
                .Where(t => t.VaiTro == "2")
                .Join(_context.SinhViens,
                      tk => tk.MaTaiKhoan,
                      sv => sv.MaTaiKhoan,
                      (tk, sv) => new {
                          MaTaiKhoan = tk.MaTaiKhoan,
                          DisplayText = sv.HoTen + " (" + tk.MaTaiKhoan + ")"
                      })
                .ToList();

            ViewData["NguoiXuLy"] = new SelectList(dsNguoiXuLy, "MaTaiKhoan", "DisplayText", phanAnh.NguoiXuLy);

            return View(phanAnh);
        }

        // GET: PhanAnhs/Edit/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanAnh = await _context.PhanAnhs.FindAsync(id);
            if (phanAnh == null)
            {
                return NotFound();
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", phanAnh.MaPhong);
            var dsSv = _context.SinhViens.Select(s => new { MaSv = s.MaSv, HoTen = s.HoTen + " (" + s.MaSv + ")" }).ToList();
            ViewData["MaSv"] = new SelectList(dsSv, "MaSv", "HoTen", phanAnh.MaSv);
            var dsXuLy = _context.TaiKhoans.Where(t => t.VaiTro == "2").Join(_context.SinhViens, tk => tk.MaTaiKhoan, sv => sv.MaTaiKhoan, (tk, sv) => new { MaTaiKhoan = tk.MaTaiKhoan, DisplayText = sv.HoTen + " (" + tk.MaTaiKhoan + ")" }).ToList();
            ViewData["NguoiXuLy"] = new SelectList(dsXuLy, "MaTaiKhoan", "DisplayText", phanAnh.NguoiXuLy);
            return View(phanAnh);
        }

        // POST: PhanAnhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Edit(string id, [Bind("MaPhanAnh,MaSv,MaPhong,MoTa,MucDoUuTien,TrangThai,NguoiXuLy,ThoiGianTao,ThoiGianCapNhat,AnDanh")] PhanAnh phanAnh)
        {
            if (id != phanAnh.MaPhanAnh)
            {
                return NotFound();
            }

            // ensure NguoiGoi remains the account id of the session user
            var taiKhoanId = HttpContext.Session.GetString("userId");
            if (!string.IsNullOrEmpty(taiKhoanId)) phanAnh.NguoiGoi = taiKhoanId;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phanAnh);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhanAnhExists(phanAnh.MaPhanAnh))
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
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", phanAnh.MaPhong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", phanAnh.MaSv);
            ViewData["NguoiXuLy"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", phanAnh.NguoiXuLy);
            return View(phanAnh);
        }

        // GET: PhanAnhs/Delete/5
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phanAnh = await _context.PhanAnhs
                .Include(p => p.MaPhongNavigation)
                .Include(p => p.MaSvNavigation)
                .Include(p => p.NguoiXuLyNavigation)
                .FirstOrDefaultAsync(m => m.MaPhanAnh == id);
            if (phanAnh == null)
            {
                return NotFound();
            }

            return View(phanAnh);
        }

        // POST: PhanAnhs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phanAnh = await _context.PhanAnhs.FindAsync(id);
            if (phanAnh != null)
            {
                _context.PhanAnhs.Remove(phanAnh);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhanAnhExists(string id)
        {
            return _context.PhanAnhs.Any(e => e.MaPhanAnh == id);
        }
    }
}
