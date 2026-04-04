namespace GerenteAcademico.Application.Dtos.Usuarios
{
    /// <summary>
    /// DTO para crear o actualizar un usuario
    /// </summary>
    public class CreateUpdateUsuarioDto
    {
        public string Nombre { get; set; } = default!;
        public string? Apellido { get; set; }
        public string Email { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string? Password { get; set; }
        public string Documentacion { get; set; } = default!;
        public string? TipoDocumentacion { get; set; }
        public string Telefono { get; set; } = default!;
        public string? TelefonoEmergencia { get; set; }
        public string? Direccion { get; set; }
        public string? Genero { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int RolId { get; set; }
        public bool Activo { get; set; } = true;
    }

    /// <summary>
    /// DTO para retornar datos del usuario
    /// </summary>
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Apellido { get; set; }
        public string Email { get; set; } = default!;
        public string Username { get; set; } = default!;
        public string Documentacion { get; set; } = default!;
        public string? TipoDocumentacion { get; set; }
        public string Telefono { get; set; } = default!;
        public string? TelefonoEmergencia { get; set; }
        public string? Direccion { get; set; }
        public string? Genero { get; set; }
        public string? Nacionalidad { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string? FotoUrl { get; set; }
        public int RolId { get; set; }
        public string? RolNombre { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
