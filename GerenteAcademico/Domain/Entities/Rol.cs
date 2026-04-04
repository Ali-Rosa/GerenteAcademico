namespace GerenteAcademico.Domain.Entities
{
    /// <summary>
    /// Define los roles disponibles en la academia.
    /// Ejemplo: Admin, Profesor, Estudiante, Supervisor, etc.
    /// 
    /// Los roles predefinidos (SuperUsuario, AdminMaster, Admin, Profesor, Estudiante, Supervisor)
    /// tienen EsPredefinido = true y NO pueden ser eliminados.
    /// </summary>
    public class Rol
    {
        /// <summary>
        /// Identificador único del rol.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Nombre único del rol (ej: "Admin", "Profesor", "Estudiante").
        /// </summary>
        public string Nombre { get; set; } = default!;

        /// <summary>
        /// Descripción del rol y sus responsabilidades.
        /// </summary>
        public string? Descripcion { get; set; }

        /// <summary>
        /// Indica si es un rol predefinido del sistema.
        /// Los roles predefinidos NO pueden ser eliminados por usuarios.
        /// Solo pueden ser modificados por SuperUsuario.
        /// </summary>
        public bool EsPredefinido { get; set; } = false;

        /// <summary>
        /// Indica si el rol está activo y disponible.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Fecha de creación del rol.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Última fecha de modificación del rol.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Usuario que creó el rol.
        /// </summary>
        public string UsuarioCreacion { get; set; } = "system";

        /// <summary>
        /// Usuario que modificó el rol.
        /// </summary>
        public string? UsuarioModificacion { get; set; }

        // ========== RELACIONES ==========
        /// <summary>
        /// Colección de usuarios que tienen este rol.
        /// </summary>
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
