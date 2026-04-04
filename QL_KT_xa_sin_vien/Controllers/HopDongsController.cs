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
    public class HopDongsController : Controller
    {
        private readonly QLSinhVienContext _context;

        public HopDongsController(QLSinhVienContext context)
        {
            _context = context;
        }
         
        // GET: HopDongs
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.HopDongs.Include(h => h.MaGiuongNavigation).Include(h => h.MaPhongNavigation).Include(h => h.MaSvNavigation);
            return View(await qLSinhVienContext.ToListAsync());
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
            return View();
        }

        // POST: HopDongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Create([Bind("MaHopDong,MaSv,MaPhong,MaGiuong,NgayBatDau,NgayKetThuc,TrangThai,DieuKhoan")] HopDong hopDong)
        {
            if (ModelState.IsValid)
            {
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
         
        public async Task<IActionResult> Edit(string id, [Bind("MaHopDong,MaSv,MaPhong,MaGiuong,NgayBatDau,NgayKetThuc,TrangThai,DieuKhoan")] HopDong hopDong)
        {
            if (id != hopDong.MaHopDong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
    }
}
