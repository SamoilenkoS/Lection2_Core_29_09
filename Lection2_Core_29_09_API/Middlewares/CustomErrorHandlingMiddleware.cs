namespace Lection2_Core_API.Middlewares
{
    public class CustomErrorHandlingMiddleware
    {
        private readonly ILogger<CustomErrorHandlingMiddleware> _logger;
        private readonly RequestDelegate _next;

        public CustomErrorHandlingMiddleware(
            ILogger<CustomErrorHandlingMiddleware> logger,
            RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Request.EnableBuffering();
            var host = context.Request.Host.Value;
            var port = context.Request.Host.Port;
            var path = context.Request.Path;
            var method = context.Request.Method;
            _logger.LogInformation("Request: {host}:{port}{path} {method}", host, port, path, method);

            await _next(context);

            var body = await (new StreamReader(context.Request.Body).ReadToEndAsync());
            _logger.LogInformation("Response: {statusCode}", context.Response.StatusCode);
        }
    }
}
