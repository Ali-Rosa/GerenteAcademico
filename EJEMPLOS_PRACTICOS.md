# 📚 Ejemplos Prácticos - Cadenas de Conexión Dinámicas

## Ejemplo 1: Validar una Cadena de Conexión

### Usando cURL

```bash
# Test positivo - Cadena válida
curl -X POST https://localhost:7237/api/conexiones/validar \
  -H "Content-Type: application/json" \
  -d '{
    "connectionString": "Server=db42639.public.databaseasp.net;Database=db42639;User Id=db42639;Password=Zp6!z9=A7@We;Encrypt=True;TrustServerCertificate=True;",
    "timeoutSeconds": 5
  }' \
  -v

# Response (200 OK):
# {
#   "success": true,
#   "message": "Conexión válida",
#   "servidor": "db42639.public.databaseasp.net",
#   "baseDatos": "db42639",
#   "usuario": "db42639"
# }
```

### Usando C# HttpClient

```csharp
public class ConnectionValidator
{
    private readonly HttpClient _httpClient;

    public ConnectionValidator(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<bool> ValidateConnectionAsync(string connectionString)
    {
        var request = new { connectionString, timeoutSeconds = 5 };
        
        var response = await _httpClient.PostAsJsonAsync(
            "api/conexiones/validar", 
            request
        );

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsAsync<dynamic>();
            return result.success == true;
        }

        return false;
    }
}
```

### Usando JavaScript/Fetch

```javascript
async function validarConexion(connectionString) {
    const response = await fetch('/api/conexiones/validar', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify({
            connectionString: connectionString,
            timeoutSeconds: 5
        })
    });

    const data = await response.json();
    
    if (data.success) {
        console.log('Servidor:', data.servidor);
        console.log('Base de datos:', data.baseDatos);
        console.log('Usuario:', data.usuario);
        return true;
    } else {
        console.error('Error:', data.error);
        return false;
    }
}

// Uso
const connStr = "Server=...;Database=...;";
const esValida = await validarConexion(connStr);
```

---

## Ejemplo 2: Obtener Cadena de Conexión de Academia

### Usando HttpClient Inyectado

```csharp
[Inject] 
protected HttpClient Http { get; set; }

public async Task ObtenerConexionAcademiaAsync(string academiaCodigo)
{
    try
    {
        var response = await Http.GetAsync($"api/conexiones/academia/{academiaCodigo}");
        
        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsAsync<ConexionAcademiaResponse>();
            
            Console.WriteLine($"Academia: {data.Academia}");
            Console.WriteLine($"Servidor: {data.Servidor}");
            Console.WriteLine($"BD: {data.BaseDatos}");
            Console.WriteLine($"Configurada: {data.CadenaConfigurada}");
        }
        else
        {
            Console.WriteLine("No se encontró la academia o conexión inválida");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

public class ConexionAcademiaResponse
{
    public bool Success { get; set; }
    public string Academia { get; set; }
    public string Servidor { get; set; }
    public string BaseDatos { get; set; }
    public string Usuario { get; set; }
    public bool CadenaConfigurada { get; set; }
}
```

### En un Componente Blazor

```razor
@page "/conexiones/{academiaCodigo}"
@inject HttpClient Http

<div class="container mt-5">
    <h1>Estado de Conexión: @academiaCodigo</h1>

    @if (isLoading)
    {
        <div class="spinner-border"></div>
    }
    else if (conexion != null)
    {
        <div class="alert alert-success">
            <h4>✓ Conexión Válida</h4>
            <table class="table">
                <tr>
                    <td><strong>Academia:</strong></td>
                    <td>@conexion.Academia</td>
                </tr>
                <tr>
                    <td><strong>Servidor:</strong></td>
                    <td>@conexion.Servidor</td>
                </tr>
                <tr>
                    <td><strong>Base de datos:</strong></td>
                    <td>@conexion.BaseDatos</td>
                </tr>
                <tr>
                    <td><strong>Usuario:</strong></td>
                    <td>@conexion.Usuario</td>
                </tr>
            </table>
        </div>
    }
    else
    {
        <div class="alert alert-danger">
            <h4>✗ Conexión Inválida</h4>
            <p>@errorMessage</p>
        </div>
    }
</div>

@code {
    [Parameter]
    public string academiaCodigo { get; set; }

    private bool isLoading = true;
    private ConexionAcademiaResponse conexion;
    private string errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await CargarConexionAsync();
    }

    private async Task CargarConexionAsync()
    {
        try
        {
            var response = await Http.GetAsync($"api/conexiones/academia/{academiaCodigo}");
            
            if (response.IsSuccessStatusCode)
            {
                conexion = await response.Content.ReadAsAsync<ConexionAcademiaResponse>();
            }
            else
            {
                var error = await response.Content.ReadAsAsync<ErrorResponse>();
                errorMessage = error.Error;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }
}
```

