using Microsoft.Data.SqlClient;

namespace GerenteAcademico.Infrastructure.Services
{
    /// <summary>
    /// Servicio para validar cadenas de conexión SQL Server
    /// </summary>
    public interface IConnectionStringValidator
    {
        /// <summary>
        /// Valida si una cadena de conexión es válida e intenta conectarse
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a validar</param>
        /// <param name="timeoutSeconds">Timeout en segundos para la validación (default 5)</param>
        /// <returns>true si la conexión es válida, false en caso contrario</returns>
        Task<bool> IsValidAsync(string connectionString, int timeoutSeconds = 5);

        /// <summary>
        /// Obtiene información sobre la validación de la conexión
        /// </summary>
        /// <param name="connectionString">Cadena de conexión a validar</param>
        /// <param name="timeoutSeconds">Timeout en segundos para la validación (default 5)</param>
        /// <returns>Objeto con información de la validación</returns>
        Task<ValidationResult> ValidateWithDetailsAsync(string connectionString, int timeoutSeconds = 5);
    }

    public class SqlConnectionValidator : IConnectionStringValidator
    {
        private readonly ILogger<SqlConnectionValidator> _logger;

        public SqlConnectionValidator(ILogger<SqlConnectionValidator> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Valida si una cadena de conexión SQL Server es válida
        /// </summary>
        public async Task<bool> IsValidAsync(string connectionString, int timeoutSeconds = 5)
        {
            var result = await ValidateWithDetailsAsync(connectionString, timeoutSeconds);
            return result.IsValid;
        }

        /// <summary>
        /// Valida con detalles de error si ocurre alguno
        /// </summary>
        public async Task<ValidationResult> ValidateWithDetailsAsync(string connectionString, int timeoutSeconds = 5)
        {
            if (string.IsNullOrWhiteSpace(connectionString))
            {
                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = "La cadena de conexión está vacía o nula"
                };
            }

            try
            {
                // Intentar parsear la cadena de conexión
                var builder = new SqlConnectionStringBuilder(connectionString);

                // Validar que tiene los componentes mínimos
                if (string.IsNullOrWhiteSpace(builder.DataSource) || string.IsNullOrWhiteSpace(builder.InitialCatalog))
                {
                    return new ValidationResult
                    {
                        IsValid = false,
                        ErrorMessage = "La cadena de conexión no contiene Server o Database"
                    };
                }

                // Intentar conectarse
                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    await connection.CloseAsync();
                }

                _logger.LogInformation($"Cadena de conexión validada correctamente para: {builder.DataSource}.{builder.InitialCatalog}");

                return new ValidationResult
                {
                    IsValid = true,
                    Server = builder.DataSource,
                    Database = builder.InitialCatalog,
                    UserId = builder.UserID
                };
            }
            catch (SqlException ex)
            {
                var errorMsg = ex.Number switch
                {
                    18456 => "Credenciales inválidas (Usuario o contraseña incorrectos)",
                    -1 => "Timeout al conectarse al servidor",
                    -2 => "Timeout al conectarse al servidor",
                    20 => "No se puede conectar al servidor especificado",
                    _ => ex.Message
                };

                _logger.LogWarning($"Error de SQL al validar conexión: {errorMsg}");

                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Error de SQL: {errorMsg}",
                    SqlErrorNumber = ex.Number
                };
            }
            catch (FormatException ex)
            {
                _logger.LogWarning($"Cadena de conexión con formato inválido: {ex.Message}");

                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Formato de cadena de conexión inválido: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error inesperado al validar conexión: {ex.Message}");

                return new ValidationResult
                {
                    IsValid = false,
                    ErrorMessage = $"Error inesperado: {ex.Message}"
                };
            }
        }
    }

    /// <summary>
    /// Resultado de la validación de cadena de conexión
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Server { get; set; }
        public string? Database { get; set; }
        public string? UserId { get; set; }
        public int? SqlErrorNumber { get; set; }
    }
}
