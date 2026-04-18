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
    [RoleAuthorize("3")]
    public class VaiTroesController : Controller
    {
        private readonly QLSinhVienContext _context;

        public VaiTroesController(QLSinhVienContext context)
        {
            _context = context;
        }
         
        // GET: VaiTroes
        // Instead of CRUD on roles, show account-role management: list accounts and their roles
        public async Task<IActionResult> Index()
        {
            var accounts = await _context.TaiKhoans.Include(t => t.VaiTroNavigation).ToListAsync();
            return View(accounts);
        }

        // GET: VaiTroes/Details/5
        // Show account details including role
        public async Task<IActionResult> Details(string id)
        {
            if (id == null) return NotFound();
            var account = await _context.TaiKhoans.Include(t => t.VaiTroNavigation).FirstOrDefaultAsync(t => t.MaTaiKhoan == id);
            if (account == null) return NotFound();
            // show account details with role info
            return View("DetailsAccount", account);
        }

        // GET: VaiTroes/EditRole/5
        // Edit role assigned to an account
        public async Task<IActionResult> EditRole(string id)
        {
            if (id == null) return NotFound();
            var account = await _context.TaiKhoans.FindAsync(id);
            if (account == null) return NotFound();
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro", account.VaiTro);
            return View(account);
        }

        // POST: VaiTroes/EditRole/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditRole(string id, string VaiTro)
        {
            if (id == null) return NotFound();
            var account = await _context.TaiKhoans.FindAsync(id);
            if (account == null) return NotFound();
            if (string.IsNullOrEmpty(VaiTro))
            {
                ModelState.AddModelError("VaiTro", "Vui lòng chọn vai trò.");
            }
            if (ModelState.IsValid)
            {
                account.VaiTro = VaiTro;
                _context.Update(account);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VaiTro"] = new SelectList(_context.VaiTros, "MaVaiTro", "TenVaiTro", account.VaiTro);
            return View(account);
        }
    }
}
