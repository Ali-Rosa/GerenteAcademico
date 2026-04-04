namespace GerenteAcademico.Domain.Entities
{
    /// <summary>
    /// Representa un usuario registrado en la academia.
    /// Cada usuario pertenece a un Rol que determina sus permisos en el sistema.
    /// </summary>
    public class Usuario
    {
        /// <summary>
        /// Identificador único del usuario.
        /// </summary>
        public int Id { get; set; }

        // ========== IDENTIFICACIÓN ==========
        /// <summary>
        /// Nombre de usuario para login (único en la academia).
        /// Ejemplo: "juan.perez", "jperez123"
        /// </summary>
        public string Username { get; set; } = default!;

        /// <summary>
        /// Email del usuario (único en la academia).
        /// Se usa para recuperación de contraseña y notificaciones.
        /// </summary>
        public string Email { get; set; } = default!;

        // ========== AUTENTICACIÓN ==========
        /// <summary>
        /// Hash de la contraseña (nunca se almacena texto plano).
        /// Se genera usando BCrypt o similar.
        /// </summary>
        public string PasswordHash { get; set; } = default!;

        /// <summary>
        /// Salt adicional para el hash (opcional, depende del algoritmo de hash).
        /// </summary>
        public string? PasswordSalt { get; set; }

        // ========== INFORMACIÓN BÁSICA ==========
        /// <summary>
        /// Nombre del usuario.
        /// </summary>
        public string Nombre { get; set; } = default!;

        /// <summary>
        /// Apellido del usuario (opcional).
        /// </summary>
        public string? Apellido { get; set; }

        /// <summary>
        /// Teléfono de contacto principal del usuario.
        /// Requerido.
        /// </summary>
        public string Telefono { get; set; } = default!;

        /// <summary>
        /// Teléfono de emergencia (opcional).
        /// </summary>
        public string? TelefonoEmergencia { get; set; }

        // ========== DOCUMENTACIÓN (OBLIGATORIO) ==========
        /// <summary>
        /// Documento de identificación del usuario (DNI, Pasaporte, Cédula, etc.).
        /// REQUERIDO - No puede ser null.
        /// Debe ser único en la academia.
        /// </summary>
        public string Documentacion { get; set; } = default!;

        /// <summary>
        /// Tipo de documentación (ej: "DNI", "Pasaporte", "Cédula de Identidad").
        /// Opcional pero recomendado para clasificación.
        /// </summary>
        public string? TipoDocumentacion { get; set; }

        // ========== INFORMACIÓN PERSONAL ==========
        /// <summary>
        /// Fecha de nacimiento del usuario (opcional).
        /// </summary>
        public DateTime? FechaNacimiento { get; set; }

        /// <summary>
        /// Género del usuario (ej: "M", "F", "Otro").
        /// Opcional.
        /// </summary>
        public string? Genero { get; set; }

        /// <summary>
        /// Nacionalidad del usuario (opcional).
        /// </summary>
        public string? Nacionalidad { get; set; }

        /// <summary>
        /// Dirección de residencia del usuario (opcional).
        /// </summary>
        public string? Direccion { get; set; }

        // ========== FOTO/AVATAR ==========
        /// <summary>
        /// URL o nombre del archivo de foto del usuario.
        /// Se almacena en wwwroot/Fotos/
        /// Ejemplo: "usuario-123.jpg"
        /// Opcional.
        /// </summary>
        public string? FotoUrl { get; set; }

        // ========== ROLES Y PERMISOS ==========
        /// <summary>
        /// Identificador del rol asignado al usuario (Foreign Key).
        /// El rol determina los permisos del usuario en el sistema.
        /// </summary>
        public int RolId { get; set; }

        /// <summary>
        /// Referencia al objeto Rol del usuario.
        /// Navegación para acceder a los datos del rol.
        /// </summary>
        public Rol Rol { get; set; } = default!;

        // ========== ESTADO ==========
        /// <summary>
        /// Indica si el usuario está activo y puede acceder al sistema.
        /// Si es false, el usuario no puede loguearse.
        /// </summary>
        public bool Activo { get; set; } = true;

        /// <summary>
        /// Indica si el email del usuario ha sido verificado.
        /// Se establece en true cuando el usuario valida su email.
        /// </summary>
        public bool EmailVerificado { get; set; } = false;

        // ========== AUDITORÍA ==========
        /// <summary>
        /// Fecha de creación del usuario.
        /// </summary>
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Última fecha de modificación del usuario.
        /// </summary>
        public DateTime? FechaModificacion { get; set; }

        /// <summary>
        /// Usuario o sistema que creó este usuario.
        /// </summary>
        public string UsuarioCreacion { get; set; } = "system";

        /// <summary>
        /// Usuario que realizó la última modificación.
        /// </summary>
        public string? UsuarioModificacion { get; set; }

        // ========== SEGURIDAD ==========
        /// <summary>
        /// Fecha y hora del último login exitoso.
        /// Útil para análisis de actividad.
        /// </summary>
        public DateTime? UltimoLogin { get; set; }

        /// <summary>
        /// Contador de intentos de login fallidos.
        /// Se incrementa con cada intento fallido.
        /// Se reinicia a 0 cuando el login es exitoso.
        /// Se usa para bloquear después de X intentos por seguridad.
        /// </summary>
        public int IntentosFallidos { get; set; } = 0;
    }
}
