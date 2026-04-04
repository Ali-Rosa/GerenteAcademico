namespace GerenteAcademico.Domain.Entities
{
    public class AcademiaConfig
    {
        public int Id { get; set; }
        public string Codigo { get; set; } = default!;
        public string Nombre { get; set; } = default!;
        public string? LogoUrl { get; set; }
        public string CadenaConexionPrincipal { get; set; } = default!;
        public string? Descripcion { get; set; }
        public string? Direccion { get; set; }
        public string? Telefono { get; set; }
        public string? EmailContacto { get; set; }
        public string? Pais { get; set; }
        public string? Ciudad { get; set; }
        public string IdFiscal { get; set; } = default!;
        public string? UrlSitioWeb { get; set; }
        public bool EsDemo { get; set; } = true;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaModificacion { get; set; }
        public string UsuarioCreacion { get; set; } = "system";
        public string? UsuarioModificacion { get; set; }
    }
}
