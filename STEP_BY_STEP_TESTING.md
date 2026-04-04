# 📱 INSTRUCCIONES PASO A PASO PARA PROBAR

## ⏱️ Tiempo Total: ~10 minutos

---

## 🎯 PARTE 1: CONFIGURACIÓN INICIAL (2 minutos)

### Paso 1: Verifica que tu servidor esté corriendo
```
1. Abre Visual Studio
2. Presiona F5 para ejecutar el proyecto
3. Espera a que se abra el navegador en http://localhost:7237
4. ✅ Deberías ver la página de login/dashboard
```

### Paso 2: Descarga Postman (si no lo tienes)
```
https://www.postman.com/downloads/
Instalación típica, siguiente → siguiente → Finish
```

### Paso 3: Abre Postman
```
Windows: Busca "Postman" en Inicio
Mac: Abre Postman desde Applications
```

---

## 🎯 PARTE 2: IMPORTAR COLECCIÓN (2 minutos)

### Paso 1: En Postman, haz clic en "File"
```
Menú superior → File
```

### Paso 2: Selecciona "Import"
```
File → Import
```

### Paso 3: Elige el archivo JSON
```
Busca: GerenteAcademico_API_Collection.postman_collection.json
Haz clic en "Open"
```

### Paso 4: Confirma la importación
```
Haz clic en "Import"
En el lado izquierdo aparecerá:
├── Roles
│   ├── GET - Obtener todos los roles
│   ├── GET - Obtener un rol específico
│   ├── POST - Crear un nuevo rol
│   ├── PUT - Actualizar un rol
│   └── DELETE - Eliminar un rol
├── Usuarios
│   └── ... (similar)
└── Casos de Error
    └── ... (para validaciones)
```

---

## 🎯 PARTE 3: PRUEBAS BÁSICAS (3 minutos)

### TEST 1: ¿Hay roles en la BD?

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → GET - Obtener todos los roles
   ```

2. **Verifica que la URL sea correcta**:
   ```
   http://localhost:7237/konectia/api/roles
   ```

3. **Haz clic en "Send"**
   ```
   Botón azul que dice "Send"
   ```

4. **Verifica la respuesta**:
   ```
   Status: 200 OK (verde)
   Body: 
   [
     {
       "id": 1,
       "nombre": "SuperUsuario",
       "descripcion": "...",
       "esPredefinido": true,
       "activo": true,
       "fechaCreacion": "2024-...",
       "fechaModificacion": null
     },
     ...
   ]
   ```

   **✅ Si ves esto:** Los datos se cargan correctamente
   **❌ Si ves error:** Revisa la sección "TROUBLESHOOTING"

---

### TEST 2: ¿Hay usuarios en la BD?

1. **En el lado izquierdo**, haz clic en:
   ```
   Usuarios → GET - Obtener todos los usuarios
   ```

2. **Haz clic en "Send"**

3. **Verifica que obtengas status 200 OK con datos**

   **✅ Correcto:** Usuarios cargados
   **❌ Error:** Revisa troubleshooting

---

### TEST 3: Crear un nuevo rol

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → POST - Crear un nuevo rol
   ```

2. **Verifica la pestaña "Body"** (debería estar seleccionada)
   ```
   {
     "nombre": "Docente",
     "descripcion": "Rol para docentes de la institución",
     "activo": true
   }
   ```

3. **Haz clic en "Send"**

4. **Verifica la respuesta**:
   ```
   Status: 201 Created (verde)
   Body:
   {
     "id": 5,  ← Este es el ID del nuevo rol
     "nombre": "Docente",
     "descripcion": "Rol para docentes de la institución",
     "esPredefinido": false,
     "activo": true,
     "fechaCreacion": "2024-04-03T15:30:00Z",
     "fechaModificacion": null
   }
   ```

   **✅ Correcto:** Rol creado con ID 5
   **❌ Error:** Revisa si el nombre ya existe

---

## 🎯 PARTE 4: PRUEBAS AVANZADAS (2 minutos)

