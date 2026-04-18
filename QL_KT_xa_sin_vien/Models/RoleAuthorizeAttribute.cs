using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Linq;

public class RoleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _roles;

    public RoleAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        // Guard: do not access HttpContext.Session directly because it throws when session is not configured.
        var sessionFeature = context.HttpContext.Features.Get<ISessionFeature>();
        if (sessionFeature?.Session == null)
        {
            // Session is not available; redirect to login
            context.HttpContext.Items["ErrorMessage"] = "Phiên làm việc chưa được cấu hình";
            context.Result = new RedirectToActionResult("DangNhap", "Home", null);
            return;
        }

        var role = sessionFeature.Session.GetString("userRole");

        if (string.IsNullOrEmpty(role) || !_roles.Contains(role))
        {
            context.HttpContext.Items["ErrorMessage"] = "Quyền hạn không đủ";
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
