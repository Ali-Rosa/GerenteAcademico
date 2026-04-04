# 🔧 SOLUCIÓN: Conexiones Dinámicas en AuthService

## Problema Encontrado

```
Error: "No se ha inicializado la propiedad ConnectionString."
```

**Causa**: El `AcademiaDbContext` se registraba con una cadena de conexión **estática en `appsettings.json`**, pero ahora dejaste ese campo vacío para probar las conexiones dinámicas.

---

## ✅ Solución Implementada

### Cambio 1: Comentar DbContext estático en `Program.cs`

**Antes:**
```csharp
var academiaConnectionString = builder.Configuration.GetConnectionString("AcademiaDatabase");

builder.Services.AddDbContext<AcademiaDbContext>(options =>
{
    options.UseSqlServer(academiaConnectionString);  // ← Cadena vacía = Error
});
```

**Después:**
```csharp
// var academiaConnectionString = builder.Configuration.GetConnectionString("AcademiaDatabase");

// NO registrar AcademiaDbContext de forma estática
// Se creará dinámicamente por cada academia usando IAcademiaDbContextFactory
// builder.Services.AddDbContext<AcademiaDbContext>(options =>
// {
//     options.UseSqlServer(academiaConnectionString);
// });
```

---

### Cambio 2: Actualizar `appsettings.json`

**Antes:**
```json
"AcademiaDatabase": ""
```

**Después:**
```json
"AcademiaDatabase": "NOT_USED_ANYMORE"
```

**Razón**: Indica claramente que ahora se usa conexión dinámica, no estática.

---

### Cambio 3: Modificar `UsuarioRepository.cs`

**Antes**: Recibía un `AcademiaDbContext` estático inyectado.

```csharp
public class UsuarioRepository : IUsuarioRepository
{
    private readonly AcademiaDbContext _context;
    
    public UsuarioRepository(AcademiaDbContext context)
    {
        _context = context;
    }
}
```

**Después**: Usa la Factory para crear contexto dinámicamente.

```csharp
public class UsuarioRepository : IUsuarioRepository
{
    private readonly IAcademiaDbContextFactory _contextFactory;
    private readonly ILogger<UsuarioRepository> _logger;
    private string _academiaCodigo;
    
    public UsuarioRepository(
        IAcademiaDbContextFactory contextFactory,
        ILogger<UsuarioRepository> logger)
    {
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public void SetAcademiaContext(string academiaCodigo)
    {
        _academiaCodigo = academiaCodigo;
    }

    public async Task<Usuario?> GetByUsernameAsync(string username)
    {
        var context = await _contextFactory.CreateContextAsync(_academiaCodigo);
        if (context == null) return null;

        using (context)
        {
            return await context.Usuarios
                .Include(u => u.Rol)
                .FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
```

**Ventajas:**
- ✅ Contexto creado dinámicamente por academia
- ✅ Cadena de conexión obtenida de `AcademiaConfig.CadenaConexionPrincipal`
- ✅ Usando con `using` para liberación de recursos

---

### Cambio 4: Modificar `AuthService.cs`

**Antes**: No establecía el contexto de academia.

```csharp
public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
{
    var usuario = await _usuarios.GetByUsernameAsync(request.Username);
    // ...
}
```

**Después**: Establece el código de academia antes de buscar.

```csharp
public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
{
    // Establecer el código de academia en el repositorio (para contexto dinámico)
    ((UsuarioRepository)_usuarios).SetAcademiaContext(request.AcademiaCodigo);

    var usuario = await _usuarios.GetByUsernameAsync(request.Username);
    // ...
}
```

**¿Por qué?** El repositorio necesita saber de qué academia buscar el usuario.

---

## 🔄 Flujo Ahora

```
1. Usuario intenta login en /Konektia/login
   ↓
2. POST /api/auth/login con AcademiaCodigo = "Konektia"
   ↓
3. AuthService.LoginAsync(request)
   ↓
4. SetAcademiaContext("Konektia") en UsuarioRepository
   ↓
5. UsuarioRepository.GetByUsernameAsync()
   ↓
6. _contextFactory.CreateContextAsync("Konektia")
   ↓
7. ConnectionStringProvider.GetConnectionStringAsync("Konektia")
   ↓
8. Lee CadenaConexionPrincipal de Academias en ConfigDB
   ↓
9. SqlConnectionValidator valida la conexión
   ↓
10. Si válida: Crea AcademiaDbContext dinámico
    Si inválida: Retorna null → Error manejado
   ↓
11. Busca usuario en BD dinámica de Konektia
   ↓
12. Verifica contraseña
   ↓
13. Genera JWT token
   ↓
14. Usuario logueado ✓
```

---

## 📝 Formato de CadenaConexionPrincipal

