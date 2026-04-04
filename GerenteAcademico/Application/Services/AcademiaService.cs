using GerenteAcademico.Application.Dtos;
using GerenteAcademico.Domain.Entities;
using GerenteAcademico.Domain.Exceptions;
using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Infrastructure.Services;

namespace GerenteAcademico.Application.Services
{
    /// <summary>
    /// Servicio de aplicación para académias.
    /// Solo proporciona operaciones de LECTURA y VALIDACIÓN.
    /// Las academias se crean manualmente en la BD, esta app solo las consume.
    /// </summary>
    public class AcademiaService
    {
        private readonly IAcademiaRepository _repo;
        private readonly IConnectionStringValidator _connectionValidator;
        private readonly ILogger<AcademiaService> _logger;

        public AcademiaService(
            IAcademiaRepository repo,
            IConnectionStringValidator connectionValidator,
            ILogger<AcademiaService> logger)
        {
            _repo = repo;
            _connectionValidator = connectionValidator;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene TODOS los datos de una academia.
        /// </summary>
        public async Task<List<AcademiaConfig>> GetAllAsync()
        {
            return await _repo.GetAllAsync();
        }

        /// <summary>
        /// Obtiene una academia por su ID.
        /// </summary>
        public async Task<AcademiaConfig?> GetByIdAsync(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        /// <summary>
        /// Obtiene una academia por su código.
        /// </summary>
        public async Task<AcademiaConfig?> GetByCodigoAsync(string codigo)
        {
            return await _repo.GetByCodigoAsync(codigo);
        }

        /// <summary>
        /// Obtiene y valida completamente una academia antes del login.
        /// 
        /// IMPORTANTE: Esta es la validación inicial más crítica del sistema.
        /// Verifica que:
        /// 1. La academia existe en ConfigDatabase
        /// 2. La academia está ACTIVA (no desactivada)
        /// 3. Todos los campos obligatorios están completos (nombre, dirección, etc)
        /// 4. La cadena de conexión (CadenaConexionPrincipal) está presente
        /// 5. La cadena de conexión es VÁLIDA y puede conectarse a SQL Server
        /// 
        /// Si cualquier validación falla, lanza una excepción específica.
        /// Esto previene que usuarios intenten loguearse en academias sin BD disponible.
        /// </summary>
        public async Task<AcademiaInitialDataDto> GetAndValidateAsync(string codigo)
        {
            // 1. Obtener la academia
            var academia = await _repo.GetByCodigoAsync(codigo);
            if (academia == null)
                throw new AcademiaNotFoundException(codigo);

            // 2. Validar que está activa
            if (!academia.Activo)
                throw new AcademiaInactiveException(codigo);

            // 3. Validar que tiene todos los datos obligatorios
            ValidateRequiredFields(academia);

            // 4. CRÍTICO: Validar la cadena de conexión
            // 
            // Esta es la validación más importante para el sistema multi-academia.
            // Si la cadena de conexión es inválida:
            //   - El usuario no podrá loguearse
            //   - Los datos de la academia no se cargarán
            //   - El dashboard no estará disponible
            //
            // Por eso validamos AQUÍ, antes de mostrar el login, para fallar rápido.
            await ValidateConnectionStringAsync(academia);

            // 5. Devolver DTO con datos iniciales
            return MapToInitialDataDto(academia);
        }

        /// <summary>
        /// Valida que la cadena de conexión (CadenaConexionPrincipal) sea correcta.
        /// 
        /// IMPORTANTE: Esto intenta CONECTAR realmente a SQL Server.
        /// Si falla, significa que:
        ///   - El servidor SQL no es accesible
        ///   - Las credenciales son incorrectas
        ///   - La base de datos no existe
        ///   - Hay problemas de firewall/red
        /// </summary>
        private async Task ValidateConnectionStringAsync(AcademiaConfig academia)
        {
            if (string.IsNullOrWhiteSpace(academia.CadenaConexionPrincipal))
            {
                throw new InvalidOperationException($"Academia '{academia.Codigo}' no tiene cadena de conexión configurada");
            }

            try
            {
                var isValid = await _connectionValidator.IsValidAsync(academia.CadenaConexionPrincipal, timeoutSeconds: 5);

                if (!isValid)
                {
                    _logger.LogWarning($"La cadena de conexión para '{academia.Codigo}' no es válida o no se puede conectar");
                    throw new InvalidOperationException($"No se puede conectar a la base de datos de '{academia.Nombre}'. Por favor, contacte al administrador.");
                }

                _logger.LogInformation($"Cadena de conexión validada correctamente para academia '{academia.Codigo}'");
            }
            catch (Exception ex) when (!(ex is InvalidOperationException))
            {
                _logger.LogError($"Error al validar conexión para '{academia.Codigo}': {ex.Message}");
                throw new InvalidOperationException($"Error de validación de base de datos para '{academia.Nombre}'", ex);
            }
        }        /// <summary>
        /// Valida que una academia tenga todos los datos obligatorios.
        /// Lanza AcademiaIncompleteDataException si faltan campos.
        /// </summary>
        private void ValidateRequiredFields(AcademiaConfig academia)
        {
            var missingFields = new List<string>();

            // Validar campos obligatorios
            if (string.IsNullOrWhiteSpace(academia.Codigo))
                missingFields.Add("Codigo");

            if (string.IsNullOrWhiteSpace(academia.Nombre))
                missingFields.Add("Nombre");

            if (string.IsNullOrWhiteSpace(academia.CadenaConexionPrincipal))
                missingFields.Add("CadenaConexionPrincipal");

            if (string.IsNullOrWhiteSpace(academia.Descripcion))
                missingFields.Add("Descripcion");

            if (string.IsNullOrWhiteSpace(academia.Direccion))
                missingFields.Add("Direccion");

            if (string.IsNullOrWhiteSpace(academia.Telefono))
                missingFields.Add("Telefono");

            if (string.IsNullOrWhiteSpace(academia.EmailContacto))
                missingFields.Add("EmailContacto");

            if (string.IsNullOrWhiteSpace(academia.Pais))
                missingFields.Add("Pais");

            if (string.IsNullOrWhiteSpace(academia.Ciudad))
                missingFields.Add("Ciudad");

            if (string.IsNullOrWhiteSpace(academia.IdFiscal))
                missingFields.Add("IdFiscal");

            // LogoUrl es opcional, pero UrlSitioWeb es obligatorio
            if (string.IsNullOrWhiteSpace(academia.UrlSitioWeb))
                missingFields.Add("UrlSitioWeb");

            // Lanzar excepción si hay campos faltantes
            if (missingFields.Count > 0)
                throw new AcademiaIncompleteDataException(academia.Codigo, missingFields.ToArray());
        }

        /// <summary>
        /// Convierte una entidad AcademiaConfig a un DTO de datos iniciales.
        /// Solo incluye los campos necesarios para renderizar el login.
        /// </summary>
        private AcademiaInitialDataDto MapToInitialDataDto(AcademiaConfig academia)
        {
            return new AcademiaInitialDataDto
            {
                Id = academia.Id,
                Codigo = academia.Codigo,
                Nombre = academia.Nombre,
                LogoUrl = academia.LogoUrl,
                EsDemo = academia.EsDemo,
                Datos = new AcademiaConfigDataDto
                {
                    Codigo = academia.Codigo,
                    Nombre = academia.Nombre,
                    LogoUrl = academia.LogoUrl,
                    Descripcion = academia.Descripcion ?? string.Empty,
                    Direccion = academia.Direccion ?? string.Empty,
                    Telefono = academia.Telefono ?? string.Empty,
                    EmailContacto = academia.EmailContacto ?? string.Empty,
                    Pais = academia.Pais ?? string.Empty,
                    Ciudad = academia.Ciudad ?? string.Empty,
                    IdFiscal = academia.IdFiscal,
                    UrlSitioWeb = academia.UrlSitioWeb,
                    EsDemo = academia.EsDemo,
                    Activo = academia.Activo
                }
            };
        }
    }
}
