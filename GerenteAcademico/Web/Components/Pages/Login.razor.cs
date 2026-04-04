using GerenteAcademico.Application.Dtos.Auth;
using GerenteAcademico.Web.Services;
using Microsoft.AspNetCore.Components;

namespace GerenteAcademico.Web.Components.Pages
{
    public class LoginBase : ComponentBase
    {
        [Parameter] public string? academia { get; set; }
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected NavigationManager Nav { get; set; } = default!;

        [Inject] protected AcademiaState AcademiaState { get; set; } = default!;

        protected string? NombreAcademia;
        protected string? LogoUrl;

        protected string Username = "";
        protected string Password = "";

        protected string? ErrorMessage; // Necesario para mostrar errores de login


        protected override void OnInitialized()
        {
            if (!AcademiaState.IsLoaded)
            {
                Nav.NavigateTo($"/{academia}", false);
                return;
            }

            NombreAcademia = AcademiaState.Nombre;
            LogoUrl = AcademiaState.LogoUrl;
        }



        protected async Task DoLogin()
        {
            var request = new LoginRequestDto
            {
                AcademiaCodigo = academia!,
                Username = Username,
                Password = Password
            };

            var response = await Http.PostAsJsonAsync("api/auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();

                if (loginResponse != null)
                {
                    // Guardar datos del usuario en el estado
                    AcademiaState.SetUserData(
                        loginResponse.NombreUsuario,
                        loginResponse.Rol,
                        loginResponse.FotoUrl,
                        loginResponse.Token
                    );
                }

                Nav.NavigateTo($"/{academia}/dashboard", true);
            }
            else
            {
                var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                ErrorMessage = error?["message"] ?? "Error al iniciar sesión.";
            }
        }

    }
}
