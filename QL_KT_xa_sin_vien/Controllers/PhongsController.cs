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

    public class PhongsController : Controller
    {
        private readonly QLSinhVienContext _context;
         
        public PhongsController(QLSinhVienContext context)
        {
            _context = context;
        }

        // GET: Phongs
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Index()
        {
            var qLSinhVienContext = _context.Phongs.Include(p => p.MaToaNhaNavigation);
            return View(await qLSinhVienContext.ToListAsync());
        }
        // GET: DangKy
        [HttpGet]
        public async Task<IActionResult> DangKy(string id)
        {
            var dk = new DangKyPhong
            {
                MaSv = id,
                HoTen = "",

                ToaNhaList = await _context.ToaNhas.Select(t => t.MaToaNha).ToListAsync(),
                PhongList = new List<string>(), // sẽ load theo tòa nhà
                GiuongList = new List<string>() // sẽ load theo phòng
            };

            return View(dk);
        }

        // POST: DangKy
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyPhong dk)
        {
            if (!ModelState.IsValid)
            {
                var dk1 = new DangKyPhong
                {
                    MaSv = dk.MaSv,
                    HoTen = dk.HoTen,

                    ToaNhaList = await _context.ToaNhas.Select(t => t.MaToaNha).ToListAsync(),
                    PhongList = new List<string>(), // sẽ load theo tòa nhà
                    GiuongList = new List<string>() // sẽ load theo phòng
                };
                return View(dk1);
            }

            // Tạo hợp đồng mới
            var hopDong = new HopDong
            {
                MaHopDong = Guid.NewGuid().ToString(),
                MaSv = dk.MaSv,
                MaPhong = dk.SelectedPhong,
                MaGiuong = dk.SelectedGiuong,
                NgayBatDau = DateOnly.FromDateTime(DateTime.Now),
                NgayKetThuc = DateOnly.FromDateTime(DateTime.Now.AddMonths(6)),
                TrangThai = "Đang sử dụng",
                DieuKhoan = "Theo quy định ký túc xá"
            };

            _context.HopDongs.Add(hopDong);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký phòng thành công!";
            return RedirectToAction("Index");
        }

        // GET: Phongs/Details/5
        [RoleAuthorize("1", "2", "3")]
        public async Task<IActionResult> Details(string id)
        {
            var phongs = new Phong();
            if (id == null)
            {
                phongs.MaPhong = "";
                phongs.MaToaNha = "";
                phongs.Tang = 0;
                phongs.LoaiPhong = "";
                phongs.SucChua = 0;
                phongs.SoLuongDangO = 0;
                phongs.GioiTinh = "";
                phongs.TrangThai = "";
            }

            var phong = await _context.Phongs
                .Include(p => p.MaToaNhaNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);
            if (phong == null)
            {
                phong = phongs;
            }

            return View(phong);
        }

        // GET: Phongs/Create
        [RoleAuthorize( "2", "3")]
        public IActionResult Create()
        {
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha");
            return View();
        }

        // POST: Phongs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> Create([Bind("MaPhong,ToaNha,Tang,LoaiPhong,SucChua,SoLuongDangO,GioiTinh,TrangThai")] Phong phong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(phong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // GET: Phongs/Edit/5
        [RoleAuthorize("2", "3")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs.FindAsync(id);
            if (phong == null)
            {
                return NotFound();
            }
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // POST: Phongs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RoleAuthorize("2", "3")]
         
        public async Task<IActionResult> Edit(string id, [Bind("MaPhong,ToaNha,Tang,LoaiPhong,SucChua,SoLuongDangO,GioiTinh,TrangThai")] Phong phong)
        {
            if (id != phong.MaPhong)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(phong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PhongExists(phong.MaPhong))
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
            ViewData["ToaNha"] = new SelectList(_context.ToaNhas, "MaToaNha", "MaToaNha", phong.MaToaNha);
            return View(phong);
        }

        // GET: Phongs/Delete/5
        [RoleAuthorize( "2", "3")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var phong = await _context.Phongs
                .Include(p => p.MaToaNhaNavigation)
                .FirstOrDefaultAsync(m => m.MaPhong == id);
            if (phong == null)
            {
                return NotFound();
            }

            return View(phong);
        }

        // POST: Phongs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [RoleAuthorize( "2", "3")]
         
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var phong = await _context.Phongs.FindAsync(id);
            if (phong != null)
            {
                _context.Phongs.Remove(phong);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PhongExists(string id)
        {
            return _context.Phongs.Any(e => e.MaPhong == id);
        }
    }
}
