using GerenteAcademico.Web.Services;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace GerenteAcademico.Web.Components.Pages
{
    public class DashboardBase : ComponentBase
    {
        [Parameter] public string? academia { get; set; }
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected AcademiaState AcademiaState { get; set; } = default!;
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected ILogger<DashboardBase> Logger { get; set; } = default!;

        protected string? NombreAcademia;
        protected string? LogoUrl;
        protected string? AcadiaCodigo;
        protected string? NombreUsuario;
        protected string? RolUsuario;
        protected string? Token;
        protected bool IsLoading = true;
        protected bool IsLoggingOut = false;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                // Cargar datos de la academia desde el estado
                NombreAcademia = AcademiaState.Nombre;
                LogoUrl = AcademiaState.LogoUrl;
                AcadiaCodigo = AcademiaState.Codigo;

                // Cargar datos del usuario desde el estado
                NombreUsuario = AcademiaState.NombreUsuario;
                RolUsuario = AcademiaState.RolUsuario;
                Token = AcademiaState.Token;

                // Validar que tenemos datos
                if (string.IsNullOrEmpty(NombreAcademia) || string.IsNullOrEmpty(NombreUsuario))
                {
                    Logger.LogWarning("Datos incompletos en AcademiaState. Redirigiendo a login.");
                    Navigation.NavigateTo($"/{academia}/login", false);
                    return;
                }

                IsLoading = false;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error al cargar el dashboard");
                IsLoading = false;
            }
        }

        /// <summary>
        /// Verifica con el servidor que la sesión es válida (basado en cookies HTTP-only)
        /// </summary>
        private async Task<bool> VerifySessionAsync()
        {
            try
            {
                var response = await Http.GetAsync("api/auth/check");
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error verificando sesión");
                return false;
            }
        }

        /// <summary>
        /// Cierra la sesión del usuario de forma segura
        /// </summary>
        protected async Task DoLogout()
        {
            IsLoggingOut = true;

            try
            {
                // Llamar al endpoint de logout para limpiar la cookie
                var response = await Http.PostAsync("api/auth/logout", null);

                if (response.IsSuccessStatusCode)
                {
                    Logger.LogInformation("Logout exitoso para usuario: {Usuario}", NombreUsuario);
                }
                else
                {
                    Logger.LogWarning("Logout devolvió status: {Status}", response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error durante logout");
                // Continuar de todos modos
            }
            finally
            {
                // Limpiar estado local y redirigir al login
                // Usar 'true' para recargar la página y asegurarse de limpiar todo
                Navigation.NavigateTo($"/{academia}/login", true);
            }
        }
    }
}
