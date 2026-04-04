# ⚡ GUÍA RÁPIDA - TESTING DE ENDPOINTS

## 🚀 Inicio Rápido (5 minutos)

### 1. Abre Postman
```
https://www.postman.com/downloads/
```

### 2. Importa la colección
```
File → Import → GerenteAcademico_API_Collection.postman_collection.json
```

### 3. Ejecuta las pruebas

---

## 📌 Endpoints Básicos

### Prueba 1: ¿Hay roles registrados?
```
GET /konectia/api/roles
```
**Esperado:** Lista de roles (no vacía si tienes datos)

### Prueba 2: ¿Hay usuarios registrados?
```
GET /konectia/api/usuarios
```
**Esperado:** Lista de usuarios (no vacía si tienes datos)

### Prueba 3: Crear un rol
```
POST /konectia/api/roles
Body:
{
  "nombre": "Profesor",
  "descripcion": "Rol para profesores",
  "activo": true
}
```
**Esperado:** Status 201 + Rol creado con ID

### Prueba 4: Obtener ese rol
```
GET /konectia/api/roles/{id_del_rol_creado}
```
**Esperado:** Status 200 + Datos del rol

### Prueba 5: Actualizar el rol
```
PUT /konectia/api/roles/{id}
Body:
{
  "nombre": "Profesor Junior",
  "descripcion": "Rol actualizado",
  "activo": true
}
```
**Esperado:** Status 200 + Datos actualizados

### Prueba 6: Eliminar el rol
```
DELETE /konectia/api/roles/{id}
```
**Esperado:** Status 200 + Mensaje de éxito

---

## 🔍 Pruebas de Validación

### ❌ Intenta eliminar rol predefinido
```
DELETE /konectia/api/roles/1
```
**Esperado error:** "No se puede eliminar un rol predefinido"

### ❌ Intenta crear rol duplicado
```
POST /konectia/api/roles
Body:
{
  "nombre": "SuperUsuario",  ← Ya existe
  "descripcion": "Test",
  "activo": true
}
```
**Esperado error:** "Ya existe un rol con el nombre"

### ❌ Accede a academia inexistente
```
GET /academia_fake/api/roles
```
**Esperado error:** 404 Not Found

---

## 📊 Tabla de Respuestas

| Acción | Status | Respuesta |
|--------|--------|-----------|
| GET roles exitoso | 200 | Array de roles |
| GET rol no existe | 404 | "Rol con ID X no encontrado" |
| POST rol creado | 201 | Rol creado |
| POST rol duplicado | 400 | "Ya existe un rol con el nombre" |
| PUT rol actualizado | 200 | Rol actualizado |
| PUT rol no existe | 404 | "Rol con ID X no encontrado" |
| DELETE rol eliminado | 200 | "Rol eliminado exitosamente" |
| DELETE rol predefinido | 400 | "No se puede eliminar rol predefinido" |
| Academia no encontrada | 404 | "No se pudo obtener la conexión..." |

---

## 💡 Consejos

1. **Siempre usa** el header `Content-Type: application/json`
2. **Reemplaza** `konectia` con tu código de academia
3. **Reemplaza** los `{id}` con IDs reales de tu BD
4. **Copia el JSON** exactamente como está (ojo con comillas)
5. **Revisa logs** si algo falla (Visual Studio → Output)

---

## 🎯 Resultado Final Esperado

Después de todas las pruebas deberías ver:

```
✅ GET /konectia/api/roles                    → 200 OK
✅ GET /konectia/api/usuarios                 → 200 OK
✅ POST /konectia/api/roles                   → 201 Created
✅ PUT /konectia/api/roles/X                  → 200 OK
✅ DELETE /konectia/api/roles/X               → 200 OK
✅ POST /konectia/api/usuarios                → 201 Created
✅ Validaciones funcionan correctamente       → Errores apropiados
✅ Página Blazor muestra datos en tabla       → Visual correcto
```

---

## 🆘 Si algo no funciona

### Error: "No se pudo obtener la conexión"
- ✅ Verifica que `konectia` exista en ConfigDatabase
- ✅ Verifica la cadena de conexión
- ✅ Verifica que la BD sea accesible

### Error: "NotFound 404"
- ✅ Verifica que el servidor esté corriendo
- ✅ Verifica que la URL sea correcta
- ✅ Revisa la consola del servidor (Visual Studio)

### Error: "El nombre del tipo no se encontró"
- ✅ Compila el proyecto (Build → Build Solution)
- ✅ Revisa que los DTOs estén en la carpeta correcta
- ✅ Verifica los namespaces

### Lista vacía en GET
- ✅ Verifica que haya datos en la BD
- ✅ Verifica que `Activo = true` (rol/usuario)
- ✅ Accede directamente a la BD con SQL

---

## 📱 URLs de Referencia

```
Desarrollo:   http://localhost:7237
Producción:   https://tudominio.com (cuando despliegues)

Ejemplo de URLs reales:
GET     http://localhost:7237/konectia/api/roles
GET     http://localhost:7237/konectia/api/roles/1
POST    http://localhost:7237/konectia/api/roles
PUT     http://localhost:7237/konectia/api/roles/1
DELETE  http://localhost:7237/konectia/api/roles/1
```

---

## 🎬 Video de Prueba Mental

```
1. Abre Postman
2. GET /konectia/api/roles
   → Ves: [{ "id": 1, "nombre": "SuperUsuario", ... }]
   
3. POST /konectia/api/roles con {"nombre": "Docente", ...}
   → Ves: { "id": 5, "nombre": "Docente", ... }
   
4. GET /konectia/api/roles/5
   → Ves: { "id": 5, "nombre": "Docente", ... }
   
5. PUT /konectia/api/roles/5 con {"nombre": "Docente Senior", ...}
   → Ves: { "id": 5, "nombre": "Docente Senior", ... }
   
6. DELETE /konectia/api/roles/5
   → Ves: { "message": "Rol eliminado exitosamente" }
   
7. GET /konectia/api/roles/5
   → Ves: "Rol con ID 5 no encontrado" (404)

✅ ¡TODO FUNCIONA!
```

---

## 📚 Archivos Disponibles

```
📄 API_TESTING_GUIDE.md                 ← Guía detallada
📄 SUMMARY_AND_CHECKLIST.md             ← Checklist completo
📄 BEFORE_AFTER_COMPARISON.md           ← Qué cambió
📄 CONTROLLERS_FIX.md                   ← Detalles técnicos
📄 GerenteAcademico_API_Collection.postman_collection.json
                                        ← Importa en Postman
📄 EndpointsTest.cs                     ← Herramienta C# de pruebas
```

---

## ⏱️ Tiempo Estimado

- Importar colección Postman: **1 minuto**
- Ejecutar pruebas básicas: **3 minutos**
- Ejecutar pruebas de validación: **2 minutos**
- Total: **~6 minutos** ✅

---

**¡A probar! 🚀**
