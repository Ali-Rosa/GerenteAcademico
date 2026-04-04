# 🔌 Guía de Cadenas de Conexión Dinámicas

## Descripción General

El sistema ahora soporta la obtención dinámica de cadenas de conexión desde el campo `CadenaConexionPrincipal` de la entidad `AcademiaConfig`. Esto permite que cada academia tenga su propia base de datos con validación automática.

## 🏗️ Arquitectura

### Componentes Principales

#### 1. **IConnectionStringValidator / SqlConnectionValidator**
Valida que una cadena de conexión SQL Server sea válida e intentando conectarse.

```csharp
// Validación simple (true/false)
var isValid = await validator.IsValidAsync(connectionString);

// Validación con detalles
var result = await validator.ValidateWithDetailsAsync(connectionString);
if (result.IsValid)
{
    Console.WriteLine($"Servidor: {result.Server}");
    Console.WriteLine($"Base de datos: {result.Database}");
}
else
{
    Console.WriteLine($"Error: {result.ErrorMessage}");
}
```

#### 2. **IConnectionStringProvider / ConnectionStringProvider**
Obtiene la cadena de conexión desde la base de datos de configuración, validándola previamente.

```csharp
// Con validación (recomendado)
var connectionString = await provider.GetConnectionStringAsync("Konectia");

// Con detalles de validación
var (connString, validationResult) = await provider.GetConnectionStringWithValidationAsync("Konectia");

// Sin validación (desarrollo)
var connString = await provider.GetConnectionStringUncheckedAsync("Konectia");
```

#### 3. **IAcademiaDbContextFactory / AcademiaDbContextFactory**
Factory que crea instancias de `AcademiaDbContext` con la cadena de conexión dinámicamente obtenida.

```csharp
// Crear contexto con validación
var academiaDb = await factory.CreateContextAsync("Konectia");
if (academiaDb != null)
{
    var usuarios = await academiaDb.Usuarios.ToListAsync();
}

// Crear contexto sin validación
var academiaDb = await factory.CreateContextUncheckedAsync("Konectia");
```

#### 4. **AcademiaService (Mejorado)**
Ahora valida la cadena de conexión cuando se obtiene una academia:

```csharp
// Esto validará automáticamente la conexión
var academiaData = await academiaService.GetAndValidateAsync("Konectia");
```

---

## 📡 Endpoints API

### Validar una Cadena de Conexión

**POST** `/api/conexiones/validar`

```bash
curl -X POST https://localhost:7237/api/conexiones/validar \
  -H "Content-Type: application/json" \
  -d '{
    "connectionString": "Server=localhost;Database=TestDB;User Id=sa;Password=pass123;Encrypt=False;",
    "timeoutSeconds": 5
  }'
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Conexión válida",
  "servidor": "localhost",
  "baseDatos": "TestDB",
  "usuario": "sa"
}
```

**Response (400 Bad Request):**
```json
{
  "success": false,
  "error": "Credenciales inválidas (Usuario o contraseña incorrectos)",
  "sqlErrorNumber": 18456
}
```

### Obtener Conexión de una Academia

**GET** `/api/conexiones/academia/{academiaCodigo}`

```bash
curl https://localhost:7237/api/conexiones/academia/Konectia
```

**Response (200 OK):**
```json
{
  "success": true,
  "academia": "Konectia",
  "servidor": "db42639.public.databaseasp.net",
  "baseDatos": "db42639",
  "usuario": "db42639",
  "cadenaConfigurada": true
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "academia": "Konectia",
  "error": "No se puede conectar a la base de datos especificada"
}
```

### Verificar Salud de Conexión

**GET** `/api/conexiones/salud/{academiaCodigo}`

```bash
curl https://localhost:7237/api/conexiones/salud/Konectia
```

**Response (200 OK):**
```json
{
  "estado": "saludable",
  "academia": "Konectia",
  "timestamp": "2024-01-15T10:30:45.123Z"
}
```

**Response (503 Service Unavailable):**
```json
{
  "estado": "degradado",
  "academia": "Konectia",
  "razon": "No se pudo obtener o validar la cadena de conexión"
}
```

---

## 📝 Cómo Configurar una Academia

### 1. Insertar Academia en ConfigDatabase

```sql
INSERT INTO Academias 
(Codigo, Nombre, LogoUrl, CadenaConexionPrincipal, Descripcion, Direccion, 
 Telefono, EmailContacto, Pais, Ciudad, IdFiscal, UrlSitioWeb, EsDemo, Activo)
VALUES 
(
  'Konectia',
  'Academia Konectia',
  'logo-konectia.png',
  'Server=db42639.public.databaseasp.net;Database=db42639;User Id=db42639;Password=Zp6!z9=A7@We;Encrypt=True;TrustServerCertificate=True;',
  'Academia de tecnología',
  'Calle Principal 123',
  '+34 555 123456',
  'contacto@konectia.com',
  'España',
  'Madrid',
  'ES12345678',
  'https://konectia.com',
  0,
  1
)
```

### 2. Verificar la Configuración

```bash
# Probar que la conexión sea válida
curl -X POST https://localhost:7237/api/conexiones/validar \
  -H "Content-Type: application/json" \
  -d '{
    "connectionString": "Server=db42639.public.databaseasp.net;Database=db42639;User Id=db42639;Password=Zp6!z9=A7@We;Encrypt=True;TrustServerCertificate=True;"
  }'

# O usar el endpoint de academia
curl https://localhost:7237/api/conexiones/academia/Konectia
```

---

