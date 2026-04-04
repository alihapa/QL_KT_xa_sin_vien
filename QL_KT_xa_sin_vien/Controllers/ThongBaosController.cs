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
    public class ThongBaosController : Controller
    {
        private readonly QLSinhVienContext _context;

        public ThongBaosController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: ThongBaos
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.ThongBaos.Include(t => t.NguoiNhanNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: ThongBaos/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos
                .Include(t => t.NguoiNhanNavigation)
                .FirstOrDefaultAsync(m => m.MaThongBao == id);
            if (thongBao == null)
            {
                return NotFound();
            }

            return View(thongBao);
        }

        // GET: ThongBaos/Create
        [RoleAuthorize( "2", "3")]
        public IActionResult Create()
        {
            ViewData["NguoiNhan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan");
            return View();
        }

        // POST: ThongBaos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Create([Bind("MaThongBao,NguoiNhan,LoaiThongBao,NoiDung,TrangThai,ThoiGianGui")] ThongBao thongBao)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thongBao);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["NguoiNhan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", thongBao.NguoiNhan);
            return View(thongBao);
        }

        // GET: ThongBaos/Edit/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao == null)
            {
                return NotFound();
            }
            ViewData["NguoiNhan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", thongBao.NguoiNhan);
            return View(thongBao);
        }

        // POST: ThongBaos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaThongBao,NguoiNhan,LoaiThongBao,NoiDung,TrangThai,ThoiGianGui")] ThongBao thongBao)
        {
            if (id != thongBao.MaThongBao)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thongBao);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ThongBaoExists(thongBao.MaThongBao))
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
            ViewData["NguoiNhan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", thongBao.NguoiNhan);
            return View(thongBao);
        }

        // GET: ThongBaos/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var thongBao = await _context.ThongBaos
                .Include(t => t.NguoiNhanNavigation)
                .FirstOrDefaultAsync(m => m.MaThongBao == id);
            if (thongBao == null)
            {
                return NotFound();
            }

            return View(thongBao);
        }

        // POST: ThongBaos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var thongBao = await _context.ThongBaos.FindAsync(id);
            if (thongBao != null)
            {
                _context.ThongBaos.Remove(thongBao);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ThongBaoExists(string id)
        {
            return _context.ThongBaos.Any(e => e.MaThongBao == id);
        }
    }
}
