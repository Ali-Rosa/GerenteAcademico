# 📖 Guía de Uso - Gestión de Usuarios

## 🎯 Acceso a la Página de Usuarios

### URL de Acceso
```
https://localhost:7237/{codigo_academia}/configuraciones/usuarios
```

Ejemplo:
```
https://localhost:7237/konectia/configuraciones/usuarios
```

### Navegación por Menú
1. Inicia sesión correctamente
2. En el sidebar izquierdo, busca **"Administración"**
3. Haz clic en **"Configuraciones"** (botón desplegable)
4. Selecciona **"Usuarios"** (👥)

## 👥 Operaciones CRUD

### 1. VER LISTA DE USUARIOS

**Ubicación**: Página principal de usuarios
- Se carga automáticamente al entrar
- Muestra tabla con:
  - Avatar del usuario (iniciales)
  - Nombre completo
  - Email
  - Número de documento
  - Teléfono
  - Rol asignado
  - Estado (Activo/Inactivo)

**Búsqueda**:
- Campo de búsqueda en la parte superior derecha
- Busca por: nombre, apellido, usuario, email, documento
- Búsqueda en tiempo real sin necesidad de presionar Enter

### 2. CREAR NUEVO USUARIO

**Pasos**:
1. Haz clic en botón **"➕ Nuevo Usuario"** (parte superior derecha)
2. Se abre el formulario de creación
3. Completa los campos requeridos (*):
   - Nombre *
   - Email *
   - Usuario *
   - Contraseña *
   - Documento *
   - Teléfono *
   - Rol *

4. Campos opcionales:
   - Apellido
   - Tipo de Documento (DUI, Pasaporte, Cédula, RUC)
   - Teléfono de Emergencia
   - Género (Masculino, Femenino, Otro)
   - Fecha de Nacimiento
   - Nacionalidad
   - Dirección

5. Selecciona estado: **"Usuario Activo"** (checkbox)
6. Haz clic en **"💾 Crear"**
7. Si todo es correcto, recibirás mensaje de éxito
8. La lista se actualiza automáticamente

**Validaciones**:
- Username único en la academia
- Email único en la academia
- Contraseña requerida para nuevos usuarios
- Todos los campos requeridos deben completarse

### 3. EDITAR USUARIO

**Pasos**:
1. En la tabla de usuarios, busca el usuario a editar
2. Haz clic en botón **"✏️"** (columna Acciones)
3. Se abre el formulario con datos actuales
4. Modifica los campos necesarios
5. La contraseña es opcional (dejar en blanco para no cambiar)
6. Haz clic en **"💾 Actualizar"**
7. Recibirás confirmación de actualización
8. La lista se actualiza automáticamente

**Notas**:
- No puede cambiar username a uno que ya existe
- No puede cambiar email a uno que ya existe
- La contraseña solo se actualiza si completas el campo

### 4. ELIMINAR USUARIO

**Pasos**:
1. En la tabla de usuarios, busca el usuario a eliminar
2. Haz clic en botón **"🗑️"** (columna Acciones)
3. Se abre ventana de confirmación: ¿Deseas eliminar este usuario?
4. Haz clic en **"OK"** para confirmar o **"Cancelar"** para abortar
5. Si confirmas, recibirás mensaje de éxito
6. La lista se actualiza automáticamente

⚠️ **Advertencia**: Esta acción no se puede deshacer. Asegúrate de seleccionar el usuario correcto.

## 🎨 Interfaz y Elementos

### Estados Visuales

**Usuario Activo**:
```
✓ Activo (badge verde)
```

**Usuario Inactivo**:
```
✗ Inactivo (badge gris)
```

**Roles**:
```
Mostrado en badge azul en la tabla
```

### Mensajes

**Éxito**:
```
✅ Usuario creado exitosamente
✅ Usuario actualizado exitosamente
✅ Usuario eliminado exitosamente
```

**Error**:
```
❌ Error al cargar los usuarios
❌ Error al guardar: [descripción]
❌ El nombre de usuario o email ya está registrado
```

## ⌨️ Formulario de Usuario

### Campo: Nombre
- **Tipo**: Texto
- **Requerido**: Sí
- **Restricción**: Máximo 100 caracteres

### Campo: Email
- **Tipo**: Email
- **Requerido**: Sí
- **Restricción**: Debe ser único en la academia

### Campo: Usuario (Username)
- **Tipo**: Texto
- **Requerido**: Sí
- **Restricción**: Debe ser único en la academia

### Campo: Contraseña
- **Tipo**: Password
- **Requerido**: Sí (solo para crear)
- **Requerido**: No (opcional para editar)

