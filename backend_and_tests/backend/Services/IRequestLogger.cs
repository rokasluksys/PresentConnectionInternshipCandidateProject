namespace backend.Services
{
    public interface IRequestLogger
    {
        Task LogAsync(HttpContext context);
    }
}