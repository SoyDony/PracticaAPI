
using System.Diagnostics;

namespace UserManagementAPI.Middlewares
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
            var stopwatch = Stopwatch.StartNew();
            
            var requestId = Guid.NewGuid().ToString();
            context.Items["RequestId"] = requestId;
            
            _logger.LogInformation(
                "Request {RequestId}: {Method} {Path} - Started",
                requestId,
                context.Request.Method,
                context.Request.Path);
            
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();
                
                _logger.LogInformation(
                    "Request {RequestId}: {Method} {Path} - Completed with {StatusCode} in {Duration}ms",
                    requestId,
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
        }
    }
}