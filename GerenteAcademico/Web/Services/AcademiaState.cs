namespace GerenteAcademico.Web.Services
{
    public class AcademiaState
    {
        public string? Codigo { get; private set; }
        public string? Nombre { get; private set; }
        public string? LogoUrl { get; private set; }
        public string? ConnectionString { get; private set; }

        // Datos del usuario autenticado
        public string? NombreUsuario { get; private set; }
        public string? RolUsuario { get; private set; }
        public string? FotoUrl { get; private set; }
        public string? Token { get; private set; }

        public void SetData(string codigo, string nombre, string logoUrl, string connectionString)
        {
            Codigo = codigo;
            Nombre = nombre;
            LogoUrl = logoUrl;
            ConnectionString = connectionString;
        }

        public void SetUserData(string nombreUsuario, string rolUsuario, string? fotoUrl, string token)
        {
            NombreUsuario = nombreUsuario;
            RolUsuario = rolUsuario;
            FotoUrl = fotoUrl;
            Token = token;
        }

        public bool IsLoaded => Codigo is not null;
        public bool IsAuthenticated => !string.IsNullOrWhiteSpace(Token);
    }

}
