namespace GerenteAcademico.Web.Services
{
    public class AuthService
    {
        private readonly HttpClient _http;

        public AuthService(HttpClient http)
        {
            _http = http;
        }

        /// <summary>
        /// Verifica si el usuario tiene una sesión autenticada válida
        /// </summary>
        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                // Intenta hacer una llamada al servidor para verificar si la sesión es válida
                var response = await _http.GetAsync("api/auth/check");
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
