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
    [RoleAuthorize("2", "3")]
    public class GiuongsController : Controller
    {
        private readonly QLSinhVienContext _context;
        
        public GiuongsController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: Giuongs
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.Giuongs.Include(g => g.MaPhongNavigation).Include(g => g.OccupiedByNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: Giuongs/Details/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giuong = await _context.Giuongs
                .Include(g => g.MaPhongNavigation)
                .Include(g => g.OccupiedByNavigation)
                .FirstOrDefaultAsync(m => m.MaGiuong == id);
            if (giuong == null)
            {
                return NotFound();
            }

            return View(giuong);
        }

        // GET: Giuongs/Create
        [RoleAuthorize("2", "3")]
        public IActionResult Create()
        {
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong");
            return View();
        }

        // POST: Giuongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        
        public async Task<IActionResult> Create([Bind("MaGiuong,MaPhong,SoGiuong,OccupiedBy,TrangThai")] Giuong giuong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(giuong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", giuong.MaPhong);
            return View(giuong);
        }

        // GET: Giuongs/Edit/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giuong = await _context.Giuongs.FindAsync(id);
            if (giuong == null)
            {
                return NotFound();
            }
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", giuong.MaPhong);
            ViewData["OccupiedBy"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", giuong.OccupiedBy);
            return View(giuong);
        }

        // POST: Giuongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
        
        public async Task<IActionResult> Edit(string id, [Bind("MaGiuong,MaPhong,SoGiuong,OccupiedBy,TrangThai")] Giuong giuong)
        {
            if (id != giuong.MaGiuong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(giuong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!GiuongExists(giuong.MaGiuong))
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
            ViewData["MaPhong"] = new SelectList(_context.Phongs, "MaPhong", "MaPhong", giuong.MaPhong);
            ViewData["OccupiedBy"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", giuong.OccupiedBy);
            return View(giuong);
        }

        // GET: Giuongs/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var giuong = await _context.Giuongs
                .Include(g => g.MaPhongNavigation)
                .Include(g => g.OccupiedByNavigation)
                .FirstOrDefaultAsync(m => m.MaGiuong == id);
            if (giuong == null)
            {
                return NotFound();
            }

            return View(giuong);
        }

        // POST: Giuongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var giuong = await _context.Giuongs.FindAsync(id);
            if (giuong != null)
            {
                _context.Giuongs.Remove(giuong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GiuongExists(string id)
        {
            return _context.Giuongs.Any(e => e.MaGiuong == id);
        }
    }
}