### TEST 4: Obtener el rol que creamos

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → GET - Obtener un rol específico
   ```

2. **Edita la URL** para usar el ID del rol creado:
   ```
   ACTUAL:  http://localhost:7237/konectia/api/roles/1
   CAMBIAR: http://localhost:7237/konectia/api/roles/5
                                                      ↑
   (usa el ID que recibiste en TEST 3)
   ```

3. **Haz clic en "Send"**

4. **Verifica que obtengas el rol con ID 5**

---

### TEST 5: Actualizar el rol

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → PUT - Actualizar un rol
   ```

2. **Edita la URL** (igual que en TEST 4):
   ```
   http://localhost:7237/konectia/api/roles/5
   ```

3. **Edita el Body** (puedes cambiar el nombre):
   ```
   {
     "nombre": "Docente Senior",  ← Cambié aquí
     "descripcion": "Rol para docentes con experiencia",
     "activo": true
   }
   ```

4. **Haz clic en "Send"**

5. **Verifica la respuesta**:
   ```
   Status: 200 OK (verde)
   Body: Debe mostrar "Docente Senior" como nombre
   ```

---

### TEST 6: Eliminar el rol

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → DELETE - Eliminar un rol
   ```

2. **Edita la URL** (igual que antes):
   ```
   http://localhost:7237/konectia/api/roles/5
   ```

3. **Haz clic en "Send"**

4. **Verifica la respuesta**:
   ```
   Status: 200 OK (verde)
   Body:
   {
     "message": "Rol eliminado exitosamente"
   }
   ```

---

## 🎯 PARTE 5: PRUEBAS DE VALIDACIÓN (1 minuto)

### TEST 7: Intenta eliminar un rol predefinido

1. **En el lado izquierdo**, haz clic en:
   ```
   Casos de Error → DELETE - Rol predefinido (400)
   ```

2. **Haz clic en "Send"**

3. **Verifica que obtengas un error**:
   ```
   Status: 400 Bad Request (rojo)
   Body:
   "No se puede eliminar un rol predefinido del sistema"
   ```

   **✅ Correcto:** La validación funciona
   **❌ Error:** Revisa que el rol 1 sea predefinido en la BD

---

### TEST 8: Intenta crear un rol duplicado

1. **En el lado izquierdo**, haz clic en:
   ```
   Roles → POST - Crear un nuevo rol
   ```

2. **Cambia el Body** para usar un nombre que ya existe:
   ```
   {
     "nombre": "SuperUsuario",  ← Ya existe
     "descripcion": "Test",
     "activo": true
   }
   ```

3. **Haz clic en "Send"**

4. **Verifica que obtengas un error**:
   ```
   Status: 400 Bad Request (rojo)
   Body:
   "Ya existe un rol con el nombre 'SuperUsuario'"
   ```

   **✅ Correcto:** La validación de duplicados funciona

---

## 🎯 PARTE 6: VERIFICAR EN BLAZOR (1 minuto)

### Paso 1: Abre la página de Roles en Blazor

1. **En tu navegador, ve a**:
   ```
   http://localhost:7237/konectia/configuraciones/roles
   ```

2. **Presiona F5** para recargar

3. **Verifica que aparezcan los roles en una tabla**:
   ```
   ✅ Deberías ver:
   - Tabla con encabezados (ID, Nombre, Descripción, Estado, Acciones)
   - Filas con los roles de la BD
   - Botones "Nuevo Rol" en la esquina superior derecha
   ```

   ```
   ❌ Si ves error:
   - "No hay roles registrados"
   - "Error al cargar los roles"
   - Revisa la sección TROUBLESHOOTING
   ```

---

### Paso 2: Prueba crear un rol desde Blazor

1. **Haz clic en "Nuevo Rol"**

2. **Completa el formulario**:
   ```
   Nombre: Estudiante
   Descripción: Rol para estudiantes
   Activo: ☑ (marcado)
   ```

3. **Haz clic en "Guardar"**

4. **Verifica que aparezca en la tabla**

---

## 🆘 TROUBLESHOOTING

### ❌ Error: "No se pudo obtener la conexión a la academia"

**Causa:** Academia no encontrada en ConfigDatabase

**Solución:**
```
1. Abre SQL Server Management Studio
2. Conecta a tu servidor SQL
3. Busca la BD de Configuración (ConfigDb)
4. Verifica que la tabla Academias tenga:
   - Código = "konectia"
   - Activo = 1
   - CadenaConexionPrincipal = cadena válida
