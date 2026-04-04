using GerenteAcademico.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenteAcademico.Web.Controllers
{
    /// <summary>
    /// Controlador para validar y obtener información sobre conexiones de bases de datos
    /// </summary>
    [ApiController]
    [Route("api/conexiones")]
    public class ConexionesController : ControllerBase
    {
        private readonly IConnectionStringValidator _validator;
        private readonly IConnectionStringProvider _provider;
        private readonly ILogger<ConexionesController> _logger;

        public ConexionesController(
            IConnectionStringValidator validator,
            IConnectionStringProvider provider,
            ILogger<ConexionesController> logger)
        {
            _validator = validator;
            _provider = provider;
            _logger = logger;
        }

        /// <summary>
        /// Valida una cadena de conexión
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a validar</param>
        /// <returns>Resultado de la validación</returns>
        [HttpPost("validar")]
        public async Task<ActionResult> ValidarConexion([FromBody] ValidarConexionRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.ConnectionString))
            {
                return BadRequest(new { message = "La cadena de conexión no puede estar vacía" });
            }

            var resultado = await _validator.ValidateWithDetailsAsync(request.ConnectionString, request.TimeoutSeconds ?? 5);

            if (resultado.IsValid)
            {
                return Ok(new
                {
                    success = true,
                    message = "Conexión válida",
                    servidor = resultado.Server,
                    baseDatos = resultado.Database,
                    usuario = resultado.UserId
                });
            }
            else
            {
                return BadRequest(new
                {
                    success = false,
                    error = resultado.ErrorMessage,
                    sqlErrorNumber = resultado.SqlErrorNumber
                });
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión validada para una academia
        /// </summary>
        /// <param name="academiaCodigo">Código de la academia</param>
        /// <returns>Información sobre la conexión</returns>
        [HttpGet("academia/{academiaCodigo}")]
        public async Task<ActionResult> ObtenerConexionAcademia(string academiaCodigo)
        {
            if (string.IsNullOrWhiteSpace(academiaCodigo))
            {
                return BadRequest(new { message = "Código de academia requerido" });
            }

            var (connectionString, validationResult) = await _provider.GetConnectionStringWithValidationAsync(academiaCodigo);

            if (!validationResult.IsValid)
            {
                return NotFound(new
                {
                    success = false,
                    academia = academiaCodigo,
                    error = validationResult.ErrorMessage
                });
            }

            return Ok(new
            {
                success = true,
                academia = academiaCodigo,
                servidor = validationResult.Server,
                baseDatos = validationResult.Database,
                usuario = validationResult.UserId,
                cadenaConfigurada = !string.IsNullOrEmpty(connectionString)
            });
        }

        /// <summary>
        /// Verifica la salud de la conexión para una academia
        /// </summary>
        /// <param name="academiaCodigo">Código de la academia</param>
        /// <returns>Estado de la conexión</returns>
        [HttpGet("salud/{academiaCodigo}")]
        public async Task<ActionResult> VerificarSalud(string academiaCodigo)
        {
            if (string.IsNullOrWhiteSpace(academiaCodigo))
            {
                return BadRequest(new { mensaje = "Código de academia requerido" });
            }

            try
            {
                var connectionString = await _provider.GetConnectionStringAsync(academiaCodigo);

                if (connectionString is null)
                {
                    return StatusCode(503, new
                    {
                        estado = "degradado",
                        academia = academiaCodigo,
                        razon = "No se pudo obtener o validar la cadena de conexión"
                    });
                }

                return Ok(new
                {
                    estado = "saludable",
                    academia = academiaCodigo,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al verificar salud de {academiaCodigo}: {ex.Message}");

                return StatusCode(500, new
                {
                    estado = "error",
                    academia = academiaCodigo,
                    mensaje = ex.Message
                });
            }
        }
    }

    public class ValidarConexionRequest
    {
        public string ConnectionString { get; set; } = "";
        public int? TimeoutSeconds { get; set; }
    }
}
