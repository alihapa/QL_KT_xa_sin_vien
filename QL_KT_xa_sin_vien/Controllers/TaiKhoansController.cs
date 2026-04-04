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
    public class TaiKhoansController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public TaiKhoansController(QLSinhVienContext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {
                _context.Add(taiKhoan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        [RoleAuthorize( "3")]
         
        public async Task<IActionResult> Edit(string id, [Bind("MaTaiKhoan,TenDangNhap,MatKhauMh,Email,Sdt,VaiTro,TrangThai")] TaiKhoan taiKhoan)
        {
            if (id != taiKhoan.MaTaiKhoan)
            {
                return NotFound();
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
