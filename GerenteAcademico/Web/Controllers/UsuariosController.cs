using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GerenteAcademico.Domain.Entities;
using GerenteAcademico.Domain.Interfaces;
using GerenteAcademico.Infrastructure.Services;
using GerenteAcademico.Application.Dtos.Usuarios;
using System.Security.Cryptography;
using System.Text;

namespace GerenteAcademico.Web.Controllers
{
    /// <summary>
    /// Controlador API para gestionar usuarios de la academia
    /// </summary>
    [ApiController]
    [Route("{academia}/api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IAcademiaDbContextFactory _contextFactory;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(
            IConnectionStringProvider connectionStringProvider,
            IAcademiaDbContextFactory contextFactory,
            ILogger<UsuariosController> logger)
        {
            _connectionStringProvider = connectionStringProvider;
            _contextFactory = contextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los usuarios de la academia actual
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<UsuarioDto>>> GetAll()
        {
            try
            {
                var academiaCodigo = GetAcademiaCodigoFromPath();
                if (string.IsNullOrEmpty(academiaCodigo))
                    return BadRequest("No se pudo obtener el código de la academia");

                var context = await _contextFactory.CreateContextAsync(academiaCodigo);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var usuarios = context.Usuarios
                    .Include(u => u.Rol)
                    .AsNoTracking()
                    .ToList()
                    .Select(u => MapToDto(u))
                    .ToList();

                return Ok(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios");
                return StatusCode(500, new { message = "Error al obtener usuarios", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un usuario específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> GetById(int id)
        {
            try
            {
                var academiaCodigo = GetAcademiaCodigoFromPath();
                if (string.IsNullOrEmpty(academiaCodigo))
                    return BadRequest("No se pudo obtener el código de la academia");

                var context = await _contextFactory.CreateContextAsync(academiaCodigo);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var usuario = context.Usuarios
                    .Include(u => u.Rol)
                    .AsNoTracking()
                    .FirstOrDefault(u => u.Id == id);

                if (usuario == null)
                    return NotFound($"Usuario con ID {id} no encontrado");

                return Ok(MapToDto(usuario));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuario {UsuarioId}", id);
                return StatusCode(500, new { message = "Error al obtener usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo usuario
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<UsuarioDto>> Create(string academia, [FromBody] CreateUpdateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                // Validaciones
                if (string.IsNullOrWhiteSpace(dto.Nombre) || string.IsNullOrWhiteSpace(dto.Email) ||
                    string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Documentacion) ||
                    string.IsNullOrWhiteSpace(dto.Password) || dto.RolId <= 0)
                {
                    return BadRequest("Todos los campos requeridos deben ser completados");
                }

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                // Verificar si el usuario ya existe
                var existingUser = await context.Usuarios
                    .FirstOrDefaultAsync(u => u.Username == dto.Username || u.Email == dto.Email);

                if (existingUser != null)
                    return BadRequest("El nombre de usuario o email ya está registrado");

                // Crear nuevo usuario
                var usuario = new Usuario
                {
                    Nombre = dto.Nombre.Trim(),
                    Apellido = dto.Apellido?.Trim(),
                    Email = dto.Email.Trim(),
                    Username = dto.Username.Trim(),
                    PasswordHash = HashPassword(dto.Password),
                    Documentacion = dto.Documentacion.Trim(),
                    TipoDocumentacion = dto.TipoDocumentacion?.Trim(),
                    Telefono = dto.Telefono.Trim(),
                    TelefonoEmergencia = dto.TelefonoEmergencia?.Trim(),
                    Direccion = dto.Direccion?.Trim(),
                    Genero = dto.Genero?.Trim(),
                    Nacionalidad = dto.Nacionalidad?.Trim(),
                    FechaNacimiento = dto.FechaNacimiento,
                    RolId = dto.RolId,
                    Activo = dto.Activo,
                    FechaCreacion = DateTime.UtcNow
                };

                context.Usuarios.Add(usuario);
                await context.SaveChangesAsync();

                // Recargar para obtener el rol
                var createdUser = await context.Usuarios
                    .Include(u => u.Rol)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Id == usuario.Id);

                return CreatedAtAction(nameof(GetById), new { academia, id = usuario.Id }, MapToDto(createdUser!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear usuario para academia {Academia}", academia);
                return StatusCode(500, new { message = "Error al crear usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un usuario existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<UsuarioDto>> Update(int id, [FromBody] CreateUpdateUsuarioDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var academiaCodigo = GetAcademiaCodigoFromPath();
                if (string.IsNullOrEmpty(academiaCodigo))
                    return BadRequest("No se pudo obtener el código de la academia");

                var context = await _contextFactory.CreateContextAsync(academiaCodigo);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var usuario = await Task.FromResult(context.Usuarios.FirstOrDefault(u => u.Id == id));
                if (usuario == null)
                    return NotFound($"Usuario con ID {id} no encontrado");

                // Verificar si el email o username ya están en uso por otro usuario
                var duplicate = await Task.FromResult(
                    context.Usuarios.FirstOrDefault(u => 
                        (u.Username == dto.Username || u.Email == dto.Email) && u.Id != id)
                );

                if (duplicate != null)
                    return BadRequest("El nombre de usuario o email ya está registrado");

                // Actualizar campos
                usuario.Nombre = dto.Nombre.Trim();
                usuario.Apellido = dto.Apellido?.Trim();
                usuario.Email = dto.Email.Trim();
                usuario.Username = dto.Username.Trim();
                usuario.Documentacion = dto.Documentacion.Trim();
                usuario.TipoDocumentacion = dto.TipoDocumentacion?.Trim();
                usuario.Telefono = dto.Telefono.Trim();
                usuario.TelefonoEmergencia = dto.TelefonoEmergencia?.Trim();
                usuario.Direccion = dto.Direccion?.Trim();
                usuario.Genero = dto.Genero?.Trim();
                usuario.Nacionalidad = dto.Nacionalidad?.Trim();
                usuario.FechaNacimiento = dto.FechaNacimiento;
                usuario.RolId = dto.RolId;
                usuario.Activo = dto.Activo;
                usuario.FechaModificacion = DateTime.UtcNow;

                // Actualizar contraseña si se proporciona
                if (!string.IsNullOrWhiteSpace(dto.Password))
                {
                    usuario.PasswordHash = HashPassword(dto.Password);
                }

                context.Usuarios.Update(usuario);
                await context.SaveChangesAsync();

                // Recargar para obtener el rol actualizado
                var updatedUser = await Task.FromResult(
                    context.Usuarios
                        .Include(u => u.Rol)
                        .AsNoTracking()
                        .FirstOrDefault(u => u.Id == id)
                );

                return Ok(MapToDto(updatedUser!));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar usuario {UsuarioId}", id);
                return StatusCode(500, new { message = "Error al actualizar usuario", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un usuario
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            try
            {
                var academiaCodigo = GetAcademiaCodigoFromPath();
                if (string.IsNullOrEmpty(academiaCodigo))
                    return BadRequest("No se pudo obtener el código de la academia");

                var context = await _contextFactory.CreateContextAsync(academiaCodigo);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var usuario = await Task.FromResult(context.Usuarios.FirstOrDefault(u => u.Id == id));
                if (usuario == null)
                    return NotFound($"Usuario con ID {id} no encontrado");

                context.Usuarios.Remove(usuario);
                await context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar usuario {UsuarioId}", id);
                return StatusCode(500, new { message = "Error al eliminar usuario", error = ex.Message });
            }
        }

        private UsuarioDto MapToDto(Usuario usuario) =>
            new()
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Apellido = usuario.Apellido,
                Email = usuario.Email,
                Username = usuario.Username,
                Documentacion = usuario.Documentacion,
                TipoDocumentacion = usuario.TipoDocumentacion,
                Telefono = usuario.Telefono,
                TelefonoEmergencia = usuario.TelefonoEmergencia,
                Direccion = usuario.Direccion,
                Genero = usuario.Genero,
                Nacionalidad = usuario.Nacionalidad,
                FechaNacimiento = usuario.FechaNacimiento,
                FotoUrl = usuario.FotoUrl,
                RolId = usuario.RolId,
                RolNombre = usuario.Rol?.Nombre,
                Activo = usuario.Activo,
                FechaCreacion = usuario.FechaCreacion,
                FechaModificacion = usuario.FechaModificacion
            };

        private static string HashPassword(string password) =>
            BCrypt.Net.BCrypt.HashPassword(password);

        private string? GetAcademiaCodigoFromPath()
        {
            // Extraer el código de la academia desde la URL: /academia/api/usuarios
            var path = HttpContext.Request.Path.ToString();
            var segments = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);

            return segments.Length > 0 ? segments[0] : null;
        }
    }
}
