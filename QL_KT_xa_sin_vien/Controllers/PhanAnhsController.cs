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
    public class PhanAnhsController : Controller
    {
        private readonly QLSinhVienContext _context;

        public PhanAnhsController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: PhanAnhs
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.PhanAnhs.Include(p => p.MaPhongNavigation).Include(p => p.MaSvNavigation).Include(p => p.NguoiXuLyNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: PhanAnhs/Details/5
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

            return View(phanAnh);
        }

        // GET: PhanAnhs/Create
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
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv");
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
        public async Task<IActionResult> Create([Bind("MaPhanAnh,MaSv,MaPhong,MucDoUuTien,TrangThai,MoTa,NguoiXuLy,ThoiGianTao,ThoiGianCapNhat")] PhanAnh phanAnh)
        {
            //phanAnh.MaPhanAnh = Guid.NewGuid().ToString();
            //phanAnh.MucDoUuTien = "1";
            //phanAnh.TrangThai = "đang xác minh";

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
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "HoTen", phanAnh.MaSv);

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
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", phanAnh.MaSv);
            ViewData["NguoiXuLy"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", phanAnh.NguoiXuLy);
            return View(phanAnh);
        }

        // POST: PhanAnhs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("MaPhanAnh,MaSv,MaPhong,MoTa,MucDoUuTien,TrangThai,NguoiXuLy,ThoiGianTao,ThoiGianCapNhat")] PhanAnh phanAnh)
        {
            if (id != phanAnh.MaPhanAnh)
            {
                return NotFound();
            }

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
