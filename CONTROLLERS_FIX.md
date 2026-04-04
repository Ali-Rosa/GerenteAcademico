# 🔧 Corrección de Controladores - Roles y Usuarios

## Problema Identificado

Los controladores `RolesController` y `UsuariosController` no estaban devolviendo datos porque:

1. **Las rutas eran incorrectas**: Esperaban `/api/roles` y `/api/usuarios` en lugar de `/{academia}/api/roles` y `/{academia}/api/usuarios`
2. **Extraían el código de academia de forma incorrecta**: Usaban un método `GetAcademiaCodigoFromPath()` que no funcionaba correctamente
3. **Devolvían entidades en lugar de DTOs**: El `RolesController` devolvía `List<Rol>` en lugar de `List<RolDto>`
4. **Faltaban endpoints CRUD completos**: Solo tenían GET, faltaban POST, PUT, DELETE

## Soluciones Aplicadas

### 1. RolesController.cs ✅

**Cambios:**

- ✅ **Ruta actualizada**: `[Route("{academia}/api/[controller]")]`
- ✅ **Todos los métodos reciben `academia` como parámetro**: `GetAll(string academia)`
- ✅ **Devuelven DTOs**: Todos los endpoints retornan `RolDto` en lugar de `Rol`
- ✅ **CRUD completo implementado**:
  - `GET` - Obtiene todos los roles
  - `GET /{id}` - Obtiene un rol específico
  - `POST` - Crea un nuevo rol
  - `PUT /{id}` - Actualiza un rol
  - `DELETE /{id}` - Elimina un rol

**Validaciones Agregadas:**

- No permitir editar/eliminar roles predefinidos del sistema
- Validar que siempre exista al menos un rol SuperUsuario
- Validar que no haya usuarios asignados antes de eliminar un rol
- Evitar nombres de rol duplicados

### 2. UsuariosController.cs ✅

**Cambios:**

- ✅ **Ruta actualizada**: `[Route("{academia}/api/[controller]")]`
- ✅ **Todos los métodos reciben `academia` como parámetro**: `GetAll(string academia)`
- ✅ **CRUD completo implementado**:
  - `GET` - Obtiene todos los usuarios
  - `GET /{id}` - Obtiene un usuario específico
  - `POST` - Crea un nuevo usuario
  - `PUT /{id}` - Actualiza un usuario
  - `DELETE /{id}` - Elimina un usuario

**Validaciones Agregadas:**

- Validar que siempre exista al menos un usuario con rol SuperUsuario
- Evitar nombres de usuario y emails duplicados
- Verificar que el RolId sea válido
- Permitir actualizar contraseña sin proporcionarla en las actualizaciones

### 3. DTOs - Roles ✅

Se crearon dos DTOs para Roles:

```csharp
// RolDto.cs - Para retornar datos
public class RolDto
{
    public int Id { get; set; }
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool EsPredefinido { get; set; }
    public bool Activo { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaModificacion { get; set; }
}

// CreateUpdateRolDto.cs - Para crear/actualizar
public class CreateUpdateRolDto
{
    public string Nombre { get; set; }
    public string? Descripcion { get; set; }
    public bool Activo { get; set; } = true;
}
```

## URLs de los Endpoints

### Roles
- `GET /{academia}/api/roles` - Obtener todos los roles
- `GET /{academia}/api/roles/{id}` - Obtener un rol
- `POST /{academia}/api/roles` - Crear un rol
- `PUT /{academia}/api/roles/{id}` - Actualizar un rol
- `DELETE /{academia}/api/roles/{id}` - Eliminar un rol

### Usuarios
- `GET /{academia}/api/usuarios` - Obtener todos los usuarios
- `GET /{academia}/api/usuarios/{id}` - Obtener un usuario
- `POST /{academia}/api/usuarios` - Crear un usuario
- `PUT /{academia}/api/usuarios/{id}` - Actualizar un usuario
- `DELETE /{academia}/api/usuarios/{id}` - Eliminar un usuario

## Flujo de Datos

```
Blazor Component (RolesPage/UsuariosPage)
        ↓
    Http.GetAsync(/{academia}/api/roles)
        ↓
RolesController.GetAll(academia)
        ↓
IAcademiaDbContextFactory.CreateContextAsync(academia)
        ↓
IConnectionStringProvider.GetConnectionStringAsync(academia)
        ↓
ConfigDatabase → Busca academia por código
        ↓
Retorna conexión válida
        ↓
AcademiaDbContext
        ↓
Consulta Entity Framework
        ↓
MapToDto
        ↓
Retorna JSON con RolDto/UsuarioDto
```

## Pruebas

1. Verifica que tienes datos en tu base de datos de la academia
2. Asegúrate que el código de academia sea correcto en la URL
3. Verifica que la cadena de conexión en AcademiaConfig sea válida
4. Revisa los logs si hay errores "No se pudo obtener la conexión"

## Estado ✅

- ✅ Build exitoso
- ✅ DTOs creados y configurados
- ✅ Controladores actualizados con rutas correctas
- ✅ CRUD completo implementado
- ✅ Validaciones de datos agregadas
- ✅ Protección para roles/usuarios especiales (SuperUsuario)
