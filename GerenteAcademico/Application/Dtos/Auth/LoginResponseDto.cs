namespace GerenteAcademico.Application.Dtos.Auth
{
    public class LoginResponseDto
    {
        public string NombreUsuario { get; set; } = "";
        public string Rol { get; set; } = "";
        public string? FotoUrl { get; set; }
        public string Token { get; set; } = "";
    }
}
