using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using QL_KT_xa_sin_vien.Models;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Services
{
    public class ExpiringContractBackgroundService : BackgroundService
    {
        private readonly ILogger<ExpiringContractBackgroundService> _logger;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly TimeSpan _delay = TimeSpan.FromHours(24); // run once per day

        public ExpiringContractBackgroundService(ILogger<ExpiringContractBackgroundService> logger, IServiceScopeFactory scopeFactory, IConfiguration configuration)
        {
            _logger = logger;
            _scopeFactory = scopeFactory;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ExpiringContractBackgroundService started.");

            // Run immediately on startup, then wait
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckAndNotifyAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error while running expiring contract check");
                }

                try
                {
                    await Task.Delay(_delay, stoppingToken);
                }
                catch (TaskCanceledException) { }
            }

            _logger.LogInformation("ExpiringContractBackgroundService stopping.");
        }

        private async Task CheckAndNotifyAsync(CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<QLSinhVienContext>();
            var smtpSettings = _configuration.GetSection("Smtp").Get<SmtpSettings>();

            var now = DateOnly.FromDateTime(DateTime.Now);
            var soon = now.AddDays(7);

            var targets = await db.HopDongs
                .Where(h => h.NgayKetThuc.HasValue && h.NgayKetThuc.Value.CompareTo(now) >= 0 && h.NgayKetThuc.Value.CompareTo(soon) <= 0)
                .ToListAsync(cancellationToken);

            foreach (var h in targets)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var studentId = h.MaSv;
                // avoid duplicate
                var exists = db.ThongBaos.Any(t => t.LoaiThongBao == "HopDongSapHet" && t.NguoiNhan == studentId && t.NoiDung.Contains(h.MaHopDong));
                if (exists) continue;

                var tb = new ThongBao
                {
                    MaThongBao = Guid.NewGuid().ToString(),
                    NguoiGui = "System",
                    NguoiNhan = studentId,
                    LoaiThongBao = "HopDongSapHet",
                    NoiDung = $"Hợp đồng {h.MaHopDong} của bạn sẽ hết hạn vào {h.NgayKetThuc}",
                    TrangThai = "Chưa đọc",
                    ThoiGianGui = DateTime.Now
                };
                db.ThongBaos.Add(tb);

                // send email if email exists
                try
                {
                    var svEmail = await db.SinhViens.Where(s => s.MaSv == studentId).Select(s => s.Email).FirstOrDefaultAsync(cancellationToken);
                    if (!string.IsNullOrEmpty(svEmail) && smtpSettings != null)
                    {
                        using var client = new System.Net.Mail.SmtpClient(smtpSettings.Host)
                        {
                            Port = smtpSettings.Port,
                            EnableSsl = smtpSettings.EnableSsl,
                            Credentials = new System.Net.NetworkCredential(smtpSettings.User, smtpSettings.Password)
                        };
                        var mail = new System.Net.Mail.MailMessage
                        {
                            From = new System.Net.Mail.MailAddress(smtpSettings.User),
                            Subject = "Hợp đồng sắp hết hạn",
                            Body = tb.NoiDung,
                            IsBodyHtml = false
                        };
                        mail.To.Add(svEmail);
                        client.Send(mail);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to send expiring notification email for HopDong {HopDong}", h.MaHopDong);
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