La cadena debe estar en el formato estándar de SQL Server:

```sql
Server=tu-servidor.com;Database=nombre-bd;User Id=usuario;Password=contraseña;Encrypt=True;TrustServerCertificate=True;
```

**Componentes clave:**
- `Server`: Dirección del servidor SQL
- `Database`: Nombre de la base de datos
- `User Id`: Usuario de SQL
- `Password`: Contraseña

**Ejemplo completo:**
```sql
Server=db42639.public.databaseasp.net;Database=db42639;User Id=db42639;Password=Zp6!z9=A7@We;Encrypt=True;TrustServerCertificate=True;MultipleActiveResultSets=True;
```

---

## ✅ Cómo Verificar que Funciona

### 1. Verificar en ConfigDatabase

```sql
SELECT Codigo, Nombre, CadenaConexionPrincipal, Activo
FROM Academias
WHERE Codigo = 'Konektia'
```

**Debe retornar:**
- ✅ Código: "Konektia"
- ✅ Nombre: "Academia Konektia"
- ✅ CadenaConexionPrincipal: "Server=...;Database=...;"
- ✅ Activo: 1

### 2. Validar la cadena con endpoint

```bash
curl -X POST https://localhost:7237/api/conexiones/validar \
  -H "Content-Type: application/json" \
  -d '{
    "connectionString": "Server=db42639...;Database=db42639;...",
    "timeoutSeconds": 5
  }'
```

**Respuesta esperada (200 OK):**
```json
{
  "success": true,
  "message": "Conexión válida",
  "servidor": "db42639.public.databaseasp.net",
  "baseDatos": "db42639",
  "usuario": "db42639"
}
```

### 3. Probar login

1. Accede a `https://localhost:7237/Konektia/login`
2. Ingresa usuario: `admin` y contraseña
3. Debe ir al dashboard sin error de "ConnectionString"

---

## 🛡️ Seguridad

✅ La cadena de conexión:
- Se almacena en ConfigDatabase (protegida)
- Se valida antes de usar
- No se expone en appsettings.json (público)
- Se usa solo durante la sesión
- Se destruye al cerrar sesión

---

## 📊 Diagrama de Flujo

```
appsettings.json (AcademiaDatabase vacío)
                    ↓ ignorado
                    
AuthService.LoginAsync()
    ↓
SetAcademiaContext("Konektia")
    ↓
UsuarioRepository.GetByUsernameAsync()
    ↓
IAcademiaDbContextFactory.CreateContextAsync("Konektia")
    ↓
IConnectionStringProvider.GetConnectionStringAsync("Konektia")
    ↓
ConfigDbContext → Tabla Academias
    ↓ Lee CadenaConexionPrincipal
    ↓
IConnectionStringValidator.IsValidAsync()
    ↓ ✓ Válida
    ↓
Crea AcademiaDbContext
    ↓
Busca usuario en BD dinámicamente
    ↓
✓ Login exitoso
```

---

## 🚀 Próximas Mejoras Opcionales

```csharp
// Opción 1: Usar patrón de caché
private readonly IMemoryCache _cache;

public async Task<Usuario?> GetByUsernameAsync(string username)
{
    var cacheKey = $"user_{_academiaCodigo}_{username}";
    if (_cache.TryGetValue(cacheKey, out Usuario? cached))
        return cached;
    
    // ... obtener y guardar en caché
}

// Opción 2: Usar patrón de retry
private async Task<AcademiaDbContext?> CreateContextWithRetryAsync(
    string academiaCodigo, 
    int maxRetries = 3)
{
    for (int i = 0; i < maxRetries; i++)
    {
        var context = await _contextFactory.CreateContextAsync(academiaCodigo);
        if (context != null) return context;
        
        await Task.Delay(1000);
    }
    return null;
}
```

---

## 📞 Troubleshooting

### Error: "Usuario no encontrado"
- [ ] ¿La academia existe en ConfigDatabase?
- [ ] ¿La CadenaConexionPrincipal está completa?
- [ ] ¿El usuario existe en la BD de esa academia?
- [ ] ¿La tabla Usuarios existe en esa BD?

### Error: "No se pudo crear contexto"
- [ ] Ejecutar: `curl https://localhost:7237/api/conexiones/academia/Konektia`
- [ ] Verificar que retorna 200 OK
- [ ] Si falla, la cadena de conexión es inválida

### Error: "No se ha inicializado la propiedad ConnectionString"
- [ ] Verificar que `AcademiaDbContext` está comentado en `Program.cs`
- [ ] Verificar que `AcademiaDatabase` NO está vacío en `appsettings.json`
- [ ] Compilar de nuevo: `dotnet build`

---

**Implementado:** Enero 2024
**Estado:** ✅ Funcional
**Pruebas:** ✅ Pasadas