---

## Ejemplo 3: Usar el ConnectionStringProvider en un Servicio

### Obtener Conexión Dinámicamente

```csharp
public class AcademiaDataService
{
    private readonly IConnectionStringProvider _connProvider;
    private readonly IAcademiaDbContextFactory _contextFactory;
    private readonly ILogger<AcademiaDataService> _logger;

    public AcademiaDataService(
        IConnectionStringProvider connProvider,
        IAcademiaDbContextFactory contextFactory,
        ILogger<AcademiaDataService> logger)
    {
        _connProvider = connProvider;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    /// <summary>
    /// Obtiene todos los usuarios de una academia
    /// </summary>
    public async Task<List<Usuario>> GetUsuariosPorAcademiaAsync(string academiaCodigo)
    {
        // Opción 1: Con validación
        var context = await _contextFactory.CreateContextAsync(academiaCodigo);
        
        if (context == null)
        {
            _logger.LogError($"No se pudo crear contexto para {academiaCodigo}");
            return new List<Usuario>();
        }

        try
        {
            using (context)
            {
                return await context.Usuarios
                    .Include(u => u.Rol)
                    .Where(u => u.Activo)
                    .ToListAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error al obtener usuarios: {ex.Message}");
            return new List<Usuario>();
        }
    }

    /// <summary>
    /// Verifica si una academia tiene conexión válida
    /// </summary>
    public async Task<(bool isValid, string message)> VerificarConexionAcademiaAsync(string academiaCodigo)
    {
        var (connString, validationResult) = 
            await _connProvider.GetConnectionStringWithValidationAsync(academiaCodigo);

        if (validationResult.IsValid)
        {
            return (true, $"Conexión válida a {validationResult.Server}.{validationResult.Database}");
        }
        else
        {
            return (false, $"Error: {validationResult.ErrorMessage}");
        }
    }

    /// <summary>
    /// Obtiene información de la academia sin validar (desarrollo)
    /// </summary>
    public async Task<string> GetConnectionStringUncheckedAsync(string academiaCodigo)
    {
        return await _connProvider.GetConnectionStringUncheckedAsync(academiaCodigo);
    }
}
```

### Usar el Servicio en un Controlador

```csharp
[ApiController]
[Route("api/academias")]
public class AcademiasDataController : ControllerBase
{
    private readonly AcademiaDataService _academiaDataService;

    public AcademiasDataController(AcademiaDataService academiaDataService)
    {
        _academiaDataService = academiaDataService;
    }

    [HttpGet("{academiaCodigo}/usuarios")]
    public async Task<ActionResult> GetUsuarios(string academiaCodigo)
    {
        var usuarios = await _academiaDataService.GetUsuariosPorAcademiaAsync(academiaCodigo);
        return Ok(usuarios);
    }

    [HttpGet("{academiaCodigo}/estado")]
    public async Task<ActionResult> GetEstadoConexion(string academiaCodigo)
    {
        var (isValid, message) = await _academiaDataService.VerificarConexionAcademiaAsync(academiaCodigo);
        
        if (isValid)
            return Ok(new { estado = "activo", mensaje = message });
        else
            return StatusCode(503, new { estado = "inactivo", mensaje = message });
    }
}
```

---

## Ejemplo 4: Manejo Avanzado de Errores

### Con Retry Logic

