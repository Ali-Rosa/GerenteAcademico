namespace GerenteAcademico.Application.Dtos.Roles
{
    /// <summary>
    /// DTO para retornar datos del rol
    /// </summary>
    public class RolDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public bool EsPredefinido { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }
    }
}
