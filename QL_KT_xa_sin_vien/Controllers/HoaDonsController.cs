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
    [RoleAuthorize("1", "2", "3", "4")]
    public class HoaDonsController : Controller
    {
        private readonly QLSinhVienContext _context;

        public HoaDonsController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: HoaDons
        [RoleAuthorize("1", "2", "3", "4")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.HoaDons.Include(h => h.MaHopDongNavigation).Include(h => h.MaSvNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: HoaDons/Details/5
        [RoleAuthorize("1", "2", "3", "4")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaHopDongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // GET: HoaDons/Create
        [RoleAuthorize("2", "3")]
        public IActionResult Create()
        {
            ViewData["MaHopDong"] = new SelectList(_context.HopDongs, "MaHopDong", "MaHopDong");
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv");
            return View();
        }

        // POST: HoaDons/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Create([Bind("MaHoaDon,MaHopDong,MaSv,SoTien,TrangThai,NgayXuat")] HoaDon hoaDon)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoaDon);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaHopDong"] = new SelectList(_context.HopDongs, "MaHopDong", "MaHopDong", hoaDon.MaHopDong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hoaDon.MaSv);
            return View(hoaDon);
        }

        // GET: HoaDons/Edit/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon == null)
            {
                return NotFound();
            }
            ViewData["MaHopDong"] = new SelectList(_context.HopDongs, "MaHopDong", "MaHopDong", hoaDon.MaHopDong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hoaDon.MaSv);
            return View(hoaDon);
        }

        // POST: HoaDons/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaHoaDon,MaHopDong,MaSv,SoTien,TrangThai,NgayXuat")] HoaDon hoaDon)
        {
            if (id != hoaDon.MaHoaDon)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoaDon);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HoaDonExists(hoaDon.MaHoaDon))
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
            ViewData["MaHopDong"] = new SelectList(_context.HopDongs, "MaHopDong", "MaHopDong", hoaDon.MaHopDong);
            ViewData["MaSv"] = new SelectList(_context.SinhViens, "MaSv", "MaSv", hoaDon.MaSv);
            return View(hoaDon);
        }

        // GET: HoaDons/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hoaDon = await _context.HoaDons
                .Include(h => h.MaHopDongNavigation)
                .Include(h => h.MaSvNavigation)
                .FirstOrDefaultAsync(m => m.MaHoaDon == id);
            if (hoaDon == null)
            {
                return NotFound();
            }

            return View(hoaDon);
        }

        // POST: HoaDons/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var hoaDon = await _context.HoaDons.FindAsync(id);
            if (hoaDon != null)
            {
                _context.HoaDons.Remove(hoaDon);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HoaDonExists(string id)
        {
            return _context.HoaDons.Any(e => e.MaHoaDon == id);
        }
    }
}
