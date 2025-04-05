namespace Dispatch.API
{
    public class ExpiryMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExpiryMiddleware> _logger;
        private readonly DateTime _expiryDate;

        public ExpiryMiddleware(RequestDelegate next, ILogger<ExpiryMiddleware> logger, IConfiguration config)
        {
            _next = next;
            _logger = logger;
            var expiryStr = config["AppSecurity:ExpiryDate"];
            _expiryDate = DateTime.TryParse(expiryStr, out var date) ? date : DateTime.MaxValue;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (DateTime.UtcNow > _expiryDate)
            {
                _logger.LogWarning("Application access expired on {_expiryDate}", _expiryDate);
                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Access to this application has expired. Please contact the developer.");
                return;
            }

            await _next(context);
        }
    }
}
