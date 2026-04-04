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
    [RoleAuthorize( "2", "3")]
    public class ToaNhasController : Controller
    {
        private readonly QLSinhVienContext _context;

        public ToaNhasController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: ToaNhas
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.ToaNhas.ToListAsync());
        }

        // GET: ToaNhas/Details/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toaNha = await _context.ToaNhas
                .FirstOrDefaultAsync(m => m.MaToaNha == id);
            if (toaNha == null)
            {
                return NotFound();
            }

            return View(toaNha);
        }

        // GET: ToaNhas/Create
        [RoleAuthorize("2", "3")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: ToaNhas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Create([Bind("MaToaNha,TenToaNha,DiaChi")] ToaNha toaNha)
        {
            if (ModelState.IsValid)
            {
                _context.Add(toaNha);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(toaNha);
        }

        // GET: ToaNhas/Edit/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toaNha = await _context.ToaNhas.FindAsync(id);
            if (toaNha == null)
            {
                return NotFound();
            }
            return View(toaNha);
        }

        // POST: ToaNhas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaToaNha,TenToaNha,DiaChi")] ToaNha toaNha)
        {
            if (id != toaNha.MaToaNha)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(toaNha);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ToaNhaExists(toaNha.MaToaNha))
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
            return View(toaNha);
        }

        // GET: ToaNhas/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var toaNha = await _context.ToaNhas
                .FirstOrDefaultAsync(m => m.MaToaNha == id);
            if (toaNha == null)
            {
                return NotFound();
            }

            return View(toaNha);
        }

        // POST: ToaNhas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var toaNha = await _context.ToaNhas.FindAsync(id);
            if (toaNha != null)
            {
                _context.ToaNhas.Remove(toaNha);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ToaNhaExists(string id)
        {
            return _context.ToaNhas.Any(e => e.MaToaNha == id);
        }
    }
}
