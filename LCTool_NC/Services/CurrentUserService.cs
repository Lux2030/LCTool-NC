using System.Security.Claims;

namespace LCTool_NC.Services;

public class CurrentUserService
{
    private const int MaxUserLength = 256;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string GetUserName() => GetCurrentUser();

    public string GetCurrentUser()
    {
        var context = _httpContextAccessor.HttpContext;

        var currentUser = GetAuthenticatedUser(context)
            ?? GetClaimUser(context)
            ?? GetHeaderUser(context)
            ?? GetLocalProcessUser();

        return StripDomain(NormalizeUser(currentUser));
    }

    private static string? GetAuthenticatedUser(HttpContext? context)
    {
        var identity = context?.User?.Identity;
        if (identity?.IsAuthenticated == true && !string.IsNullOrWhiteSpace(identity.Name))
            return identity.Name;
        return null;
    }

    private static string? GetClaimUser(HttpContext? context)
    {
        return context?.User?.Claims
            ?.FirstOrDefault(c =>
                c.Type == ClaimTypes.Name ||
                c.Type == ClaimTypes.Upn ||
                c.Type.Equals("name", StringComparison.OrdinalIgnoreCase) ||
                c.Type.Equals("preferred_username", StringComparison.OrdinalIgnoreCase))
            ?.Value;
    }

    private static string? GetHeaderUser(HttpContext? context)
    {
        if (context == null) return null;

        var headerNames = new[]
        {
            "X-Windows-User",
            "X-User-Name",
            "X-Forwarded-User",
            "REMOTE_USER"
        };

        foreach (var headerName in headerNames)
        {
            if (context.Request.Headers.TryGetValue(headerName, out var values))
            {
                var value = values.FirstOrDefault();
                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }
        }
        return null;
    }

    private static string GetLocalProcessUser()
    {
        var userName = Environment.UserName;
        var domainName = Environment.UserDomainName;

        if (string.IsNullOrWhiteSpace(userName)) return "Sconosciuto";
        if (string.IsNullOrWhiteSpace(domainName)) return userName;
        return $"{domainName}\\{userName}";
    }

    private static string NormalizeUser(string? value)
    {
        var normalized = string.IsNullOrWhiteSpace(value) ? "Sconosciuto" : value.Trim();
        return normalized.Length <= MaxUserLength ? normalized : normalized[..MaxUserLength];
    }

    private static string StripDomain(string value)
    {
        var idx = value.IndexOf('\\');
        if (idx >= 0 && idx < value.Length - 1)
            return value[(idx + 1)..];
        return value;
    }
}
