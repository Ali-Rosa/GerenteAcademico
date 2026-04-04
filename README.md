# 📚 GerenteAcademico

Sistema integral de gestión académica construido con **Blazor Server**, **.NET 10**, y **SQL Server**. Diseñado para administrar múltiples academias con autenticación segura basada en JWT tokens y cookies HTTP-only.

![.NET 10](https://img.shields.io/badge/.NET-10-purple)
![C# 14.0](https://img.shields.io/badge/C%23-14.0-blue)
![Blazor Server](https://img.shields.io/badge/Blazor-Server-lightblue)
![SQL Server](https://img.shields.io/badge/SQL-Server-orange)
![License](https://img.shields.io/badge/License-MIT-green)

---

## 🎯 Descripción General

**GerenteAcademico** es una aplicación web empresarial que permite la gestión centralizada de múltiples instituciones académicas. Proporciona un sistema robusto de autenticación multi-academia, gestión de usuarios y un dashboard intuitivo para administradores.

### ✨ Características Principales

- ✅ **Sistema Multi-Academia**: Soporta múltiples academias independientes en una sola instancia
- ✅ **Autenticación Segura**: JWT tokens + Cookies HTTP-only con HTTPS obligatorio
- ✅ **Sesiones Persistentes**: Las sesiones se mantienen activas sin redireccionamientos innecesarios
- ✅ **Dashboard Interactivo**: Interfaz moderna con Bootstrap 5
- ✅ **Validación de Credenciales**: Sistema robusto con hash BCrypt
- ✅ **API RESTful**: Endpoints seguros para autenticación y verificación
- ✅ **Control de Acceso**: Roles y permisos por usuario
- ✅ **Arquitectura Limpia**: Separación clara de responsabilidades

---

## 🏗️ Arquitectura

El proyecto sigue una arquitectura en capas:

```
GerenteAcademico/
├── Web/                          # Capa de Presentación (Blazor)
│   ├── Components/
│   │   └── Pages/               # Componentes Razor (Login, Dashboard, etc.)
│   ├── Controllers/             # API Controllers
│   ├── Services/                # Servicios del cliente (AuthService, AcademiaState)
│   └── Middleware/              # Middleware personalizado
│
├── Application/                  # Capa de Aplicación
│   ├── Services/                # Servicios de negocio (AuthService, JwtTokenService)
│   └── Dtos/                    # Data Transfer Objects
│
├── Domain/                       # Capa de Dominio
│   ├── Entities/                # Modelos de dominio
│   └── Interfaces/              # Contratos
│
└── Infrastructure/              # Capa de Infraestructura
    ├── Persistence/             # DbContext
    └── Repositories/            # Implementaciones de repositorio
```

### 📊 Flujo de Autenticación

```
1. Usuario accede a /{academia}
   ↓
2. AcademiaEntry valida la academia
   ↓
3. Redirige a /{academia}/login
   ↓
4. Login POST /api/auth/login
   ↓
5. AuthService genera JWT token
   ↓
6. AuthController crea cookie HTTP-only segura
   ↓
7. Dashboard verifica sesión con /api/auth/check
   ↓
8. Acceso concedido si la cookie es válida
```

---

## 🛠️ Stack Tecnológico

| Componente | Tecnología | Versión |
|---|---|---|
| **Framework** | .NET | 10.0 |
| **Lenguaje** | C# | 14.0 |
| **Web** | Blazor Server | - |
| **Base de Datos** | SQL Server | 2019+ |
| **ORM** | Entity Framework Core | 10.0 |
| **Autenticación** | JWT + Cookies | - |
| **Hash de Contraseñas** | BCrypt.NET | - |
| **Frontend** | Bootstrap | 5.3 |
| **Build** | MSBuild | - |

---

## 📋 Requisitos Previos

Antes de comenzar, asegúrate de tener instalado:

- **.NET SDK 10.0** o superior ([Descargar](https://dotnet.microsoft.com/download))
- **SQL Server 2019** o superior ([Descargar Express](https://www.microsoft.com/es-es/sql-server/sql-server-downloads))
- **Visual Studio 2022** o superior (o Visual Studio Code + extensiones)
- **Git** ([Descargar](https://git-scm.com))

### Verificar instalación:

```bash
dotnet --version
```

---

## 🚀 Instalación y Configuración

### 1. Clonar el Repositorio

```bash
git clone https://github.com/tu-usuario/GerenteAcademico.git
cd GerenteAcademico
```

### 2. Restaurar Dependencias

```bash
dotnet restore
```

### 3. Configurar Bases de Datos

Edita el archivo `appsettings.json` con tus cadenas de conexión:

```json
{
  "ConnectionStrings": {
    "ConfigDatabase": "Server=tu-servidor;Database=db_config;User Id=sa;Password=tu_password;Encrypt=True;TrustServerCertificate=True;",
    "AcademiaDatabase": "Server=tu-servidor;Database=db_academia;User Id=sa;Password=tu_password;Encrypt=True;TrustServerCertificate=True;"
  },
  "AppBaseUrl": "https://localhost:7237/",
  "Jwt": {
    "SecretKey": "tu-clave-secreta-muy-larga-de-al-menos-32-caracteres-CAMBIAR-EN-PRODUCCION",
    "Issuer": "GerenteAcademico",
    "Audience": "GerenteAcademico",
    "ExpirationMinutes": 480
  }
}
```

### 4. Aplicar Migraciones

```bash
dotnet ef database update --project GerenteAcademico/GerenteAcademico.csproj
```

### 5. Ejecutar la Aplicación

```bash
dotnet run
```

La aplicación estará disponible en `https://localhost:7237/`

---

## 📖 Uso

### Acceder a una Academia

1. Navega a `https://localhost:7237/{codigo-academia}`
   - Ejemplo: `https://localhost:7237/Konectia`

2. El sistema validará que la academia existe

3. Serás redirigido a `/Konectia/login`

### Iniciar Sesión

1. Ingresa tu usuario y contraseña
2. Si las credenciales son correctas:
   - Se genera un **JWT token**
   - Se crea una **cookie HTTP-only segura**
   - Eres redirigido al dashboard

3. En el dashboard verás:
   - Información de la academia
   - Datos de tu usuario
   - Estado de la sesión

### Cerrar Sesión

Haz clic en el botón **"🚪 Cerrar Sesión"** en el navbar del dashboard. Esto:
- Destruye la cookie en el servidor
- Limpia el estado local
- Te redirige a la pantalla de entrada

---

## 🔐 Seguridad

Este proyecto implementa varias capas de seguridad:

### Autenticación

- ✅ **JWT Tokens**: Tokens firmados digitalmente con clave secreta
- ✅ **BCrypt Hashing**: Contraseñas hasheadas de forma irreversible
- ✅ **Cookie HTTP-only**: No accesibles desde JavaScript (protección XSS)
- ✅ **HTTPS Only**: Cookies solo transmitidas por HTTPS

### Sesiones

- ✅ **Sliding Expiration**: Las sesiones se extienden automáticamente
- ✅ **Expiración de 8 horas**: Límite de vida de sesión configurable
- ✅ **Validación de Servidor**: Cada acción verifica la sesión en el servidor
- ✅ **SameSite Cookies**: Protección contra CSRF

### API

- ✅ **Validación de Entrada**: DTOs con validación de datos
- ✅ **Error Handling Global**: Middleware para capturar excepciones
- ✅ **Autorización por Claims**: Roles y permisos granulares

### Configuración Recomendada en Producción

```json
{
  "Jwt": {
    "SecretKey": "GENERAR-CLAVE-ALEATORIA-FUERTE-EN-PRODUCCION",
    "ExpirationMinutes": 240
  }
}
```

**Generar clave segura en PowerShell:**

```powershell
[System.Convert]::ToBase64String([System.Security.Cryptography.RNGCryptoServiceProvider]::new().GetBytes(64))
```

---

## 🔄 Endpoints API

### Autenticación

#### POST `/api/auth/login`
Inicia sesión y retorna JWT token

**Request:**
```json
{
  "academiaCodigo": "Konectia",
  "username": "admin",
  "password": "password123"
}
```

**Response (200 OK):**
```json
{
  "nombreUsuario": "Juan Pérez",
  "rol": "Administrador",
  "token": "eyJhbGciOiJIUzI1NiIs..."
}
```

#### POST `/api/auth/logout`
Cierra la sesión y destruye la cookie

**Response (200 OK):**
```json
{
  "message": "Sesión cerrada correctamente"
}
```

#### GET `/api/auth/check`
Verifica si la sesión actual es válida

**Response (200 OK):**
```json
{
  "authenticated": true,
  "user": {
    "name": "Juan Pérez",
    "role": "Administrador",
    "academia": "Konectia"
  }
}
```

**Response (401 Unauthorized):**
```json
{
  "authenticated": false
}
```

#### GET `/api/academias/validate/{codigo}`
Valida que una academia existe y retorna sus datos

**Response (200 OK):**
```json
{
  "nombre": "Konectia",
  "logoUrl": "logo.png",
  "connectionString": "Server=..."
}
```

---

## 📱 Componentes Principales

### `AcademiaEntry.razor`
- **Ruta**: `/{academia}`
- **Función**: Valida la academia y redirige a login
- **Flujo**: Verifica con el servidor que la academia existe

### `Login.razor`
- **Ruta**: `/{academia}/login`
- **Función**: Formulario de inicio de sesión
- **Flujo**: POST a `/api/auth/login`, guarda credenciales, redirige a dashboard

### `Dashboard.razor`
- **Ruta**: `/{academia}/dashboard`
- **Función**: Panel de control del usuario
- **Flujo**: Muestra datos de usuario y academia, verifica sesión activa

---

## 🗂️ Estructura de Servicios

### `AcademiaState` (Singleton)
Gestiona el estado global de la academia y usuario autenticado

```csharp
public class AcademiaState
{
    public string? Codigo { get; }
    public string? Nombre { get; }
    public string? NombreUsuario { get; }
    public string? RolUsuario { get; }
    public string? Token { get; }
    
    public bool IsAuthenticated { get; }
    public void SetUserData(string nombre, string rol, string token);
}
```

### `JwtTokenService`
Genera y valida JWT tokens

```csharp
public class JwtTokenService
{
    public string GenerateToken(string usuarioId, string nombre, string rol, string academia);
    public ClaimsPrincipal? ValidateToken(string token);
}
```

### `AuthService` (Servidor)
Lógica de autenticación y validación de credenciales

```csharp
public class AuthService
{
    public Task<LoginResponseDto> LoginAsync(LoginRequestDto request);
}
```

---

## 🧪 Características de Testing

Para probar la aplicación localmente:

1. **Usuario de Prueba**:
   - Username: `admin`
   - Password: `admin123`
   - Rol: `Administrador`

2. **Academia de Prueba**:
   - Código: `Konectia`
   - Nombre: `Academia Konectia`

---

## 📊 Modelos de Datos

### Usuario
```csharp
public class Usuario
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Usuario { get; set; }
    public string PasswordHash { get; set; }
    public bool Activo { get; set; }
    public int IntentosFallidos { get; set; }
    public DateTime UltimoLogin { get; set; }
    public Rol Rol { get; set; }
}
```

### Rol
```csharp
public class Rol
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string Descripcion { get; set; }
    public List<Usuario> Usuarios { get; set; }
}
```

### Academia
```csharp
public class Academia
{
    public int Id { get; set; }
    public string Codigo { get; set; }
    public string Nombre { get; set; }
    public string LogoUrl { get; set; }
    public string ConnectionString { get; set; }
}
```

---

## 🚨 Solución de Problemas

### El navegador redirige infinitamente a login

**Causa**: La sesión se está perdiendo entre componentes.

**Solución**:
- Verifica que `AcademiaState` está registrado como `Singleton`
- Asegúrate de que el endpoint `/api/auth/check` retorna `200 OK`
- Revisa que la cookie se está creando correctamente

### No puedo conectar a la base de datos

**Causa**: Cadena de conexión incorrecta.

**Solución**:
```bash
# Prueba la conexión con dotnet
dotnet ef dbcontext info

# Verifica las credenciales en appsettings.json
# Asegúrate de que SQL Server está corriendo
```

### JWT Token expira demasiado rápido

**Solución**: Ajusta `ExpirationMinutes` en `appsettings.json`

```json
"Jwt": {
  "ExpirationMinutes": 480  // Cambiar a otro valor (en minutos)
}
```

---

## 🔧 Compilación para Producción

```bash
# Compilar en modo Release
dotnet publish -c Release -o ./publish

# El resultado estará en ./publish/
```

### Docker (Opcional)

Crea un archivo `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["GerenteAcademico.csproj", "."]
RUN dotnet restore "GerenteAcademico.csproj"
COPY . .
RUN dotnet publish "GerenteAcademico.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80 443
ENTRYPOINT ["dotnet", "GerenteAcademico.dll"]
```

Construir y ejecutar:

```bash
docker build -t gerente-academico:latest .
docker run -p 7237:443 -e ASPNETCORE_ENVIRONMENT=Production gerente-academico:latest
```

---

## 📈 Roadmap Futuro

- [ ] Recuperación de contraseña por email
- [ ] Autenticación de dos factores (2FA)
- [ ] Integración con OAuth2 (Google, Microsoft)
- [ ] Dashboard con gráficos y estadísticas
- [ ] Sistema de auditoría completo
- [ ] Exportación de reportes
- [ ] API GraphQL
- [ ] Aplicación móvil
- [ ] Sincronización en tiempo real con SignalR

---

## 🤝 Contribuciones

Las contribuciones son bienvenidas. Para contribuir:

1. Fork el proyecto
2. Crea una rama de feature (`git checkout -b feature/AmazingFeature`)
3. Commit los cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

---

## 📄 Licencia

Este proyecto está bajo la Licencia MIT. Ver el archivo `LICENSE` para más detalles.

---

## 👥 Autor

**Desarrollado por**: Tu Nombre
- GitHub: [@tu-usuario](https://github.com/tu-usuario)
- Email: tu-email@example.com

---

## 📞 Soporte

Si encuentras algún problema o tienes preguntas:

- 📧 Email: soporte@gerente-academico.com
- 💬 Issues: [GitHub Issues](https://github.com/tu-usuario/GerenteAcademico/issues)
- 📚 Documentación: [Wiki](https://github.com/tu-usuario/GerenteAcademico/wiki)

---

## 🙏 Agradecimientos

- [Microsoft .NET](https://dotnet.microsoft.com/)
- [Blazor Documentation](https://learn.microsoft.com/es-es/aspnet/core/blazor/)
- [Bootstrap](https://getbootstrap.com/)
- [Entity Framework Core](https://learn.microsoft.com/en-us/ef/core/)

---

**⭐ Si este proyecto te fue útil, considera darle una estrella en GitHub!**

---

*Última actualización: 2024*
