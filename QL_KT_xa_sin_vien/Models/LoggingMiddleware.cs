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
            var maTaiKhoan = context.Session.GetString("userId");
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
