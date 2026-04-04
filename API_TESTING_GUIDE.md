# 🧪 GUÍA DE PRUEBAS DE ENDPOINTS

## ℹ️ Información Base

- **Base URL**: `http://localhost:7237`
- **Academia**: `konectia` (o el código que uses)
- **Content-Type**: `application/json`

---

## 📋 ENDPOINTS DE ROLES

### 1. ✅ GET - Obtener todos los roles

```http
GET /konectia/api/roles HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
[
  {
    "id": 1,
    "nombre": "SuperUsuario",
    "descripcion": "Acceso total al sistema",
    "esPredefinido": true,
    "activo": true,
    "fechaCreacion": "2024-01-01T00:00:00Z",
    "fechaModificacion": null
  },
  {
    "id": 2,
    "nombre": "Admin",
    "descripcion": "Administrador de la academia",
    "esPredefinido": true,
    "activo": true,
    "fechaCreacion": "2024-01-01T00:00:00Z",
    "fechaModificacion": null
  }
]
```

---

### 2. ✅ GET - Obtener un rol específico

```http
GET /konectia/api/roles/1 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
{
  "id": 1,
  "nombre": "SuperUsuario",
  "descripcion": "Acceso total al sistema",
  "esPredefinido": true,
  "activo": true,
  "fechaCreacion": "2024-01-01T00:00:00Z",
  "fechaModificacion": null
}
```

---

### 3. ✅ POST - Crear un nuevo rol

```http
POST /konectia/api/roles HTTP/1.1
Host: localhost:7237
Content-Type: application/json

{
  "nombre": "Docente",
  "descripcion": "Rol para docentes de la institución",
  "activo": true
}
```

**Respuesta esperada (201 Created):**
```json
{
  "id": 5,
  "nombre": "Docente",
  "descripcion": "Rol para docentes de la institución",
  "esPredefinido": false,
  "activo": true,
  "fechaCreacion": "2024-04-03T15:30:00Z",
  "fechaModificacion": null
}
```

---

### 4. ✅ PUT - Actualizar un rol

```http
PUT /konectia/api/roles/5 HTTP/1.1
Host: localhost:7237
Content-Type: application/json

{
  "nombre": "Docente Senior",
  "descripcion": "Rol para docentes con experiencia",
  "activo": true
}
```

**Respuesta esperada (200 OK):**
```json
{
  "id": 5,
  "nombre": "Docente Senior",
  "descripcion": "Rol para docentes con experiencia",
  "esPredefinido": false,
  "activo": true,
  "fechaCreacion": "2024-04-03T15:30:00Z",
  "fechaModificacion": "2024-04-03T15:35:00Z"
}
```

---

### 5. ✅ DELETE - Eliminar un rol

```http
DELETE /konectia/api/roles/5 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
{
  "message": "Rol eliminado exitosamente"
}
```

---

## 👥 ENDPOINTS DE USUARIOS

### 1. ✅ GET - Obtener todos los usuarios

```http
GET /konectia/api/usuarios HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
[
  {
    "id": 1,
    "nombre": "Juan",
    "apellido": "Pérez",
    "email": "juan@ejemplo.com",
    "username": "jperez",
    "documentacion": "12345678",
    "tipoDocumentacion": "cedula",
    "telefono": "1234567890",
    "telefonoEmergencia": null,
    "direccion": "Calle 1",
    "genero": "M",
    "nacionalidad": "Colombiana",
    "fechaNacimiento": "1990-01-01T00:00:00Z",
    "fotoUrl": null,
    "rolId": 1,
    "rolNombre": "SuperUsuario",
    "activo": true,
    "fechaCreacion": "2024-01-01T00:00:00Z",
    "fechaModificacion": null
  }
]
```

---

### 2. ✅ GET - Obtener un usuario específico

