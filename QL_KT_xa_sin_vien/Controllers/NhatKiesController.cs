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
    [RoleAuthorize( "3")]
    public class NhatKiesController : Controller
    {
        private readonly QLSinhVienContext _context;

        public NhatKiesController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: NhatKies
        [RoleAuthorize("3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.NhatKies.Include(n => n.NguoiThucHienNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: NhatKies/Details/5
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhatKy = await _context.NhatKies
                .Include(n => n.NguoiThucHienNavigation)
                .FirstOrDefaultAsync(m => m.MaLog == id);
            if (nhatKy == null)
            {
                return NotFound();
            }

            return View(nhatKy);
        }

        // GET: NhatKies/Create
        [RoleAuthorize( "3")]
        public IActionResult Create()
        {
            ViewData["NguoiThucHien"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan");
            return View();
        }

        // POST: NhatKies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Create([Bind("MaLog,NguoiThucHien,HanhDong,DoiTuong,GiaTriTruoc,GiaTriSau,ThoiGian")] NhatKy nhatKy)
        {
            if (ModelState.IsValid)
            {
                _context.Add(nhatKy);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NguoiThucHien"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", nhatKy.NguoiThucHien);
            return View(nhatKy);
        }

        // GET: NhatKies/Edit/5
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhatKy = await _context.NhatKies.FindAsync(id);
            if (nhatKy == null)
            {
                return NotFound();
            }
            ViewData["NguoiThucHien"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", nhatKy.NguoiThucHien);
            return View(nhatKy);
        }

        // POST: NhatKies/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaLog,NguoiThucHien,HanhDong,DoiTuong,GiaTriTruoc,GiaTriSau,ThoiGian")] NhatKy nhatKy)
        {
            if (id != nhatKy.MaLog)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(nhatKy);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!NhatKyExists(nhatKy.MaLog))
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
            ViewData["NguoiThucHien"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", nhatKy.NguoiThucHien);
            return View(nhatKy);
        }

        // GET: NhatKies/Delete/5
        [RoleAuthorize( "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var nhatKy = await _context.NhatKies
                .Include(n => n.NguoiThucHienNavigation)
                .FirstOrDefaultAsync(m => m.MaLog == id);
            if (nhatKy == null)
            {
                return NotFound();
            }

            return View(nhatKy);
        }

        // POST: NhatKies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "3")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var nhatKy = await _context.NhatKies.FindAsync(id);
            if (nhatKy != null)
            {
                _context.NhatKies.Remove(nhatKy);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool NhatKyExists(string id)
        {
            return _context.NhatKies.Any(e => e.MaLog == id);
        }
    }
}
