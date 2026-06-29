namespace UserService.Presentation.Utils;

public static class JwtUtils
{
    public static bool IsAdmin(HttpContext context)
    {
        string? sub = context.User.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        return sub == "microservice";
    }
}