```http
GET /konectia/api/usuarios/1 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
{
  "id": 1,
  "nombre": "Juan",
  "apellido": "Pérez",
  "email": "juan@ejemplo.com",
  "username": "jperez",
  "documentacion": "12345678",
  "tipoDocumentacion": "cedula",
  "telefono": "1234567890",
  "telefonoEmergencia": null,
  "direccion": "Calle 1",
  "genero": "M",
  "nacionalidad": "Colombiana",
  "fechaNacimiento": "1990-01-01T00:00:00Z",
  "fotoUrl": null,
  "rolId": 1,
  "rolNombre": "SuperUsuario",
  "activo": true,
  "fechaCreacion": "2024-01-01T00:00:00Z",
  "fechaModificacion": null
}
```

---

### 3. ✅ POST - Crear un nuevo usuario

```http
POST /konectia/api/usuarios HTTP/1.1
Host: localhost:7237
Content-Type: application/json

{
  "nombre": "María",
  "apellido": "López",
  "email": "maria@ejemplo.com",
  "username": "mlopez",
  "password": "Password123!",
  "documentacion": "87654321",
  "tipoDocumentacion": "cedula",
  "telefono": "9876543210",
  "telefonoEmergencia": null,
  "direccion": "Calle 2",
  "genero": "F",
  "nacionalidad": "Colombiana",
  "fechaNacimiento": "1995-05-15T00:00:00Z",
  "rolId": 2,
  "activo": true
}
```

**Respuesta esperada (201 Created):**
```json
{
  "id": 2,
  "nombre": "María",
  "apellido": "López",
  "email": "maria@ejemplo.com",
  "username": "mlopez",
  "documentacion": "87654321",
  "tipoDocumentacion": "cedula",
  "telefono": "9876543210",
  "telefonoEmergencia": null,
  "direccion": "Calle 2",
  "genero": "F",
  "nacionalidad": "Colombiana",
  "fechaNacimiento": "1995-05-15T00:00:00Z",
  "fotoUrl": null,
  "rolId": 2,
  "rolNombre": "Admin",
  "activo": true,
  "fechaCreacion": "2024-04-03T16:00:00Z",
  "fechaModificacion": null
}
```

---

### 4. ✅ PUT - Actualizar un usuario

```http
PUT /konectia/api/usuarios/2 HTTP/1.1
Host: localhost:7237
Content-Type: application/json

{
  "nombre": "María",
  "apellido": "López García",
  "email": "maria.lopez@ejemplo.com",
  "username": "mlopez",
  "documentacion": "87654321",
  "tipoDocumentacion": "cedula",
  "telefono": "9876543210",
  "telefonoEmergencia": "1234567890",
  "direccion": "Calle 2, Apt 5",
  "genero": "F",
  "nacionalidad": "Colombiana",
  "fechaNacimiento": "1995-05-15T00:00:00Z",
  "rolId": 2,
  "activo": true
}
```

**Respuesta esperada (200 OK):**
```json
{
  "id": 2,
  "nombre": "María",
  "apellido": "López García",
  "email": "maria.lopez@ejemplo.com",
  "username": "mlopez",
  "documentacion": "87654321",
  "tipoDocumentacion": "cedula",
  "telefono": "9876543210",
  "telefonoEmergencia": "1234567890",
  "direccion": "Calle 2, Apt 5",
  "genero": "F",
  "nacionalidad": "Colombiana",
  "fechaNacimiento": "1995-05-15T00:00:00Z",
  "fotoUrl": null,
  "rolId": 2,
  "rolNombre": "Admin",
  "activo": true,
  "fechaCreacion": "2024-04-03T16:00:00Z",
  "fechaModificacion": "2024-04-03T16:05:00Z"
}
```

---

### 5. ✅ DELETE - Eliminar un usuario