## 🔄 Flujo Completo de Login

```
1. Usuario accede a /Konectia
   ↓
2. AcademiaEntry carga datos de ConfigDatabase
   ↓
3. AcademiaService.GetAndValidateAsync() ejecuta:
   - Verifica que academia existe
   - Verifica que está activa
   - Verifica que tiene todos los campos obligatorios
   - **VALIDA LA CADENA DE CONEXIÓN** ← AQUÍ
   - Retorna DTO si todo es válido
   ↓
4. Se redirige a /Konectia/login
   ↓
5. Usuario ingresa credenciales
   ↓
6. AuthService busca usuario en AcademiaDatabase usando:
   - ConnectionStringProvider obtiene la cadena de Konectia
   - Crea AcademiaDbContext dinámicamente
   - Valida credenciales del usuario
   ↓
7. Si es válido, genera JWT token
   ↓
8. Usuario ve dashboard
```

---

## 🛡️ Manejo de Errores

### Error: "No se puede conectar a la base de datos de '{academia}'"

**Causas posibles:**
- Servidor SQL no disponible
- Credenciales incorrectas
- Base de datos no existe
- Firewall bloqueando la conexión

**Solución:**
1. Verifica la cadena de conexión con `/api/conexiones/validar`
2. Comprueba que el servidor está disponible
3. Verifica credenciales y permisos en SQL
4. Comprueba la configuración de firewall/red

### Error: "Timeout al conectarse al servidor"

**Causas:**
- Servidor lento o no responde
- Timeout por defecto muy bajo (5 segundos)

**Solución:**
```csharp
// Aumentar timeout en validación
var result = await validator.ValidateWithDetailsAsync(
    connectionString, 
    timeoutSeconds: 15  // Cambiar timeout
);
```

---

## 💾 Base de Datos Requerida

La cadena de conexión debe apuntar a una base de datos **que ya exista** con las tablas necesarias:

```sql
-- Estructura mínima requerida
CREATE TABLE Roles (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL UNIQUE,
    Descripcion NVARCHAR(500)
);

CREATE TABLE Usuarios (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Nombre NVARCHAR(100) NOT NULL,
    Usuario NVARCHAR(100) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    Activo BIT NOT NULL DEFAULT 1,
    IntentosFallidos INT DEFAULT 0,
    UltimoLogin DATETIME2,
    RolId INT NOT NULL,
    FOREIGN KEY (RolId) REFERENCES Roles(Id)
);
```

---

## 🔒 Mejores Prácticas de Seguridad

### 1. Cifrar Cadenas de Conexión

En producción, guarda las cadenas de conexión cifradas:

```csharp
// En appsettings.Production.json, usar Azure Key Vault
"Jwt": {
  "SecretKey": "@Microsoft.KeyVault(SecretUri=https://...)"
}
```

### 2. Validar en Cada Acceso

El sistema valida automáticamente en:
- `AcademiaService.GetAndValidateAsync()` - Validación inicial
- `AuthService.LoginAsync()` - Acceso a usuario

### 3. Logging de Intentos

Todos los intentos de conexión se registran:
```
ILogger<SqlConnectionValidator>
ILogger<ConnectionStringProvider>
```

---

## 📊 Monitoreo

### Ver logs de conexiones

```csharp
// En Program.cs
builder.Logging.AddConsole();
builder.Logging.AddDebug();
builder.Logging.SetMinimumLevel(LogLevel.Information);
```

### Endpoint de Salud

```bash
# Verificar que todas las conexiones funcionan
curl https://localhost:7237/api/conexiones/salud/Konectia
curl https://localhost:7237/api/conexiones/salud/OtraAcademia
```

---

## 🧪 Testing

```csharp
[TestFixture]
public class ConnectionStringProviderTests
{
    private IConnectionStringProvider _provider;
    
    [Test]
    public async Task GetConnectionStringAsync_WithValidAcademia_ReturnsValidConnectionString()
    {
        // Arrange
        var academiaCodigo = "Konectia";
        
        // Act
        var result = await _provider.GetConnectionStringAsync(academiaCodigo);
        
        // Assert
        Assert.IsNotNull(result);
        Assert.IsNotEmpty(result);
    }
}
```

---

## 📚 Referencia Rápida

| Servicio | Método | Cuando usar |
|----------|--------|------------|
| `IConnectionStringValidator` | `IsValidAsync()` | Validar cualquier cadena de conexión |
| `IConnectionStringValidator` | `ValidateWithDetailsAsync()` | Obtener detalles del error |
| `IConnectionStringProvider` | `GetConnectionStringAsync()` | Obtener cadena con validación |
| `IConnectionStringProvider` | `GetConnectionStringUncheckedAsync()` | Obtener cadena sin validar (desarrollo) |
| `IAcademiaDbContextFactory` | `CreateContextAsync()` | Crear contexto dinámico |
| `AcademiaService` | `GetAndValidateAsync()` | Validar academia completa |

---

## 🚀 Próximas Mejoras

- [ ] Caché de cadenas de conexión validadas
- [ ] Retry automático en fallos temporales
- [ ] Pool de conexiones por academia
- [ ] Monitoreo en tiempo real del estado de conexiones
- [ ] Dashboard de salud de academias

---

## 📞 Soporte

Si encuentras problemas con las cadenas de conexión:
1. Verifica el endpoint `/api/conexiones/validar`
2. Revisa los logs en la consola
3. Comprueba la conectividad de red a SQL Server
4. Abre un issue en GitHub con los logs

---

*Última actualización: 2024*
