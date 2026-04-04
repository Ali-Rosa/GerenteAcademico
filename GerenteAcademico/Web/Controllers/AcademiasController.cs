using GerenteAcademico.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace GerenteAcademico.Web.Controllers
{
    /// <summary>
    /// API Controller para acceso a datos de academias.
    /// NOTA: Solo operaciones de LECTURA y VALIDACIÓN.
    /// Las academias se crean manualmente en la BD, no desde esta API.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AcademiasController : ControllerBase
    {
        private readonly AcademiaService _service;

        public AcademiasController(AcademiaService service)
        {
            _service = service;
        }

        /// <summary>
        /// Obtiene TODOS los datos de todas las academias.
        /// Solo para propósitos administrativos (back-office).
        /// 
        /// GET /api/academias
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var academias = await _service.GetAllAsync();
            return Ok(academias);
        }

        /// <summary>
        /// Obtiene una academia por su ID (numérico).
        /// 
        /// GET /api/academias/1
        /// </summary>
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var academia = await _service.GetByIdAsync(id);
            if (academia == null)
                return NotFound(new { message = "Academia no encontrada" });

            return Ok(academia);
        }

        /// <summary>
        /// Obtiene y valida una academia por su código.
        /// 
        /// Valida que:
        /// - La academia existe
        /// - La academia está activa
        /// - Tiene todos los datos obligatorios
        /// 
        /// Se usa en el login inicial para cargar datos de marca (nombre + logo).
        /// 
        /// GET /api/academias/codigo/ACAD-001
        /// </summary>
        [HttpGet("codigo/{codigo}")]
        public async Task<IActionResult> GetByCodigo(string codigo)
        {
            var academia = await _service.GetByCodigoAsync(codigo);
            if (academia == null)
                return NotFound(new { message = "Academia no encontrada" });

            return Ok(academia);
        }

        /// <summary>
        /// Obtiene y valida una academia para el login inicial.
        /// 
        /// Realiza validaciones completas:
        /// 1. ¿Existe la academia?
        /// 2. ¿Está activa?
        /// 3. ¿Tiene todos los datos obligatorios?
        /// 4. ¿Tiene conexión a su BD?
        /// 
        /// Retorna un DTO optimizado con solo los datos para renderizar login.
        /// 
        /// GET /api/academias/validate/ACAD-001
        /// 
        /// Respuestas posibles:
        /// - 200: Academia válida, lista para login
        /// - 404: Academia no existe
        /// - 403: Academia desactivada
        /// - 422: Academia existe pero le faltan datos obligatorios
        /// </summary>
        [HttpGet("validate/{codigo}")]
        public async Task<IActionResult> ValidateForLogin(string codigo)
        {
            var academiaData = await _service.GetAndValidateAsync(codigo);
            return Ok(academiaData);
        }
    }
}