```csharp
public class ResilientConnectionProvider
{
    private readonly IConnectionStringValidator _validator;
    private readonly ILogger<ResilientConnectionProvider> _logger;
    private const int MaxRetries = 3;
    private const int RetryDelayMs = 1000;

    public ResilientConnectionProvider(
        IConnectionStringValidator validator,
        ILogger<ResilientConnectionProvider> logger)
    {
        _validator = validator;
        _logger = logger;
    }

    public async Task<bool> ValidateWithRetryAsync(
        string connectionString, 
        int maxRetries = MaxRetries)
    {
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            try
            {
                var isValid = await _validator.IsValidAsync(connectionString);
                
                if (isValid)
                {
                    _logger.LogInformation($"Validación exitosa en intento {attempt}");
                    return true;
                }

                _logger.LogWarning($"Validación falló en intento {attempt}/{maxRetries}");
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Error en intento {attempt}: {ex.Message}");
            }

            if (attempt < maxRetries)
            {
                _logger.LogInformation($"Reintentando en {RetryDelayMs}ms...");
                await Task.Delay(RetryDelayMs);
            }
        }

        _logger.LogError($"Validación falló después de {maxRetries} intentos");
        return false;
    }
}
```

### Con Caché

```csharp
public class CachedConnectionStringProvider : IConnectionStringProvider
{
    private readonly IConnectionStringProvider _innerProvider;
    private readonly IMemoryCache _cache;
    private const string CacheKeyPrefix = "conexion_";
    private const int CacheDurationMinutes = 30;

    public CachedConnectionStringProvider(
        IConnectionStringProvider innerProvider,
        IMemoryCache cache)
    {
        _innerProvider = innerProvider;
        _cache = cache;
    }

    public async Task<string?> GetConnectionStringAsync(string academiaCodigo)
    {
        var cacheKey = $"{CacheKeyPrefix}{academiaCodigo}";

        if (_cache.TryGetValue(cacheKey, out string? cached))
        {
            return cached;
        }

        var connectionString = await _innerProvider.GetConnectionStringAsync(academiaCodigo);

        if (connectionString != null)
        {
            _cache.Set(
                cacheKey,
                connectionString,
                TimeSpan.FromMinutes(CacheDurationMinutes)
            );
        }

        return connectionString;
    }

    public Task<(string?, ValidationResult)> GetConnectionStringWithValidationAsync(
        string academiaCodigo)
    {
        return _innerProvider.GetConnectionStringWithValidationAsync(academiaCodigo);
    }

    public Task<string?> GetConnectionStringUncheckedAsync(string academiaCodigo)
    {
        return _innerProvider.GetConnectionStringUncheckedAsync(academiaCodigo);
    }
}
```

Registrar en Program.cs:

```csharp
// Con caché
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IConnectionStringProvider>(sp =>
{
    var innerProvider = new ConnectionStringProvider(
        sp.GetRequiredService<ConfigDbContext>(),
        sp.GetRequiredService<IConnectionStringValidator>(),
        sp.GetRequiredService<ILogger<ConnectionStringProvider>>()
    );
    return new CachedConnectionStringProvider(
        innerProvider,
        sp.GetRequiredService<IMemoryCache>()
    );
});
```

---

## Ejemplo 5: Testing Unitario

```csharp
[TestFixture]
public class ConnectionStringValidatorTests
{
    private SqlConnectionValidator _validator;
    private ILogger<SqlConnectionValidator> _logger;

    [SetUp]
    public void Setup()
    {
        _logger = new MockLogger<SqlConnectionValidator>();
        _validator = new SqlConnectionValidator(_logger);
    }

    [Test]
    public async Task IsValidAsync_WithValidConnectionString_ReturnsTrue()
    {
        // Arrange
        var validConnection = "Server=db42639.public.databaseasp.net;Database=db42639;User Id=db42639;Password=Zp6!z9=A7@We;Encrypt=True;";

        // Act
        var result = await _validator.IsValidAsync(validConnection);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task IsValidAsync_WithEmptyConnectionString_ReturnsFalse()
    {
        // Act
        var result = await _validator.IsValidAsync("");

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task ValidateWithDetailsAsync_WithInvalidConnection_ReturnsError()
    {
        // Arrange
        var invalidConnection = "Server=invalid-server;Database=invalid-db;User Id=fake;Password=wrong;";

        // Act
        var result = await _validator.ValidateWithDetailsAsync(invalidConnection);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.ErrorMessage);
    }

    [Test]
    public async Task ValidateWithDetailsAsync_ParsesSQLErrorNumbers()
    {
        // Arrange
        var invalidCredsConnection = "Server=localhost;Database=test;User Id=wrong;Password=wrong;";

        // Act
        var result = await _validator.ValidateWithDetailsAsync(invalidCredsConnection);

        // Assert
        Assert.IsFalse(result.IsValid);
        Assert.IsNotNull(result.SqlErrorNumber);
    }
}
```

