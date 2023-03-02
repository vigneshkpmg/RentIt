namespace web.Services;

public class IdentityService:IIdentityServices
{
    private readonly IHttpContextAccessor _contextAccessor;

    public IdentityService(IHttpContextAccessor httpContextAccessor)
    {
        _contextAccessor = httpContextAccessor;
    }
    public string? GetUserIdentity()
    {
        return _contextAccessor.HttpContext?.User.FindFirst("sub")?.Value;
    }
}