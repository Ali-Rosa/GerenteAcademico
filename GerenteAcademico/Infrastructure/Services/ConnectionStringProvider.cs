using GerenteAcademico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Services
{
    /// <summary>
    /// Servicio para obtener dinámicamente cadenas de conexión desde AcademiaConfig
    /// </summary>
    public interface IConnectionStringProvider
    {
        /// <summary>
        /// Obtiene la cadena de conexión válida para una academia específica
        /// </summary>
        /// <param name="academiaCodigo">Código de la academia</param>
        /// <returns>Cadena de conexión validada, null si no es válida</returns>
        Task<string?> GetConnectionStringAsync(string academiaCodigo);

        /// <summary>
        /// Obtiene la cadena de conexión con detalles de validación
        /// </summary>
        Task<(string? connectionString, ValidationResult validationResult)> GetConnectionStringWithValidationAsync(string academiaCodigo);

        /// <summary>
        /// Obtiene la cadena de conexión sin validación (para desarrollo)
        /// </summary>
        Task<string?> GetConnectionStringUncheckedAsync(string academiaCodigo);
    }

    public class ConnectionStringProvider : IConnectionStringProvider
    {
        private readonly ConfigDbContext _configDb;
        private readonly IConnectionStringValidator _validator;
        private readonly ILogger<ConnectionStringProvider> _logger;

        public ConnectionStringProvider(
            ConfigDbContext configDb,
            IConnectionStringValidator validator,
            ILogger<ConnectionStringProvider> logger)
        {
            _configDb = configDb;
            _validator = validator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene la cadena de conexión validada desde AcademiaConfig
        /// </summary>
        public async Task<string?> GetConnectionStringAsync(string academiaCodigo)
        {
            if (string.IsNullOrWhiteSpace(academiaCodigo))
            {
                _logger.LogWarning("Código de academia vacío al obtener cadena de conexión");
                return null;
            }

            try
            {
                // Buscar la academia en ConfigDatabase
                var academia = await _configDb.Academias
                    .Where(a => a.Codigo == academiaCodigo && a.Activo)
                    .FirstOrDefaultAsync();

                if (academia is null)
                {
                    _logger.LogWarning($"Academia '{academiaCodigo}' no encontrada o no está activa");
                    return null;
                }

                if (string.IsNullOrWhiteSpace(academia.CadenaConexionPrincipal))
                {
                    _logger.LogWarning($"Academia '{academiaCodigo}' no tiene cadena de conexión configurada");
                    return null;
                }

                // Validar la cadena de conexión
                var isValid = await _validator.IsValidAsync(academia.CadenaConexionPrincipal);

                if (!isValid)
                {
                    _logger.LogError($"Cadena de conexión inválida para academia '{academiaCodigo}'");
                    return null;
                }

                _logger.LogInformation($"Cadena de conexión obtenida exitosamente para academia '{academiaCodigo}'");
                return academia.CadenaConexionPrincipal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cadena de conexión para '{academiaCodigo}': {ex.Message}");
                return null;
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión con información detallada de la validación
        /// </summary>
        public async Task<(string? connectionString, ValidationResult validationResult)> GetConnectionStringWithValidationAsync(string academiaCodigo)
        {
            if (string.IsNullOrWhiteSpace(academiaCodigo))
            {
                return (null, new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "Código de academia vacío"
                });
            }

            try
            {
                var academia = await _configDb.Academias
                    .Where(a => a.Codigo == academiaCodigo && a.Activo)
                    .FirstOrDefaultAsync();

                if (academia is null)
                {
                    return (null, new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = $"Academia '{academiaCodigo}' no encontrada o no está activa"
                    });
                }

                if (string.IsNullOrWhiteSpace(academia.CadenaConexionPrincipal))
                {
                    return (null, new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "Academia sin cadena de conexión configurada"
                    });
                }

                var validationResult = await _validator.ValidateWithDetailsAsync(academia.CadenaConexionPrincipal);

                if (validationResult.IsValid)
                {
                    _logger.LogInformation($"Cadena de conexión validada para '{academiaCodigo}'");
                    return (academia.CadenaConexionPrincipal, validationResult);
                }
                else
                {
                    _logger.LogError($"Validación fallida para '{academiaCodigo}': {validationResult.ErrorMessage}");
                    return (null, validationResult);
                }
            }
            catch (Exception ex)
            {
                var errorMsg = $"Error al obtener cadena de conexión: {ex.Message}";
                _logger.LogError(errorMsg);

                return (null, new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = errorMsg
                });
            }
        }

        /// <summary>
        /// Obtiene la cadena de conexión sin validación (útil para desarrollo/debugging)
        /// </summary>
        public async Task<string?> GetConnectionStringUncheckedAsync(string academiaCodigo)
        {
            if (string.IsNullOrWhiteSpace(academiaCodigo))
                return null;

            try
            {
                var academia = await _configDb.Academias
                    .Where(a => a.Codigo == academiaCodigo && a.Activo)
                    .FirstOrDefaultAsync();

                return academia?.CadenaConexionPrincipal;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al obtener cadena sin validar: {ex.Message}");
                return null;
            }
        }
    }
}