---

## Ejemplo 6: Monitoreo en Producción

### Health Check

```csharp
public class ConnectionStringHealthCheck : IHealthCheck
{
    private readonly IConnectionStringProvider _provider;

    public ConnectionStringHealthCheck(IConnectionStringProvider provider)
    {
        _provider = provider;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        var academias = new[] { "Konektia", "OtraAcademia" };
        var failedAcademias = new List<string>();

        foreach (var academia in academias)
        {
            var connectionString = await _provider.GetConnectionStringAsync(academia);
            if (connectionString == null)
            {
                failedAcademias.Add(academia);
            }
        }

        if (failedAcademias.Count == 0)
        {
            return HealthCheckResult.Healthy("Todas las conexiones están disponibles");
        }

        var failedList = string.Join(", ", failedAcademias);
        return HealthCheckResult.Unhealthy($"Academias con error: {failedList}");
    }
}

// Registrar en Program.cs
builder.Services.AddHealthChecks()
    .AddCheck<ConnectionStringHealthCheck>("conexiones");

app.MapHealthChecks("/health");
```

Usar:

```bash
curl https://localhost:7237/health
# Respuesta:
# {"status":"Healthy","checks":{"conexiones":{"status":"Healthy",...}}}
```

---

## Ejemplo 7: Dashboard de Administración

```razor
@page "/admin/conexiones"
@inject HttpClient Http

<div class="container mt-5">
    <h1>Estado de Conexiones - Administrador</h1>

    @if (academias != null)
    {
        <table class="table table-striped">
            <thead>
                <tr>
                    <th>Academia</th>
                    <th>Servidor</th>
                    <th>BD</th>
                    <th>Estado</th>
                    <th>Acciones</th>
                </tr>
            </thead>
            <tbody>
                @foreach (var academia in academias)
                {
                    <tr>
                        <td>@academia.Academia</td>
                        <td>@academia.Servidor</td>
                        <td>@academia.BaseDatos</td>
                        <td>
                            @if (academia.Valida)
                            {
                                <span class="badge bg-success">✓ Activo</span>
                            }
                            else
                            {
                                <span class="badge bg-danger">✗ Error</span>
                            }
                        </td>
                        <td>
                            <button @onclick="@(() => Revalidar(academia.Academia))" 
                                    class="btn btn-sm btn-primary">
                                Verificar
                            </button>
                        </td>
                    </tr>
                }
            </tbody>
        </table>
    }
</div>

@code {
    private List<AcademiaStatus> academias;

    protected override async Task OnInitializedAsync()
    {
        await CargarAcademiasAsync();
    }

    private async Task CargarAcademiasAsync()
    {
        // Aquí cargarías la lista de academias y su estado
        academias = new List<AcademiaStatus>();
        // ... código para llenar la lista
    }

    private async Task Revalidar(string academiaCodigo)
    {
        var response = await Http.GetAsync($"api/conexiones/salud/{academiaCodigo}");
        // Actualizar UI
        await CargarAcademiasAsync();
    }

    public class AcademiaStatus
    {
        public string Academia { get; set; }
        public string Servidor { get; set; }
        public string BaseDatos { get; set; }
        public bool Valida { get; set; }
    }
}
```

---

*Estos ejemplos te muestran cómo usar el sistema en diferentes escenarios. ¡Adapta según tus necesidades! 🚀*
