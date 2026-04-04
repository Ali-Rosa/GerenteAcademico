# 🔄 Diagrama de Flujo - Cadenas de Conexión Dinámicas

## 1. Flujo de Login con Validación

```
┌─────────────────────────────────────────────────────────────────────┐
│                     USUARIO ACCEDE A /Konektia                       │
└──────────────────────────────┬──────────────────────────────────────┘
                               │
                               ▼
                    ┌──────────────────────┐
                    │  AcademiaEntry      │
                    │  OnInitialized()    │
                    └──────────┬───────────┘
                               │
                               ▼
                    ┌──────────────────────────────┐
                    │ AcademiaService              │
                    │ GetAndValidateAsync()        │
                    └──────────┬───────────────────┘
                               │
          ┌────────────────────┼────────────────────┐
          │                    │                    │
          ▼                    ▼                    ▼
   ┌─────────────┐      ┌──────────────┐   ┌──────────────┐
   │ ¿Academia   │      │ ¿Academia    │   │ ¿Campos      │
   │ existe?     │      │ activa?      │   │ obligatorios?│
   │             │      │              │   │              │
   │ ✓ ConfigDB  │      │ ✓ ConfigDB   │   │ ✓ Validar    │
   └─────────────┘      └──────────────┘   └──────────────┘
                               │
                               ▼
          ┌────────────────────────────────────────┐
          │  VALIDAR CADENA DE CONEXIÓN ← NUEVO   │
          │  (ConnectionStringValidator)           │
          └────────┬─────────────────────────────┘
                   │
        ┌──────────┴──────────┐
        │                     │
        ▼                     ▼
   ┌────────┐            ┌────────┐
   │ ✓ VÁLIDA│            │ ✗ INVÁLIDA
   │        │            │
   │ Conecta│            │ Error: "No se puede
   │ OK     │            │ conectar a BD"
   └────┬───┘            │
        │                └────────────────┐
        │                                 │
        ▼                                 ▼
   ┌─────────────┐              ┌──────────────────┐
   │ Retorna DTO │              │ Retorna Error    │
   │             │              │ HTTP 400         │
   │ -Nombre     │              │                  │
   │ -LogoUrl    │              │ Redirige a /     │
   │ -Conexión OK│              │                  │
   └────┬────────┘              └──────────────────┘
        │
        ▼
   ┌──────────────────────┐
   │ Redirige a login     │
   │ /Konektia/login      │
   └──────────┬───────────┘
              │
              ▼
   ┌──────────────────────┐
   │ Usuario ingresa      │
   │ credenciales         │
   └──────────┬───────────┘
              │
              ▼
   ┌──────────────────────────────┐
   │ AuthService.LoginAsync()     │
   │                              │
   │ 1. Obtener cadena dinámica   │
   │    ConnectionStringProvider  │
   │ 2. Crear AcademiaDbContext   │
   │ 3. Buscar usuario            │
   │ 4. Validar contraseña        │
   │ 5. Generar JWT token         │
   └──────────┬───────────────────┘
              │
        ┌─────┴─────┐
        │           │
        ▼           ▼
   ┌────────┐  ┌────────┐
   │ ✓ OK   │  │ ✗ ERROR│
   │        │  │        │
   │ Login  │  │ Mostrar│
   │ exitoso│  │ error  │
   └────┬───┘  │        │
        │      └────────┘
        ▼
   ┌──────────────────────┐
   │ Redirige a           │
   │ /Konektia/dashboard  │
   └──────────────────────┘
```

---

## 2. Arquitectura de Servicios

