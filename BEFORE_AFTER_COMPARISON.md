# 📊 Comparativa: Antes vs Después

## RUTAS

### ANTES ❌
```
RolesController:
[Route("api/[controller]")]
GET /api/roles                      ← Ruta incorrecta, no tenía {academia}

UsuariosController:
[Route("api/[controller]")]
GET /api/usuarios                   ← Ruta incorrecta, no tenía {academia}
```

### DESPUÉS ✅
```
RolesController:
[Route("{academia}/api/[controller]")]
GET /{academia}/api/roles           ← Ahora incluye el código de academia
GET /{academia}/api/roles/{id}      ← Nuevo: Get por ID
POST /{academia}/api/roles          ← Nuevo: Crear
PUT /{academia}/api/roles/{id}      ← Nuevo: Actualizar
DELETE /{academia}/api/roles/{id}   ← Nuevo: Eliminar

UsuariosController:
[Route("{academia}/api/[controller]")]
GET /{academia}/api/usuarios        ← Actualizado
GET /{academia}/api/usuarios/{id}   ← Ya existía, corregido
POST /{academia}/api/usuarios       ← Ya existía, corregido
PUT /{academia}/api/usuarios/{id}   ← Ya existía, corregido
DELETE /{academia}/api/usuarios/{id}← Ya existía, corregido
```

## FIRMA DE MÉTODOS

### ANTES ❌ (RolesController)
```csharp
[HttpGet]
public async Task<ActionResult<List<Rol>>> GetAll()
{
    var academiaCodigo = GetAcademiaCodigoFromPath();  // Extrae de PATH
    // ...
}
```

### DESPUÉS ✅ (RolesController)
```csharp
[HttpGet]
public async Task<ActionResult<List<RolDto>>> GetAll(string academia)
{
    // academia viene como parámetro explícito
    if (string.IsNullOrEmpty(academia))
        return BadRequest("El código de la academia es requerido");
    // ...
}
```

## RETORNO DE DATOS

### ANTES ❌
```csharp
// Retornaba Rol directamente (expone datos internos)
return Ok(roles);
// [{ "id": 1, "nombre": "Admin", ... todo }]
```

### DESPUÉS ✅
```csharp
// Mapea a RolDto (solo datos necesarios)
return Ok(roles.Select(r => new RolDto
{
    Id = r.Id,
    Nombre = r.Nombre,
    Descripcion = r.Descripcion,
    EsPredefinido = r.EsPredefinido,
    Activo = r.Activo,
    FechaCreacion = r.FechaCreacion,
    FechaModificacion = r.FechaModificacion
}).ToList());
```

## VALIDACIONES AGREGADAS

### Roles
```csharp
✅ Validar que el rol no sea predefinido antes de editar/eliminar
✅ Validar que siempre exista al menos un SuperUsuario
✅ Validar que no haya usuarios asignados al rol antes de eliminar
✅ Evitar nombres de rol duplicados
```

### Usuarios
```csharp
✅ Validar que siempre exista al menos un SuperUsuario
✅ Evitar usernames y emails duplicados
✅ Validar que el RolId sea válido
✅ Permitir actualizar sin proporcionar nueva contraseña
```

## ASYNC/AWAIT

### ANTES ❌
```csharp
var roles = await Task.FromResult(
    context.Roles
        .Where(r => r.Activo)
        .AsNoTracking()
        .OrderBy(r => r.Nombre)
        .ToList()  // ← Ejecuta en memoria, no es async real
);
```

### DESPUÉS ✅
```csharp
var roles = await context.Roles
    .AsNoTracking()
    .OrderBy(r => r.Nombre)
    .Select(r => new RolDto { ... })
    .ToListAsync();  // ← Async verdadero, espera DB
```

## URLS EN BLAZOR

### LoadRoles() - DESPUÉS ✅
```csharp
// RolesPage.razor
var response = await Http.GetAsync($"/{AcademiaState.Codigo}/api/roles");

// Si AcademiaState.Codigo = "konectia"
// URL FINAL: /konectia/api/roles ✅

// Ahora funciona porque:
// 1. Route es {academia}/api/[controller]
// 2. La URL tiene el código de academia
// 3. GetAll(string academia) recibe "konectia"
```

## ESTADO DE LA ENTIDAD SUPERUSUARIO

### Validación en DELETE (Roles)
```csharp
if (rol.Nombre == "SuperUsuario")
{
    var superusuarios = await context.Roles
        .Where(r => r.Nombre == "SuperUsuario")
        .CountAsync();

    if (superusuarios <= 1)
        return BadRequest("Debe existir al menos un rol SuperUsuario");
}
```

### Validación en DELETE (Usuarios)
```csharp
if (usuario.Rol?.Nombre == "SuperUsuario")
{
    var superusuarios = await context.Usuarios
        .Include(u => u.Rol)
        .CountAsync(u => u.Rol!.Nombre == "SuperUsuario");

    if (superusuarios <= 1)
        return BadRequest("Debe existir al menos un usuario SuperUsuario");
}
```

---

## ¿POR QUÉ NO FUNCIONABA ANTES?

1. **Ruta mal configurada**: El controlador esperaba `/api/roles` pero Blazor enviaba `/{academia}/api/roles`
2. **Parámetro no recibido**: El método `GetAll()` no tenía parámetro `academia`, intentaba extraerlo del PATH
3. **Extracción fallida**: `GetAcademiaCodigoFromPath()` fallaba porque no encontraba el código correctamente
4. **Context NULL**: Si fallaba la extracción, `CreateContextAsync(null)` retornaba `null`
5. **Error NotFound**: El controlador retornaba "No se pudo obtener la conexión a la academia"

---

## TESTING (POSTMAN/INSOMNIA)

```bash
# Obtener todos los roles
GET http://localhost:7237/konectia/api/roles

# Obtener un rol específico
GET http://localhost:7237/konectia/api/roles/1

# Crear un nuevo rol
POST http://localhost:7237/konectia/api/roles
Content-Type: application/json

{
  "nombre": "Docente",
  "descripcion": "Rol para docentes",
  "activo": true
}

# Actualizar un rol
PUT http://localhost:7237/konectia/api/roles/1
Content-Type: application/json

{
  "nombre": "Docente Actualizado",
  "descripcion": "Descripción actualizada",
  "activo": true
}

# Eliminar un rol
DELETE http://localhost:7237/konectia/api/roles/1
```
