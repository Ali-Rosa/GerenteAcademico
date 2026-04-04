using GerenteAcademico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Services
{
    /// <summary>
    /// Factory para crear instancias de AcademiaDbContext con cadena de conexión dinámica
    /// </summary>
    public interface IAcademiaDbContextFactory
    {
        /// <summary>
        /// Crea un contexto de academia con la cadena de conexión validada
        /// </summary>
        /// <param name="academiaCodigo">Código de la academia</param>
        /// <returns>Contexto de academia, null si no se puede crear</returns>
        Task<AcademiaDbContext?> CreateContextAsync(string academiaCodigo);

        /// <summary>
        /// Crea un contexto de academia con la cadena de conexión sin validación
        /// </summary>
        Task<AcademiaDbContext?> CreateContextUncheckedAsync(string academiaCodigo);
    }

    public class AcademiaDbContextFactory : IAcademiaDbContextFactory
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly ILogger<AcademiaDbContextFactory> _logger;

        public AcademiaDbContextFactory(
            IConnectionStringProvider connectionStringProvider,
            ILogger<AcademiaDbContextFactory> logger)
        {
            _connectionStringProvider = connectionStringProvider;
            _logger = logger;
        }

        /// <summary>
        /// Crea un contexto con cadena de conexión validada
        /// </summary>
        public async Task<AcademiaDbContext?> CreateContextAsync(string academiaCodigo)
        {
            var connectionString = await _connectionStringProvider.GetConnectionStringAsync(academiaCodigo);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogError($"No se pudo obtener cadena de conexión validada para '{academiaCodigo}'");
                return null;
            }

            return CreateContext(connectionString);
        }

        /// <summary>
        /// Crea un contexto sin validar la cadena de conexión
        /// </summary>
        public async Task<AcademiaDbContext?> CreateContextUncheckedAsync(string academiaCodigo)
        {
            var connectionString = await _connectionStringProvider.GetConnectionStringUncheckedAsync(academiaCodigo);

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                _logger.LogWarning($"No se pudo obtener cadena de conexión para '{academiaCodigo}'");
                return null;
            }

            return CreateContext(connectionString);
        }

        /// <summary>
        /// Crea el contexto con la cadena de conexión especificada
        /// </summary>
        private AcademiaDbContext CreateContext(string connectionString)
        {
            try
            {
                var optionsBuilder = new DbContextOptionsBuilder<AcademiaDbContext>();
                optionsBuilder.UseSqlServer(connectionString);

                return new AcademiaDbContext(optionsBuilder.Options);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error al crear AcademiaDbContext: {ex.Message}");
                throw;
            }
        }
    }
}
