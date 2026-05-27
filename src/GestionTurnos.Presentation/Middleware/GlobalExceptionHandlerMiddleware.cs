using GestionTurnos.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace GestionTurnos.Presentation.Middleware
{
    public sealed class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, title) = exception switch
            {
                NotFoundException     => (HttpStatusCode.NotFound,            "Recurso no encontrado"),
                ConflictException     => (HttpStatusCode.Conflict,            "Conflicto de datos"),
                UnauthorizedException => (HttpStatusCode.Unauthorized,        "No autorizado"),
                ValidationException   => (HttpStatusCode.UnprocessableEntity, "Error de validación"),
                DatabaseException     => (HttpStatusCode.ServiceUnavailable,  "Error de base de datos"),
                _                     => (HttpStatusCode.InternalServerError, "Error interno del servidor")
            };

            var response = new ErrorResponse(
                Status: (int)statusCode,
                Title:  title,
                Detail: exception.Message,
                TraceId: context.TraceIdentifier
            );

            context.Response.ContentType = "application/json";
            context.Response.StatusCode  = (int)statusCode;

            await context.Response.WriteAsync(
                JsonSerializer.Serialize(response, JsonOptions));
        }

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
    }

    internal sealed record ErrorResponse(int Status, string Title, string Detail, string TraceId);
}