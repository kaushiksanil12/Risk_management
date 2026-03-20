using System.Diagnostics;

namespace ERMS.API.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                sw.Stop();
                _logger.LogInformation(
                    "[{Timestamp}] {Method} {Path}{QueryString} → {StatusCode} ({Duration}ms)",
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    context.Request.Method,
                    context.Request.Path,
                    context.Request.QueryString,
                    context.Response.StatusCode,
                    sw.ElapsedMilliseconds);
            }
        }
    }
}
