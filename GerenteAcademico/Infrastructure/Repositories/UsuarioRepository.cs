using GerenteAcademico.Domain.Entities;
using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Infrastructure.Persistence;
using GerenteAcademico.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;

namespace GerenteAcademico.Infrastructure.Repositories
{
    /// <summary>
    /// Repositorio de usuarios con contexto dinámico por academia.
    /// 
    /// IMPORTANTE: Este repositorio NO recibe un DbContext inyectado directamente.
    /// En su lugar, usa IAcademiaDbContextFactory para crear un contexto dinámico
    /// basado en el código de academia establecido con SetAcademiaContext().
    /// 
    /// RAZÓN: Cada academia tiene su propia base de datos con su propia tabla de usuarios.
    /// La cadena de conexión se obtiene desde Academias.CadenaConexionPrincipal en ConfigDB.
    /// 
    /// FLUJO:
    ///   1. AuthService.LoginAsync() llama SetAcademiaContext("Konektia")
    ///   2. GetByUsernameAsync() crea contexto dinámico para "Konektia"
    ///   3. ConnectionStringProvider obtiene la cadena de Academias
    ///   4. ConnectionStringValidator valida la cadena
    ///   5. Se crea AcademiaDbContext dinámicamente
    ///   6. Se busca el usuario en la BD de "Konektia"
    /// </summary>
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IAcademiaDbContextFactory _contextFactory;
        private readonly ILogger<UsuarioRepository> _logger;
        private string _academiaCodigo;

        public UsuarioRepository(
            IAcademiaDbContextFactory contextFactory,
            ILogger<UsuarioRepository> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }

        /// <summary>
        /// REQUERIDO: Establece el código de academia para las operaciones posteriores.
        /// 
        /// IMPORTANTE: Debe llamarse ANTES de GetByUsernameAsync() u UpdateAsync()
        /// 
        /// Ejemplo:
        ///   repository.SetAcademiaContext("Konektia");
        ///   var usuario = await repository.GetByUsernameAsync("admin");
        /// </summary>
        public void SetAcademiaContext(string academiaCodigo)
        {
            _academiaCodigo = academiaCodigo;
        }

        public async Task<Usuario?> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(_academiaCodigo))
            {
                _logger.LogWarning("Código de academia no establecido en UsuarioRepository");
                return null;
            }

            // Crear contexto dinámicamente con la cadena de conexión de la academia
            var context = await _contextFactory.CreateContextAsync(_academiaCodigo);
            if (context == null)
            {
                _logger.LogError($"No se pudo crear contexto para academia: {_academiaCodigo}");
                return null;
            }

            using (context)
            {
                return await context.Usuarios
                    .Include(u => u.Rol)
                    .FirstOrDefaultAsync(u => u.Username == username);
            }
        }

        public async Task UpdateAsync(Usuario usuario)
        {
            if (string.IsNullOrWhiteSpace(_academiaCodigo))
            {
                _logger.LogWarning("Código de academia no establecido en UsuarioRepository");
                return;
            }

            var context = await _contextFactory.CreateContextAsync(_academiaCodigo);
            if (context == null)
            {
                _logger.LogError($"No se pudo crear contexto para academia: {_academiaCodigo}");
                return;
            }

            using (context)
            {
                context.Usuarios.Update(usuario);
                await context.SaveChangesAsync();
            }
        }
    }
}
