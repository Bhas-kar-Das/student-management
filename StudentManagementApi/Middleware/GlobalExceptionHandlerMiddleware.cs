using System.Net;
using System.Text.Json;

namespace StudentManagementApi.Middleware
{
    // Global exception handling middleware
    public class GlobalExceptionHandlerMiddleware
    {
        // Next middleware in pipeline
        private readonly RequestDelegate _next;

        // Logger instance
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;

        // Constructor
        public GlobalExceptionHandlerMiddleware(RequestDelegate next, ILogger<GlobalExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        // Invoke method - processes each request
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continue to next middleware
                await _next(context);
            }
            catch (Exception ex)
            {
                // Log the exception
                _logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);

                // Handle the exception
                await HandleExceptionAsync(context, ex);
            }
        }

        // Handle exception and return appropriate response
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Set response content type
            context.Response.ContentType = "application/json";

            // Set default status code to 500 (Internal Server Error)
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Create error response
            var response = new
            {
                statusCode = context.Response.StatusCode,
                message = "An error occurred while processing your request",
                details = exception.Message,
                timestamp = DateTime.Now
            };

            // Log different exception types
            switch (exception)
            {
                case ArgumentNullException argEx:
                    _logger.LogWarning(argEx, "Argument null exception: {Message}", argEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "Invalid input provided",
                        details = argEx.Message,
                        timestamp = DateTime.Now
                    };
                    break;

                case InvalidOperationException invEx:
                    _logger.LogWarning(invEx, "Invalid operation: {Message}", invEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "Invalid operation",
                        details = invEx.Message,
                        timestamp = DateTime.Now
                    };
                    break;

                case KeyNotFoundException keyEx:
                    _logger.LogWarning(keyEx, "Resource not found: {Message}", keyEx.Message);
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    response = new
                    {
                        statusCode = context.Response.StatusCode,
                        message = "Resource not found",
                        details = keyEx.Message,
                        timestamp = DateTime.Now
                    };
                    break;

                default:
                    _logger.LogError(exception, "Unhandled exception: {Message}", exception.Message);
                    break;
            }

            // Serialize and write response
            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Extension method to easily add middleware to pipeline
    public static class GlobalExceptionHandlerMiddlewareExtensions
    {
        // Add global exception handler middleware to pipeline
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GlobalExceptionHandlerMiddleware>();
        }
    }
}
