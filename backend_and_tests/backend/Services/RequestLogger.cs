using System.Text;

namespace backend.Services
{
    public class RequestLogger : IRequestLogger
    {
        private readonly ILogger<RequestLogger> _logger;

        public RequestLogger(ILogger<RequestLogger> logger)
        {
            _logger = logger;
        }

        public Task LogAsync(HttpContext context)
        {
            var request = context.Request;

            var sb = new StringBuilder();
            sb.AppendLine("=== Incoming Request ===");
            sb.AppendLine($"Method: {request.Method}");
            sb.AppendLine($"Path: {request.Path}");
            sb.AppendLine($"Query: {request.QueryString}");
            sb.AppendLine($"IP: {context.Connection.RemoteIpAddress}");

            _logger.LogInformation(sb.ToString());

            return Task.CompletedTask;
        }
    }
}