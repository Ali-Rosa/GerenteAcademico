namespace GerenteAcademico.Application.Dtos.Roles
{
    /// <summary>
    /// DTO para crear o actualizar un rol
    /// </summary>
    public class CreateUpdateRolDto
    {
        public string Nombre { get; set; } = default!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
