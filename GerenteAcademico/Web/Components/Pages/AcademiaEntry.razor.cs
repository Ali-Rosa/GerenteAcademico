using GerenteAcademico.Web.Services;
using Microsoft.AspNetCore.Components;

namespace GerenteAcademico.Web.Components.Pages
{
    public class AcademiaEntryBase : ComponentBase
    {
        [Inject] protected NavigationManager Navigation { get; set; } = default!;
        [Inject] protected HttpClient Http { get; set; } = default!;
        [Inject] protected AcademiaState AcademiaState { get; set; } = default!;
        [Inject] protected AuthService AuthService { get; set; } = default!;

        [Parameter] public string? academia { get; set; }

        private bool _initialized = false;

        protected override async Task OnInitializedAsync()
        {
            // Evitar ejecutar múltiples veces
            if (_initialized) return;
            _initialized = true;

            await LoadAcademiaData();
        }

        protected async Task LoadAcademiaData()
        {
            if (string.IsNullOrWhiteSpace(academia))
            {
                Navigation.NavigateTo("/", false);
                return;
            }

            try
            {
                var result = await Http.GetFromJsonAsync<AcademiaInitialDataDto>(
                    $"api/academias/validate/{academia}"
                );

                if (result is null)
                {
                    Navigation.NavigateTo("/", false);
                    return;
                }

                AcademiaState.SetData(
                    academia!,
                    result.Nombre,
                    $"/logo/{result.LogoUrl}",
                    result.ConnectionString
                );

                // Verificar si el usuario ya está autenticado
                var isAuthenticated = await AuthService.IsAuthenticatedAsync();

                if (isAuthenticated)
                {
                    Navigation.NavigateTo($"/{academia}/dashboard", true);
                }
                else
                {
                    Navigation.NavigateTo($"/{academia}/login", true);
                }


            }
            catch
            {
                Navigation.NavigateTo("/", false);
            }
        }

        public class AcademiaInitialDataDto
        {
            public string Nombre { get; set; } = "";
            public string LogoUrl { get; set; } = "";
            public string ConnectionString { get; set; } = "";
        }
    }
}
