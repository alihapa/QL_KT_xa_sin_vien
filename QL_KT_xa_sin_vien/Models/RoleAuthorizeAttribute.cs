using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class RoleAuthorizeAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _roles;

    public RoleAuthorizeAttribute(params string[] roles)
    {
        _roles = roles;
    }

    public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var role = context.HttpContext.Session.GetString("userRole");

        if (string.IsNullOrEmpty(role) || !_roles.Contains(role))
        {
            context.HttpContext.Items["ErrorMessage"] = "Quyền hạn không đủ";
            context.Result = new RedirectToActionResult("Index", "Home", null);
        }
    }
}
