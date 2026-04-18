using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace QL_KT_xa_sin_vien.Models
{
    public class LoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public LoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Guard session access: check ISessionFeature to avoid InvalidOperationException when session middleware not configured
            var sessionFeature = context.Features.Get<ISessionFeature>();
            string maTaiKhoan = null;
            if (sessionFeature?.Session != null)
            {
                maTaiKhoan = sessionFeature.Session.GetString("userId");
            }

            await _next(context);

            var method = context.Request.Method;
            var path = context.Request.Path.ToString().ToLower();

            // Chỉ log khi là tạo (POST), sửa (PUT/PATCH), xóa (DELETE), hoặc đăng xuất
            bool isModifyAction = method == HttpMethods.Post ||
                                  method == HttpMethods.Put ||
                                  method == HttpMethods.Patch ||
                                  method == HttpMethods.Delete;

            bool isLogoutAction = path.Contains("DangXuat") || path.Contains("dangxuat");

            if (!isModifyAction && !isLogoutAction)
            {
                return; // bỏ qua các request khác
            }

            var db = context.RequestServices.GetRequiredService<QLSinhVienContext>();
            

            // Nội dung log
            var logContent = isLogoutAction
                ? "Người dùng đã đăng xuất"
                : $"Method={method}, Path={path}";

            // Giới hạn 100 ký tự
            if (logContent.Length > 100)
            {
                logContent = logContent.Substring(0, 100);
            }

            // Nếu không có maTaiKhoan hợp lệ thì bỏ qua ghi nhật ký để tránh vi phạm khóa ngoại
            if (string.IsNullOrEmpty(maTaiKhoan))
            {
                return;
            }

            // Optionally verify the account exists to be safe
            var taiKhoan = db.TaiKhoans.FirstOrDefault(t => t.MaTaiKhoan == maTaiKhoan);
            if (taiKhoan == null)
            {
                // Nếu tài khoản không tồn tại, không ghi nhật ký để tránh lỗi FK
                return;
            }

            var log = new NhatKy
            {
                MaLog = Guid.NewGuid().ToString(),
                NguoiThucHien = maTaiKhoan,
                HanhDong = isLogoutAction ? "Đăng xuất" : method,
                DoiTuong = path,
                GiaTriSau = logContent,
                ThoiGian = DateTime.Now
            };

            db.NhatKies.Add(log);
            await db.SaveChangesAsync();
        }
    }
}
