using System.Net;
using System.Text.Json;
using GerenteAcademico.Domain.Exceptions;

namespace GerenteAcademico.Web.Middleware
{
    /// <summary>
    /// Middleware global que captura TODAS las excepciones y las convierte en respuestas HTTP apropiadas.
    /// Esto garantiza una respuesta consistente y profesional para todos los errores.
    /// </summary>
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlerMiddleware> _logger;
        private readonly IWebHostEnvironment _environment;

        public GlobalExceptionHandlerMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlerMiddleware> logger,
            IWebHostEnvironment environment)
        {
            _next = next;
            _logger = logger;
            _environment = environment;
        }

        /// <summary>
        /// Método que se ejecuta en cada solicitud.
        /// Envuelve la siguiente etapa del pipeline en un try-catch.
        /// </summary>
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Continuar con el pipeline normal
                await _next(context);
            }
            catch (Exception ex)
            {
                // Capturar la excepción y manejarla
                await HandleExceptionAsync(context, ex);
            }
        }

        /// <summary>
        /// Maneja la excepción y devuelve una respuesta HTTP apropiada.
        /// </summary>
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // ID único para correlacionar con logs
            var traceId = context.TraceIdentifier;

            // Log de la excepción (siempre)
            _logger.LogError(
                exception,
                "Se capturó una excepción no manejada. TraceId: {TraceId}",
                traceId);

            // Preparar respuesta
            var response = context.Response;
            response.ContentType = "application/json";

            // Determinar código de estado y respuesta según tipo de excepción
            int statusCode;
            object errorResponse;

            if (exception is AcademiaNotFoundException ex404)
            {
                statusCode = 404;
                errorResponse = new
                {
                    error = ex404.Message,
                    code = "ACADEMIA_NOT_FOUND",
                    traceId = traceId,
                    timestamp = DateTime.UtcNow
                };
            }
            else if (exception is AcademiaInactiveException exInactive)
            {
                statusCode = 403;
                errorResponse = new
                {
                    error = exInactive.Message,
                    code = "ACADEMIA_INACTIVE",
                    traceId = traceId,
                    timestamp = DateTime.UtcNow
                };
            }
            else if (exception is AcademiaIncompleteDataException exIncomplete)
            {
                statusCode = 422;
                errorResponse = new
                {
                    error = exIncomplete.Message,
                    code = "ACADEMY_INCOMPLETE_DATA",
                    traceId = traceId,
                    timestamp = DateTime.UtcNow
                };
            }
            else
            {
                statusCode = 500;
                errorResponse = new
                {
                    error = _environment.IsDevelopment() ? exception.Message : "Error interno del servidor",
                    code = "INTERNAL_SERVER_ERROR",
                    details = _environment.IsDevelopment() ? exception.StackTrace : null,
                    traceId = traceId,
                    timestamp = DateTime.UtcNow
                };
            }

            response.StatusCode = statusCode;

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            await response.WriteAsJsonAsync(errorResponse, options);
        }
    }
}
