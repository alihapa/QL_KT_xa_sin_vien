using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using QL_KT_xa_sin_vien.Models;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Services;

public class AccountExpiryBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<AccountExpiryBackgroundService> _logger;
    private readonly TimeSpan _delay = TimeSpan.FromHours(24);

    public AccountExpiryBackgroundService(IServiceScopeFactory scopeFactory, ILogger<AccountExpiryBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("AccountExpiryBackgroundService started.");
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await CheckAndDeleteExpired(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during account expiry check");
            }

            try { await Task.Delay(_delay, stoppingToken); } catch { }
        }
    }

    private async Task CheckAndDeleteExpired(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<QLSinhVienContext>();
        var now = DateTime.Now;
        var expired = await db.TaiKhoans.Where(t => t.ThoiHan.HasValue && t.ThoiHan <= now).ToListAsync(cancellationToken);
        if (!expired.Any()) return;

        foreach (var tk in expired)
        {
            // delete related sinhvien entries
            var sv = db.SinhViens.FirstOrDefault(s => s.MaTaiKhoan == tk.MaTaiKhoan);
            if (sv != null) db.SinhViens.Remove(sv);
            // log
            db.NhatKies.Add(new NhatKy
            {
                MaLog = Guid.NewGuid().ToString(),
                NguoiThucHien = "System",
                HanhDong = "AutoDeleteExpiredAccount",
                DoiTuong = tk.TenDangNhap ?? tk.MaTaiKhoan,
                GiaTriTruoc = tk.TrangThai,
                GiaTriSau = "(deleted due expiry)",
                ThoiGian = DateTime.Now
            });
            db.TaiKhoans.Remove(tk);
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
