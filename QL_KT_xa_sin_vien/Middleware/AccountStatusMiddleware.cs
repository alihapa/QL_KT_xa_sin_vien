using Microsoft.AspNetCore.Http;
using QL_KT_xa_sin_vien.Models;
using Microsoft.EntityFrameworkCore;

namespace QL_KT_xa_sin_vien.Middleware
{
    public class AccountStatusMiddleware
    {
        private readonly RequestDelegate _next;

        public AccountStatusMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value?.ToLower() ?? string.Empty;

            // Skip static files and common public endpoints
            if (path.StartsWith("/css") || path.StartsWith("/js") || path.StartsWith("/lib") || path.StartsWith("/images") || path.StartsWith("/favicon")
                || path.StartsWith("/home/dangnhap") || path.StartsWith("/home/taotaikhoan") || path.StartsWith("/home/quenmatkhau") || path.StartsWith("/home/datlaimatkhau")
                || path.StartsWith("/home/kichhoattaikhoan") || path.StartsWith("/home/kichhoattaikhoanconfirm")
                || path.StartsWith("/taikhoans/create") || path.StartsWith("/api"))
            {
                await _next(context);
                return;
            }

            if (context.Session == null)
            {
                await _next(context);
                return;
            }

            var userId = context.Session.GetString("userId");
            if (string.IsNullOrEmpty(userId))
            {
                await _next(context);
                return;
            }

            try
            {
                var db = context.RequestServices.GetRequiredService<QLSinhVienContext>();
                var user = await db.TaiKhoans.AsNoTracking().FirstOrDefaultAsync(t => t.MaTaiKhoan == userId);

                if (user == null)
                {
                    context.Session.Remove("users");
                    context.Session.Remove("userId");
                    context.Session.Remove("userRole");
                    context.Session.SetString("ErrorMessage", "Tài khoản không tồn tại. Vui lòng đăng nhập lại.");
                    context.Response.Redirect("/Home/DangNhap");
                    return;
                }

                if (user.TrangThai == "-1")
                {
                    context.Session.Remove("users");
                    context.Session.Remove("userId");
                    context.Session.Remove("userRole");
                    context.Session.SetString("ErrorMessage", "Tài khoản đã bị cấm. Vui lòng liên hệ quản trị viên.");
                    context.Response.Redirect("/Home/DangNhap");
                    return;
                }

                if (user.TrangThai == "0")
                {
                    context.Session.Remove("users");
                    context.Session.Remove("userId");
                    context.Session.Remove("userRole");
                    // include a hint to request activation link
                    context.Session.SetString("ErrorMessage", "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra email để kích hoạt tài khoản hoặc truy cập /Home/KichHoatTaiKhoan để gửi lại liên kết.");
                    context.Response.Redirect("/Home/DangNhap");
                    return;
                }
            }
            catch
            {
                // ignore errors and continue
            }

            await _next(context);
        }
    }
}
