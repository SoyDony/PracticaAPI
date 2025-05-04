
using System.Text.Json;

namespace UserManagementAPI.Middlewares
{
    public class TokenAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenAuthenticationMiddleware> _logger;
        private readonly List<string> _validTokens;
        
        public TokenAuthenticationMiddleware(RequestDelegate next, ILogger<TokenAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _validTokens = new List<string> { "valid-token-123", "admin-token-456" };
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.ToString();
            
            if (path.StartsWith("/swagger") || path.StartsWith("/api/swagger"))
            {
                await _next(context);
                return;
            }
            
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                _logger.LogWarning("Missing or invalid authorization header");
                await RespondWithUnauthorized(context);
                return;
            }
            
            var token = authHeader.Substring("Bearer ".Length).Trim();
            
            if (!IsValidToken(token))
            {
                _logger.LogWarning("Invalid token: {Token}", token);
                await RespondWithUnauthorized(context);
                return;
            }
            
            context.Items["User"] = GetUserFromToken(token);
            await _next(context);
        }
        
        private bool IsValidToken(string token)
        {
            return _validTokens.Contains(token);
        }
        
        private object GetUserFromToken(string token)
        {
            return token switch
            {
                "valid-token-123" => new { Username = "user1", Role = "User" },
                "admin-token-456" => new { Username = "admin", Role = "Admin" },
                _ => null
            };
        }
        
        private async Task RespondWithUnauthorized(HttpContext context)
        {
            context.Response.StatusCode = 401;
            context.Response.ContentType = "application/json";
            
            var response = new { error = "Unauthorized access" };
            var jsonResponse = JsonSerializer.Serialize(response);
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
}