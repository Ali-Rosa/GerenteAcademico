using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GerenteAcademico.Domain.Entities;
using GerenteAcademico.Application.Dtos.Roles;
using GerenteAcademico.Infrastructure.Services;

namespace GerenteAcademico.Web.Controllers
{
    /// <summary>
    /// Controlador API para gestionar los roles disponibles
    /// </summary>
    [ApiController]
    [Route("{academia}/api/[controller]")]
    public class RolesController : ControllerBase
    {
        private readonly IConnectionStringProvider _connectionStringProvider;
        private readonly IAcademiaDbContextFactory _contextFactory;
        private readonly ILogger<RolesController> _logger;

        public RolesController(
            IConnectionStringProvider connectionStringProvider,
            IAcademiaDbContextFactory contextFactory,
            ILogger<RolesController> logger)
        {
            _connectionStringProvider = connectionStringProvider;
            _contextFactory = contextFactory;
            _logger = logger;
        }

        /// <summary>
        /// Obtiene todos los roles disponibles en la academia
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<RolDto>>> GetAll(string academia)
        {
            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var roles = await context.Roles
                    .AsNoTracking()
                    .OrderBy(r => r.Nombre)
                    .Select(r => new RolDto
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        Descripcion = r.Descripcion,
                        EsPredefinido = r.EsPredefinido,
                        Activo = r.Activo,
                        FechaCreacion = r.FechaCreacion,
                        FechaModificacion = r.FechaModificacion
                    })
                    .ToListAsync();

                return Ok(roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener roles para academia {Academia}", academia);
                return StatusCode(500, new { message = "Error al obtener roles", error = ex.Message });
            }
        }

        /// <summary>
        /// Obtiene un rol específico por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RolDto>> GetById(string academia, int id)
        {
            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var rol = await context.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (rol == null)
                    return NotFound($"Rol con ID {id} no encontrado");

                return Ok(new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre,
                    Descripcion = rol.Descripcion,
                    EsPredefinido = rol.EsPredefinido,
                    Activo = rol.Activo,
                    FechaCreacion = rol.FechaCreacion,
                    FechaModificacion = rol.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener rol {RolId} para academia {Academia}", id, academia);
                return StatusCode(500, new { message = "Error al obtener rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Crea un nuevo rol
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RolDto>> Create(string academia, [FromBody] CreateUpdateRolDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("El nombre del rol es requerido");

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                // Validar que no exista un rol con el mismo nombre
                var existingRol = await context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre.ToLower() == dto.Nombre.ToLower());

                if (existingRol != null)
                    return BadRequest($"Ya existe un rol con el nombre '{dto.Nombre}'");

                var rol = new Rol
                {
                    Nombre = dto.Nombre,
                    Descripcion = dto.Descripcion,
                    Activo = dto.Activo,
                    EsPredefinido = false,
                    FechaCreacion = DateTime.UtcNow
                };

                context.Roles.Add(rol);
                await context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetById), new { academia, id = rol.Id }, new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre,
                    Descripcion = rol.Descripcion,
                    EsPredefinido = rol.EsPredefinido,
                    Activo = rol.Activo,
                    FechaCreacion = rol.FechaCreacion,
                    FechaModificacion = rol.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear rol para academia {Academia}", academia);
                return StatusCode(500, new { message = "Error al crear rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Actualiza un rol existente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<RolDto>> Update(string academia, int id, [FromBody] CreateUpdateRolDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                if (string.IsNullOrWhiteSpace(dto.Nombre))
                    return BadRequest("El nombre del rol es requerido");

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var rol = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (rol == null)
                    return NotFound($"Rol con ID {id} no encontrado");

                // No permitir editar roles predefinidos
                if (rol.EsPredefinido)
                    return BadRequest("No se puede editar un rol predefinido del sistema");

                // Validar que no exista otro rol con el mismo nombre
                var existingRol = await context.Roles
                    .FirstOrDefaultAsync(r => r.Nombre.ToLower() == dto.Nombre.ToLower() && r.Id != id);

                if (existingRol != null)
                    return BadRequest($"Ya existe otro rol con el nombre '{dto.Nombre}'");

                rol.Nombre = dto.Nombre;
                rol.Descripcion = dto.Descripcion;
                rol.Activo = dto.Activo;
                rol.FechaModificacion = DateTime.UtcNow;

                context.Roles.Update(rol);
                await context.SaveChangesAsync();

                return Ok(new RolDto
                {
                    Id = rol.Id,
                    Nombre = rol.Nombre,
                    Descripcion = rol.Descripcion,
                    EsPredefinido = rol.EsPredefinido,
                    Activo = rol.Activo,
                    FechaCreacion = rol.FechaCreacion,
                    FechaModificacion = rol.FechaModificacion
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar rol {RolId} para academia {Academia}", id, academia);
                return StatusCode(500, new { message = "Error al actualizar rol", error = ex.Message });
            }
        }

        /// <summary>
        /// Elimina un rol
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string academia, int id)
        {
            try
            {
                if (string.IsNullOrEmpty(academia))
                    return BadRequest("El código de la academia es requerido");

                var context = await _contextFactory.CreateContextAsync(academia);
                if (context == null)
                    return NotFound("No se pudo obtener la conexión a la academia");

                var rol = await context.Roles.FirstOrDefaultAsync(r => r.Id == id);
                if (rol == null)
                    return NotFound($"Rol con ID {id} no encontrado");

                // No permitir eliminar roles predefinidos
                if (rol.EsPredefinido)
                    return BadRequest("No se puede eliminar un rol predefinido del sistema");

                // Validar que siempre exista al menos un SuperUsuario
                if (rol.Nombre == "SuperUsuario")
                {
                    var superusuarios = await context.Roles
                        .Where(r => r.Nombre == "SuperUsuario")
                        .CountAsync();

                    if (superusuarios <= 1)
                        return BadRequest("Debe existir al menos un rol SuperUsuario en el sistema");
                }

                // Verificar que no haya usuarios asignados a este rol
                var usuariosConRol = await context.Usuarios
                    .CountAsync(u => u.RolId == id);

                if (usuariosConRol > 0)
                    return BadRequest($"No se puede eliminar el rol porque hay {usuariosConRol} usuario(s) asignado(s)");

                context.Roles.Remove(rol);
                await context.SaveChangesAsync();

                return Ok(new { message = "Rol eliminado exitosamente" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar rol {RolId} para academia {Academia}", id, academia);
                return StatusCode(500, new { message = "Error al eliminar rol", error = ex.Message });
            }
        }

        private string? GetAcademiaCodigoFromPath()
        {
            // Extraer el código de la academia desde la URL: /academia/api/roles
            var path = HttpContext.Request.Path.ToString();
            var segments = path.Split('/', System.StringSplitOptions.RemoveEmptyEntries);

            return segments.Length > 0 ? segments[0] : null;
        }
    }
}
