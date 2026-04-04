namespace GerenteAcademico.Application.Dtos
{
    /// <summary>
    /// DTO con los datos iniciales de una academia que se cargan al iniciar la aplicación.
    /// Solo contiene los datos necesarios para renderizar la página de login.
    /// </summary>
    public class AcademiaInitialDataDto
    {
        /// <summary>
        /// ID único de la academia.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Código único de la academia (ej: "ACAD-001").
        /// Se usa para identificar la academia por dominio.
        /// </summary>
        public string Codigo { get; set; } = string.Empty;

        /// <summary>
        /// Nombre oficial de la academia.
        /// Se renderiza en la página de login.
        /// </summary>
        public string Nombre { get; set; } = string.Empty;

        /// <summary>
        /// Nombre del archivo del logo (sin ruta completa).
        /// Ej: "logo-academia.png"
        /// Se busca en: wwwroot/Logos/
        /// Si está vacío, no se muestra logo.
        /// </summary>
        public string? LogoUrl { get; set; }

        /// <summary>
        /// Indica si es una academia de demostración.
        /// Puede afectar características disponibles.
        /// </summary>
        public bool EsDemo { get; set; }

        /// <summary>
        /// Todos los datos de la academia (completos).
        /// Se usa para validaciones internas.
        /// </summary>
        public AcademiaConfigDataDto Datos { get; set; } = new();
    }

    /// <summary>
    /// DTO con todos los datos de configuración de la academia.
    /// </summary>
    public class AcademiaConfigDataDto
    {
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? LogoUrl { get; set; }
        public string Descripcion { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;
        public string EmailContacto { get; set; } = string.Empty;
        public string Pais { get; set; } = string.Empty;
        public string Ciudad { get; set; } = string.Empty;
        public string IdFiscal { get; set; } = string.Empty;
        public string? UrlSitioWeb { get; set; }
        public bool EsDemo { get; set; }
        public bool Activo { get; set; }
    }
}
