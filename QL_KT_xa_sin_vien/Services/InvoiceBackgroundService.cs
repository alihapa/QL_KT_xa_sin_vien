using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using QL_KT_xa_sin_vien.Models;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Services
{
    public class InvoiceBackgroundService : BackgroundService
    {
        private readonly ILogger<InvoiceBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly TimeSpan _delay = TimeSpan.FromDays(30);

        public InvoiceBackgroundService(ILogger<InvoiceBackgroundService> logger, IServiceScopeFactory scopeFactory)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("InvoiceBackgroundService started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await GenerateInvoicesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error generating invoices");
                }

                try { await Task.Delay(_delay, stoppingToken); } catch { }
            }
        }

        private async Task GenerateInvoicesAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QLSinhVienContext>();

            // pick latest DonGia
            var dg = await db.DonGias.OrderByDescending(d => d.NgayHieuLuc).FirstOrDefaultAsync(cancellationToken);
            if (dg == null) return;

            // for each active contract, create monthly HoaDon based on formula: room + dien + nuoc
            var active = await db.HopDongs.Where(h => h.TrangThai == "1").ToListAsync(cancellationToken);
            foreach (var h in active)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var sv = await db.SinhViens.FirstOrDefaultAsync(s => s.MaSv == h.MaSv, cancellationToken);
                if (sv == null) continue;

                var dienUsage = dg.DienUsageDefault ?? 0m;
                var nuocUsage = dg.NuocUsageDefault ?? 0m;

                var total = (dg.DonGiaPhong ?? 0m) + (dg.DonGiaDien ?? 0m) * dienUsage + (dg.DonGiaNuoc ?? 0m) * nuocUsage;

                var hoaDon = new HoaDon
                {
                    MaHoaDon = Guid.NewGuid().ToString(),
                    MaHopDong = h.MaHopDong,
                    MaSv = h.MaSv,
                    SoTien = total,
                    TrangThai = "Chưa thanh toán",
                    NgayXuat = DateTime.Now
                };
                db.HoaDons.Add(hoaDon);

                // create notification for student
                var tb = new ThongBao
                {
                    MaThongBao = Guid.NewGuid().ToString(),
                    NguoiGui = "System",
                    NguoiNhan = sv.MaTaiKhoan,
                    LoaiThongBao = "HoaDonMoi",
                    NoiDung = $"Hóa đơn mới cho hợp đồng {h.MaHopDong}: {total}",
                    TrangThai = "Chưa đọc",
                    ThoiGianGui = DateTime.Now
                };
                db.ThongBaos.Add(tb);
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
