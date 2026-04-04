using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien.Controllers
{
    [RoleAuthorize("1", "2", "3")]
    public class SinhViensController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public SinhViensController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: SinhViens
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Index()
        { 
            var qLSinhVienContext = _context.SinhViens.Include(s => s.MaTaiKhoanNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }

        // GET: SinhViens/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}

            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // GET: SinhViens/Create
        [RoleAuthorize("3")]
        public IActionResult Create()
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan");
            return View();
        }

        // POST: SinhViens/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("3")]
        //ghi nhật ký
         
        public async Task<IActionResult> Create([Bind("MaSv,HoTen,Lop,Khoa,SoCmnd,Email,MaTaiKhoan")] SinhVien sinhVien)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}

            //var oldSv = await _context.SinhViens.FindAsync(id);
            //if (oldSv == null) return NotFound();

            //var giaTriTruoc = JsonSerializer.Serialize(oldSv);

            if (ModelState.IsValid)
            {
                _context.Add(sinhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            //await LogService.GhiNhatKy(HttpContext.Session.GetString("userId"), "Them", "SinhVien", null, sinhVien);
            return View(sinhVien);
        }

        // GET: SinhViens/Edit/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            var oldSv = await _context.SinhViens.FindAsync(id);
            if (oldSv == null) return NotFound();
            var giaTriTruoc = JsonSerializer.Serialize(oldSv);
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien == null)
            {
                return NotFound();
            }
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            return View(sinhVien);
        }

        // POST: SinhViens/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Edit(string id, [Bind("MaSv,HoTen,Lop,Khoa,SoCmnd,Email,MaTaiKhoan")] SinhVien sinhVien)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            if (id != sinhVien.MaSv)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sinhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SinhVienExists(sinhVien.MaSv))
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
            ViewData["MaTaiKhoan"] = new SelectList(_context.TaiKhoans, "MaTaiKhoan", "MaTaiKhoan", sinhVien.MaTaiKhoan);
            return View(sinhVien);
        }

        // GET: SinhViens/Delete/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            if (id == null)
            {
                return NotFound();
            }

            var sinhVien = await _context.SinhViens
                .Include(s => s.MaTaiKhoanNavigation)
                .FirstOrDefaultAsync(m => m.MaSv == id);
            if (sinhVien == null)
            {
                return NotFound();
            }

            return View(sinhVien);
        }

        // POST: SinhViens/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("1", "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //if (string.IsNullOrEmpty(HttpContext.Session.GetString("users")))
            //{
            //    // Nếu chưa đăng nhập, chuyển hướng đến trang đăng nhập
            //    return RedirectToAction("DangNhap");
            //}
            var sinhVien = await _context.SinhViens.FindAsync(id);
            if (sinhVien != null)
            {
                _context.SinhViens.Remove(sinhVien);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SinhVienExists(string id)
        {
            return _context.SinhViens.Any(e => e.MaSv == id);
        }
    }
}