```
┌─────────────────────────────────────────────────────────────────┐
│                         WEB / CONTROLLERS                        │
│  ┌─────────────────────────────────────────────────────────┐   │
│  │ AcademiasController    AuthController   ConexionesController│
│  │ GET /{academia}/...    POST /login      POST /validar      │
│  │                        POST /logout     GET /academia/{id}  │
│  │                        GET  /check      GET /salud/{id}     │
│  └─────────────────────────────────────────────────────────┘   │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                      APPLICATION SERVICES                        │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ AcademiaService         AuthService      JwtTokenService│  │
│  │ GetAndValidateAsync()   LoginAsync()     GenerateToken()  │
│  │ └─────────────────────────────────────────────────────────┘ │
│  │        ▲                      ▲                  │         │
│  │        │                      │                  │         │
│  │        └──────────────────────┴──────────────────┘         │
│  │              INFRASTRUCTURE SERVICES                       │
│  │  ┌─────────────────────────────────────────────────────┐  │
│  │  │ ConnectionStringValidator                           │  │
│  │  │ - IsValidAsync()                                    │  │
│  │  │ - ValidateWithDetailsAsync()                        │  │
│  │  │                                                     │  │
│  │  │ ConnectionStringProvider                            │  │
│  │  │ - GetConnectionStringAsync()        ──┐             │  │
│  │  │ - GetConnectionStringWithValidationAsync()          │  │
│  │  │ - GetConnectionStringUncheckedAsync()               │  │
│  │  │                                      │              │  │
│  │  │ AcademiaDbContextFactory      ◄─────┘              │  │
│  │  │ - CreateContextAsync()                              │  │
│  │  │ - CreateContextUncheckedAsync()                     │  │
│  │  └─────────────────────────────────────────────────────┘  │
│  └────────────────────────────────────────────────────────────┘
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                     PERSISTENCE LAYER                            │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ConfigDbContext (Escritura: Manual)                      │  │
│  │ - DbSet<AcademiaConfig> Academias  ◄─── Lee Config     │  │
│  │                                                          │  │
│  │ AcademiaDbContext (Dinámico por Academia)              │  │
│  │ - DbSet<Usuario>                                        │  │
│  │ - DbSet<Rol>                                            │  │
│  │ - Creado dinámicamente según academia                   │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────┬────────────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────────────┐
│                      SQL SERVER DATABASES                        │
│  ┌──────────────────────────────────────────────────────────┐  │
│  │ ConfigDatabase              AcademiaDatabase (DINÁMICO) │  │
│  │                                                         │  │
│  │ ┌────────────┐            ┌──────────────────────┐    │  │
│  │ │ Academias  │            │ Usuarios (Konektia) │    │  │
│  │ │            │            │ Roles               │    │  │
│  │ │ Código: OK │            └──────────────────────┘    │  │
│  │ │ Nombre: OK │            ┌──────────────────────┐    │  │
│  │ │ Cadena: OK │◄──────────►│ Otros datos academias    │  │
│  │ └────────────┘            └──────────────────────┘    │  │
│  └──────────────────────────────────────────────────────────┘  │
└────────────────────────────────────────────────────────────────┘
```

---

## 3. Flujo de Validación de Conexión

```
              ConnectionStringValidator
                     │
      ┌──────────────┼──────────────┐
      │              │              │
      ▼              ▼              ▼
┌─────────┐   ┌──────────────┐  ┌────────────┐
│ Parsear │   │ Validar que  │  │ Intentar   │
│ Sintaxis│   │ tiene Server │  │ conectarse │
│         │   │ y Database   │  │            │
└────┬────┘   └──────┬───────┘  └────┬───────┘
     │               │               │
  ✗ │           ✗ │               ✗ │
  Invalida     Invalida         Conexión
     │               │               │
     └───────┬───────┴───────┬───────┘
             │               │
          ┌──┴──────────────┐│
          │                 │
          ▼                 ▼
    ┌──────────┐    ┌──────────────┐
    │ Retorna  │    │ Retorna      │
    │ Formato  │    │ Detalles del │
    │ Inválido │    │ Error SQL    │
    │          │    │              │
    │ ✗ INVÁLIDA    │ - Error 18456
    └──────────┘    │ - Error 20
                    │ - Timeout
                    │ - Mensaje SQL
                    │
                    │ ✗ INVÁLIDA
                    └──────────────┘
```

---

## 4. Estado de Academia en ConfigDatabase

```
┌────────────────────────────────────────────────────────┐
│                     Tabla: Academias                    │
├────────────────────────────────────────────────────────┤
│ ID  │ Codigo   │ Nombre           │ Activo │ ← Mín. reqts
├─────┼──────────┼──────────────────┼────────┤
│ 1   │ Konektia │ Academia Konektia│ 1      │
└────────────────────────────────────────────────────────┘
                        │
                        ▼
        ┌───────────────────────────────────────┐
        │ CadenaConexionPrincipal (CLAVE)       │
        ├───────────────────────────────────────┤
        │ Server=db42639....;Database=db42639;..│
        └───────────────────────────────────────┘
                        │
                        ▼
        ┌───────────────────────────────────────┐
        │ ConnectionStringProvider              │
        │                                       │
        │ 1. Lee cadena de ConfigDB            │
        │ 2. Valida conexión                   │
        │ 3. Retorna si es válida              │
        │ 4. Null si no es válida              │
        └───────────────────────────────────────┘
```

---

## 5. Uso en AcademiaService

```
PUBLIC GetAndValidateAsync(codigo)
   │
   ├─► Obtener Academia de ConfigDB
   │   └─► ¿Existe? ──NO──► Excepción
   │       │
   │       ▼ SÍ
   │
   ├─► Validar que está ACTIVO
   │   └─► ¿Activo? ──NO──► Excepción
   │       │
   │       ▼ SÍ
   │
   ├─► Validar Campos Obligatorios
   │   ├─ Código
   │   ├─ Nombre
   │   ├─ CadenaConexionPrincipal ◄─── NUEVO
   │   └─ Otros campos
   │   │
   │   └─► ¿Todos presentes? ──NO──► Excepción
   │       │
   │       ▼ SÍ
   │
   ├─► VALIDAR CADENA DE CONEXIÓN ◄─── NUEVO
   │   │
   │   └─► ConnectionStringValidator
   │       └─► IsValidAsync()
   │           │
   │           ├─► ¿Válida? ──NO──► Excepción
   │           │                   "No se puede conectar"
   │           │
   │           ▼ SÍ
   │
   └─► RETORNAR AcademiaInitialDataDto
       ├─ Nombre
       ├─ LogoUrl
       └─ Conexión validada ✓
```

