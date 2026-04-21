using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using QL_KT_xa_sin_vien.Middleware;
using QL_KT_xa_sin_vien.Models;

namespace QL_KT_xa_sin_vien
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<QLSinhVienContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("ABC")));

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            // register invoice background worker
            builder.Services.AddHostedService<QL_KT_xa_sin_vien.Services.InvoiceBackgroundService>();
            // register account expiry cleanup worker
            builder.Services.AddHostedService<QL_KT_xa_sin_vien.Services.AccountExpiryBackgroundService>();
            // Thêm cache và session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian hết hạn session
                options.Cookie.HttpOnly = true;                 // bảo mật cookie
                options.Cookie.IsEssential = true;              // bắt buộc cookie
            });
            // Thêm dịch vụ smtp
            builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("Smtp"));


            var app = builder.Build();
            var supportedCultures = new[] { "vi-VN" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture("vi-VN")
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<QLSinhVienContext>();

                // Nếu chưa có vai trò nào thì thêm
                if (!db.VaiTros.Any())
                {
                    db.VaiTros.AddRange(
                        new VaiTro { MaVaiTro = "1", TenVaiTro = "Sinh viên", QuyenHan = "1" },
                        new VaiTro { MaVaiTro = "2", TenVaiTro = "BQL", QuyenHan = "2" },
                        new VaiTro { MaVaiTro = "3", TenVaiTro = "Admin", QuyenHan = "3" },
                        new VaiTro { MaVaiTro = "4", TenVaiTro = "KeToan", QuyenHan = "4"}
                    );
                    db.SaveChanges();
                }

                // Nếu chưa có tài khoản nào thì thêm tài khoản mặc định
                if (!db.TaiKhoans.Any())
                {
                    var hasher = new PasswordHasher<TaiKhoan>();
                    var tk = new TaiKhoan
                    {
                        MaTaiKhoan = Guid.NewGuid().ToString(),
                        TenDangNhap = "admin",
                        MatKhauMh = hasher.HashPassword(null, "123456"), // hash bằng PasswordHasher
                        Email = "admin@example.com",
                        Sdt = "0123456789",
                        VaiTro = "3", // Admin
                        TrangThai = "1"
                    };

                    db.TaiKhoans.Add(tk);
                    db.SaveChanges();
                }
            }
            app.UseRequestLocalization(localizationOptions);

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            // Bật session trước khi dùng endpoints
            app.UseSession();
            // Register logging middleware after session is enabled so it can read session safely
            app.UseMiddleware<QL_KT_xa_sin_vien.Middleware.LoggingMiddleware>();
            // Register account status middleware to enforce account activation/banned rules across all requests
            app.UseMiddleware<QL_KT_xa_sin_vien.Middleware.AccountStatusMiddleware>();
            app.UseAuthorization();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