```http
DELETE /konectia/api/usuarios/2 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (200 OK):**
```json
{
  "message": "Usuario eliminado exitosamente"
}
```

---

## 🚫 CASOS DE ERROR

### Caso 1: Rol no encontrado

```http
GET /konectia/api/roles/999 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (404 Not Found):**
```json
"Rol con ID 999 no encontrado"
```

---

### Caso 2: Intentar eliminar rol predefinido

```http
DELETE /konectia/api/roles/1 HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (400 Bad Request):**
```json
"No se puede eliminar un rol predefinido del sistema"
```

---

### Caso 3: Usuario duplicado

```http
POST /konectia/api/usuarios HTTP/1.1
Host: localhost:7237
Content-Type: application/json

{
  "nombre": "Duplicado",
  "email": "juan@ejemplo.com",
  "username": "jperez",
  "password": "Password123!",
  "documentacion": "99999999",
  "tipoDocumentacion": "cedula",
  "telefono": "1234567890",
  "rolId": 1,
  "activo": true
}
```

**Respuesta esperada (400 Bad Request):**
```json
"El nombre de usuario o email ya está registrado"
```

---

### Caso 4: Academia no encontrada

```http
GET /academia_inexistente/api/roles HTTP/1.1
Host: localhost:7237
Content-Type: application/json
```

**Respuesta esperada (404 Not Found):**
```json
"No se pudo obtener la conexión a la academia"
```

---

## 🛠️ CÓMO PROBAR EN POSTMAN

1. **Abre Postman**
2. **Crea una nueva colección**: "GerenteAcademico API"
3. **Para cada endpoint**:
   - Haz clic en "New Request"
   - Selecciona el método (GET, POST, PUT, DELETE)
   - Pega la URL: `http://localhost:7237/konectia/api/roles`
   - Configura Headers:
     - `Content-Type: application/json`
   - Si es POST/PUT, agrega el body JSON
   - Haz clic en "Send"

---

## 📊 RESUMEN DE ENDPOINTS

| Método | URL | Función | Body |
|--------|-----|---------|------|
| GET | `/konectia/api/roles` | Obtener todos | - |
| GET | `/konectia/api/roles/{id}` | Obtener uno | - |
| POST | `/konectia/api/roles` | Crear | RolDto |
| PUT | `/konectia/api/roles/{id}` | Actualizar | RolDto |
| DELETE | `/konectia/api/roles/{id}` | Eliminar | - |
| GET | `/konectia/api/usuarios` | Obtener todos | - |
| GET | `/konectia/api/usuarios/{id}` | Obtener uno | - |
| POST | `/konectia/api/usuarios` | Crear | UsuarioDto |
| PUT | `/konectia/api/usuarios/{id}` | Actualizar | UsuarioDto |
| DELETE | `/konectia/api/usuarios/{id}` | Eliminar | - |

---

## ⚠️ VALIDACIONES IMPORTANTES

### Roles
- ✅ No puedes editar un rol predefinido
- ✅ No puedes eliminar un rol predefinido
- ✅ Siempre debe existir al menos un SuperUsuario
- ✅ No puedes eliminar un rol si tiene usuarios
- ✅ No puedes crear dos roles con el mismo nombre

### Usuarios
- ✅ Siempre debe existir al menos un SuperUsuario
- ✅ No puedes crear dos usuarios con el mismo email
- ✅ No puedes crear dos usuarios con el mismo username
- ✅ El RolId debe ser válido

---

## 🔧 TROUBLESHOOTING

### Error: "No se pudo obtener la conexión a la academia"
- Verifica que el código de academia sea correcto
- Verifica que exista en ConfigDatabase
- Revisa que la cadena de conexión sea válida

### Error: "NotFound 404"
- El servidor no está corriendo
- La ruta es incorrecta
- Verifica que estés usando `/{academia}/api/`

### Error: "BadRequest 400"
- Revisa que el JSON sea válido
- Verifica que todos los campos requeridos estén presentes
- Revisa el Content-Type header

---

¡Listo para empezar a probar! 🚀
