using Microsoft.EntityFrameworkCore;
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
            // Thêm cache và session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // thời gian hết hạn session
                options.Cookie.HttpOnly = true;                 // bảo mật cookie
                options.Cookie.IsEssential = true;              // bắt buộc cookie
            });

            var app = builder.Build();

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
            app.UseAuthorization();
            app.UseMiddleware<LoggingMiddleware>();
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
