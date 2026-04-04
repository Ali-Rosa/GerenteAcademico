namespace GerenteAcademico.Application.Dtos.Auth
{
    public class LoginRequestDto
    {
        public string AcademiaCodigo { get; set; } = "";
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }
}

