using InventoryAPI.Exceptions;

namespace InventoryAPI.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (DomainException ex) 
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    code = ex.Code,
                    message = ex.Message,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (ValidationException ex)
            {
                context.Response.StatusCode = 400;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    message = "Validation failed",
                    errors = ex.Errors.Select(e => new
                    {
                        code = e.Code,
                        message = e.Message
                    }).ToList(),
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                string? messageOut = null;
                if (ex is InvalidOperationException)
                {
                    context.Response.StatusCode = 400;
                    messageOut = ex.Message;
                }
                else
                {
                    context.Response.StatusCode = 500;
                }

                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(new
                {
                    message = messageOut ?? "Internal server error",
                    created = DateTime.UtcNow
                });

                _logger.LogError(ex, "An error occurred while processing request {Path}", context.Request.Path);

                return;
            }
        }
    }
}