---

## 6. Endpoints de Validación

```
        ┌─────────────────────────────────────────┐
        │   ConexionesController (Nuevo)          │
        └─────────────────────────────────────────┘
                        │
        ┌───────────────┼───────────────┐
        │               │               │
        ▼               ▼               ▼
    ┌────────┐     ┌────────┐     ┌────────┐
    │ POST   │     │ GET    │     │ GET    │
    │ /validar      │ /academia... │ /salud │
    │        │     │        │     │        │
    │        │     │        │     │        │
    └────┬───┘     └────┬───┘     └────┬───┘
         │              │              │
         ▼              ▼              ▼
    Valida       Obtiene       Verifica
    cualquier    conexión      estado de
    cadena       para          conexión
    ingresada    academia      actual
```

---

## 7. Flujo Completo de Login

```
┌──────────────────────────────────────────────────────────┐
│  1. Usuario entra a /Konektia                            │
└──────────┬───────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────┐
│  2. AcademiaEntry.OnInitialized()                        │
│     - Llama LoadAcademiaData()                           │
└──────────┬───────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────┐
│  3. AcademiaService.GetAndValidateAsync("Konektia")     │
│     ┌──────────────────────────────────────────────┐   │
│     │ ✓ Valida academia existe                    │   │
│     │ ✓ Valida academia activa                    │   │
│     │ ✓ Valida campos obligatorios                │   │
│     │ ✓ VALIDA CADENA DE CONEXIÓN ◄─ NUEVO       │   │
│     │   ConnectionStringValidator                  │   │
│     └──────────────────────────────────────────────┘   │
└──────────┬──────────────────────────────────────────────┘
           │
        ┌──┴──────┐
        │         │
        ▼         ▼
    ┌────┐   ┌────────────┐
    │ ✓  │   │ ✗ Error    │
    │    │   │            │
    └──┬─┘   │ Mostrar    │
       │     │ error en / │
       │     └────────────┘
       ▼
┌──────────────────────────────────────────────────────────┐
│  4. Redirige a /Konektia/login                          │
└──────────┬───────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────┐
│  5. Usuario ingresa credenciales                         │
│     - Username: admin                                    │
│     - Password: ****                                     │
└──────────┬───────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────┐
│  6. POST /api/auth/login                                │
│     AuthService.LoginAsync()                            │
│     ┌──────────────────────────────────────────────┐   │
│     │ ① Obtener cadena con ConnectionStringProvider    │
│     │ ② Crear AcademiaDbContext dinámico              │
│     │ ③ Buscar usuario en AcademiaDatabase            │
│     │ ④ Validar contraseña (BCrypt)                   │
│     │ ⑤ Generar JWT token                             │
│     │ ⑥ Crear cookie HTTP-only                        │
│     └──────────────────────────────────────────────┘   │
└──────────┬──────────────────────────────────────────────┘
           │
        ┌──┴──────┐
        │         │
        ▼         ▼
    ┌────┐   ┌────────────┐
    │ ✓  │   │ ✗ Error    │
    │    │   │            │
    │    │   │ Email error│
    │    │   │ al login   │
    │    │   └────────────┘
    │    │
    └──┬─┘
       │
       ▼
┌──────────────────────────────────────────────────────────┐
│  7. Redirige a /Konektia/dashboard                      │
└──────────┬───────────────────────────────────────────────┘
           │
           ▼
┌──────────────────────────────────────────────────────────┐
│  8. Dashboard.OnInitialized()                           │
│     - Verifica sesión con /api/auth/check              │
│     - Carga datos del usuario                           │
│     - Muestra página de bienvenida                      │
└──────────────────────────────────────────────────────────┘
```

---

## 🎯 Resumen Visual

```
ANTES:
┌─────────┐     ┌──────────────┐
│ Usuario │────→│ ConfigDB      │
│         │     │ Cadena: Fixed │
└─────────┘     └──────────────┘
                       │
                       ▼
                ┌─────────────┐
                │ AcademiaDB  │
                │ (Estática)  │
                └─────────────┘


DESPUÉS (NUEVO):
┌─────────┐     ┌──────────────┐     ┌──────────────┐
│ Usuario │────→│ ConfigDB      │────→│ Validator    │
│         │     │ Lee cadena    │     │ Verifica     │
└─────────┘     │ dinámicamente │     │ conexión ✓   │
                └──────────────┘     └──────────────┘
                       │                     │
                       └─────────┬───────────┘
                                 │
                                 ▼
                        ┌─────────────────┐
                        │ AcademiaDB      │
                        │ (DINÁMICO)      │
                        │ Por academia ✓  │
                        └─────────────────┘
```

---

*Actualizado: 2024*