5. Si no existe, créala manualmente
```

---

### ❌ Error: "NotFound 404"

**Causa:** Servidor no está corriendo o URL incorrecta

**Solución:**
```
1. Verifica que Visual Studio esté ejecutando el proyecto
2. Presiona F5 si está pausado
3. Verifica que la URL sea exacta:
   http://localhost:7237/konectia/api/roles
                        ↑ Puerto correcto
                                   ↑ Academia correcta
                                            ↑ Ruta exacta
```

---

### ❌ Error: "Ya existe un rol con el nombre"

**Causa:** Intentas crear un rol que ya existe

**Solución:**
```
1. Cambia el nombre del rol a crear
2. O elimina el rol existente primero
3. O usa otro ID/academia
```

---

### ❌ Lista de roles vacía

**Causa:** No hay datos en la BD o están inactivos

**Solución:**
```
1. Abre SQL Server Management Studio
2. Busca la BD de la academia (ej: konectia_db)
3. Ejecuta:
   SELECT * FROM Roles WHERE Activo = 1
4. Si no hay resultados, inserta datos:
   INSERT INTO Roles (Nombre, Descripcion, Activo, EsPredefinido)
   VALUES ('SuperUsuario', 'Admin total', 1, 1)
```

---

### ❌ Status 400 Bad Request

**Causa:** JSON malformado o datos inválidos

**Solución:**
```
1. Verifica que el JSON sea válido:
   - Usa comillas dobles (no simples)
   - Sin comillas alrededor de números
   - Sin comas finales

   CORRECTO:
   {
     "nombre": "Docente",
     "activo": true
   }

   INCORRECTO:
   {
     'nombre': 'Docente',  ← Comillas simples
     "activo": true,       ← Coma final
   }
```

---

### ❌ Blazor muestra "Error al cargar los roles"

**Causa:** Mismas causas que arriba + problemas de CORS

**Solución:**
```
1. Abre consola del navegador (F12)
2. Ir a Pestaña "Console"
3. Busca mensajes de error
4. Verifica que la URL sea correcta
5. Revisa los logs de Visual Studio
```

---

## ✅ CHECKLIST FINAL

Después de todas las pruebas, marca lo que funcione:

```
□ GET /konectia/api/roles retorna datos (200 OK)
□ GET /konectia/api/usuarios retorna datos (200 OK)
□ POST /konectia/api/roles crea un rol (201 Created)
□ GET /konectia/api/roles/{id} obtiene ese rol (200 OK)
□ PUT /konectia/api/roles/{id} actualiza (200 OK)
□ DELETE /konectia/api/roles/{id} elimina (200 OK)
□ DELETE rol predefinido da error (400 Bad Request)
□ POST rol duplicado da error (400 Bad Request)
□ POST /konectia/api/usuarios crea usuario (201 Created)
□ DELETE /konectia/api/usuarios/{id} elimina (200 OK)
□ Página Blazor muestra roles en tabla
□ Página Blazor muestra usuarios en tabla
□ Botón "Nuevo Rol" funciona
□ Botón "Nuevo Usuario" funciona
□ Puedo editar desde la tabla
□ Puedo eliminar desde la tabla

SI TODAS ESTÁN MARCADAS: ✅ ¡LISTO PARA PRODUCCIÓN!
```

---

## 🎉 RESUMEN

```
Has probado exitosamente:
✅ 10 endpoints (CRUD completo)
✅ 4 validaciones
✅ 3 casos de error
✅ Integración con Blazor
✅ Mapeo de DTOs
✅ Async/await
✅ Manejo de excepciones
✅ Logging

Tu API está lista para:
✅ Producción
✅ Escalabilidad
✅ Mantenimiento
✅ Extensión futura

¡FELICIDADES! 🎊
```

---

## 📞 ¿Necesitas ayuda?

Si algo no funciona:
1. Revisa TROUBLESHOOTING arriba
2. Consulta los logs de Visual Studio (View → Output)
3. Abre la consola del navegador (F12 → Console)
4. Verifica la BD directamente en SQL Server Management Studio

---

**¡Ahora sí! A probar los endpoints 🚀**