### Campo: Rol
- **Tipo**: Dropdown
- **Requerido**: Sí
- **Opciones**: Cargadas dinámicamente de la BD

### Campo: Documento
- **Tipo**: Texto
- **Requerido**: Sí
- **Restricción**: Número único

### Campo: Teléfono
- **Tipo**: Teléfono
- **Requerido**: Sí

## 🔄 Flujo Típico de Trabajo

### Crear usuario nuevo
```
1. ➕ Nuevo Usuario
2. Llenar formulario
3. Seleccionar rol
4. 💾 Crear
5. Confirmar éxito
```

### Actualizar usuario existente
```
1. Buscar usuario
2. ✏️ Editar
3. Modificar campos
4. 💾 Actualizar
5. Confirmar éxito
```

### Desactivar usuario (sin eliminar)
```
1. ✏️ Editar
2. Desmarcar "Usuario Activo"
3. 💾 Actualizar
```

### Cambiar rol de usuario
```
1. ✏️ Editar
2. Cambiar Rol (dropdown)
3. 💾 Actualizar
```

## 🔍 Búsqueda Rápida

**Por Nombre**:
- Escribe el nombre completo o parcial
- Resultado instantáneo

**Por Email**:
- Escribe el email
- Filtra en tiempo real

**Por Usuario**:
- Escribe el username
- Búsqueda parcial soportada

**Por Documento**:
- Escribe el número de documento
- Filtra por coincidencia

## ⚙️ Configuración de Roles

Los roles disponibles se cargan automáticamente desde la BD.

**Roles típicos**:
- Admin (Administrador)
- Docente (Profesor)
- Estudiante
- Coordinador
- Personal Administrativo

Para agregar nuevos roles, ve a:
```
/{academia}/configuraciones/roles
```

## 📊 Información Mostrada

### En la Tabla
- Nombre del usuario con iniciales de avatar
- Email
- Documento
- Teléfono
- Rol
- Estado
- Acciones (Editar, Eliminar)

### Contador
"Mostrando X de Y usuario(s)"

### Estado de Carga
- Spinner mientras se carga la lista
- Mensaje vacío si no hay usuarios

## 🚨 Errores Comunes

### "El nombre de usuario o email ya está registrado"
**Causa**: Username o Email ya existe
**Solución**: Usa un username/email único

### "Por favor completa todos los campos requeridos"
**Causa**: Falta completar campos marcados con *
**Solución**: Asegúrate de llenar todos los campos requeridos

### "La contraseña es requerida para nuevos usuarios"
**Causa**: No escribiste contraseña al crear usuario
**Solución**: Escribe una contraseña fuerte

### "Error al guardar"
**Causa**: Error en la API o base de datos
**Solución**: 
- Verifica la conexión
- Intenta nuevamente
- Contacta al administrador si persiste

## 💡 Tips y Trucos

1. **Búsqueda rápida**: Usa la búsqueda para encontrar usuarios rápidamente
2. **Avatar personalizado**: Las iniciales del nombre aparecen en el avatar
3. **Validación automática**: El email debe ser válido (requiere @)
4. **Cambio de contraseña**: Edita el usuario y cambia solo la contraseña
5. **Desactivar sin eliminar**: Desmarca "Usuario Activo" en lugar de eliminar

## 📱 Mobile / Responsive

La interfaz se adapta a dispositivos móviles:
- La tabla se ajusta automáticamente
- Botones son más grandes en móvil
- Búsqueda accesible en todos los tamaños
- Toque los botones de acción fácilmente

## 🔐 Seguridad

- Las contraseñas se envían cifradas (HTTPS)
- Las contraseñas se almacenan hasheadas en la BD
- Validación en cliente y servidor
- Solo administradores pueden gestionar usuarios
- Logout seguro

## ❓ Preguntas Frecuentes

**P**: ¿Puedo cambiar el email de un usuario?
**R**: Sí, edita el usuario y cambia el email. Debe ser único.

**P**: ¿Puedo recuperar un usuario eliminado?
**R**: No, la eliminación es permanente. Mejor desactívalo.

**P**: ¿Cuál es la contraseña más segura?
**R**: Usa al menos 8 caracteres con mayúsculas, minúsculas, números y símbolos.

**P**: ¿Puedo cambiar el rol de un usuario?
**R**: Sí, edita el usuario y selecciona otro rol.

**P**: ¿Qué pasa si cierro el formulario sin guardar?
**R**: Los cambios se pierden. Haz clic en Cancelar si deseas salir.

---

**Última actualización**: 2025-03-05
**Versión**: 1.0
