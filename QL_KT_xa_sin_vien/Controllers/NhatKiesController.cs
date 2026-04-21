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
        // Searchable index: supports optional date range filtering and sorts by ThoiGian desc
        [RoleAuthorize("3")]
        public async Task<IActionResult> Index(DateTime? from, DateTime? to, int page = 1, int pageSize = 50)
        {
            var q = _context.NhatKies.Include(n => n.NguoiThucHienNavigation).AsQueryable();

            if (from.HasValue)
            {
                q = q.Where(n => n.ThoiGian >= from.Value);
            }
            if (to.HasValue)
            {
                // include entire day for provided 'to' date by adding one day and comparing less than
                var toEnd = to.Value.Date.AddDays(1);
                q = q.Where(n => n.ThoiGian < toEnd);
            }

            // order by time desc
            q = q.OrderByDescending(n => n.ThoiGian);

            var total = await q.CountAsync();
            var totalPages = (int)Math.Ceiling(total / (double)pageSize);

            var items = await q.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

            ViewBag.from = from?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.to = to?.ToString("yyyy-MM-dd") ?? "";
            ViewBag.page = page;
            ViewBag.totalPages = totalPages;
            ViewBag.pageSize = pageSize;

            return View(items);
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

        // Note: Create/Edit actions intentionally removed — logs should be recorded automatically and not manually created/edited.

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

        // Export logs as plain text within optional date range
        [RoleAuthorize("3")]
        public async Task<IActionResult> Export(DateTime? from, DateTime? to)
        {
            var q = _context.NhatKies.AsQueryable();
            if (from.HasValue) q = q.Where(n => n.ThoiGian >= from.Value);
            if (to.HasValue) q = q.Where(n => n.ThoiGian < to.Value.Date.AddDays(1));
            q = q.OrderBy(n => n.ThoiGian);
            var list = await q.ToListAsync();

            var sb = new System.Text.StringBuilder();
            foreach (var l in list)
            {
                sb.AppendLine($"[{l.ThoiGian:yyyy-MM-dd HH:mm:ss}] {l.NguoiThucHien} {l.HanhDong} -> {l.DoiTuong} | Trước: {l.GiaTriTruoc} | Sau: {l.GiaTriSau}");
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"nhatky_{DateTime.Now:yyyyMMddHHmmss}.txt";
            return File(bytes, "text/plain", fileName);
        }
    }
}
