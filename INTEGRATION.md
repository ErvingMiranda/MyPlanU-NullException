# MyPlanU 2.0 – Documento de Integración Oficial
Este documento establece los lineamientos técnicos, arquitectónicos y de datos que deben seguir todas las partes del proyecto al momento de unificar el código.

Su objetivo es asegurar coherencia entre equipos y facilitar la integración de las pantallas, backend y base de datos en un solo proyecto MAUI moderno y mantenible.

---

## 1. Tecnologías oficiales

**Framework general:**  
- .NET 10 (LTS)

**UI oficial:**  
- .NET MAUI 10.x (versión estable más reciente)

**TargetFrameworks para el proyecto MAUI (`MyPlanU.App`):**
- `net10.0-android`
- `net10.0-windows10.0.19041.0`

*(iOS y MacCatalyst podrán agregarse en el futuro si es necesario)*

**TargetFramework para la biblioteca de backend (`MyPlanU.Backend`):**
- `net10.0`

**Archivo recomendado `global.json`:**
```json
{
  "sdk": {
    "version": "10.0.100",
    "rollForward": "latestFeature"
  }
}
```

---

## 2. Paquetes NuGet utilizados

### Para `MyPlanU.App` (MAUI)
- `Microsoft.Maui.Controls` (10.0.11)
- `Microsoft.Maui.Controls.Compatibility` (10.0.11)
- `Microsoft.Maui.Essentials` (10.0.11)
- `CommunityToolkit.Maui` (13.0.0)
- `CommunityToolkit.Mvvm` (8.4.0)
- `SQLitePCLRaw.bundle_e_sqlite3` (3.0.2)
- `sqlite-net-pcl` (1.10.196-beta)
- `Microsoft.Extensions.Logging.Debug` (10.0.0)
- `Microsoft.Extensions.Http` (8.0.0)

### Para `MyPlanU.Backend` (Class Library)
- `CommunityToolkit.Mvvm` (8.4.0)
- `SQLitePCLRaw.bundle_e_sqlite3` (3.0.2)
- `sqlite-net-pcl` (1.10.196-beta)
- `Microsoft.Extensions.DependencyInjection` (10.0.0)
- `Microsoft.Extensions.Logging` (10.0.0)
- `Microsoft.Extensions.Configuration` (10.0.0)

**Reglas:**
- Usar siempre las versiones más nuevas y compatibles.
- No usar paquetes heredados de Xamarin.
- No usar Essentials antiguo: MAUI ya integra sus APIs.
- Reemplazar APIs obsoletas al unificar.

---

## 3. Estructura final de la solución

```
MyPlanU.sln
├─ global.json
├─ MyPlanU.App/               # Proyecto MAUI principal
│  ├─ App.xaml / App.xaml.cs
│  ├─ AppShell.xaml / AppShell.xaml.cs
│  ├─ MauiProgram.cs
│  ├─ Pages/
│  │  ├─ LoginPage.xaml(.cs)
│  │  ├─ RegistroPage.xaml(.cs)
│  │  ├─ EventosPage.xaml(.cs)
│  │  ├─ CrearEventoPage.xaml(.cs)
│  │  ├─ ConfiguracionPage.xaml(.cs)
│  │  ├─ CambiarPasswordPage.xaml(.cs)
│  │  ├─ PromptyPage.xaml(.cs)
│  │  └─ AmigosPage.xaml(.cs)
│  ├─ ViewModels/
│  │  ├─ LoginViewModel.cs
│  │  ├─ RegistroViewModel.cs
│  │  ├─ EventosViewModel.cs
│  │  ├─ CrearEventoViewModel.cs
│  │  ├─ ConfiguracionViewModel.cs
│  │  ├─ CambiarPasswordViewModel.cs
│  │  ├─ PromptyViewModel.cs
│  │  ├─ AmigosViewModel.cs
│  │  └─ ValidationHelper.cs
│  └─ Platforms/              # Generado automáticamente por MAUI
│
└─ MyPlanU.Backend/           # Lógica de negocio y acceso a datos
   ├─ Models/
   │  ├─ Usuario.cs
   │  ├─ Amistad.cs
   │  ├─ PromptyConfig.cs
   │  ├─ Actividad.cs
   │  ├─ Recordatorio.cs
   │  ├─ ActividadCompartida.cs
   │  ├─ ChatMessage.cs
   │  ├─ PromptyChatRequest.cs
   │  └─ ...
   ├─ Data/
   │  ├─ SQLiteContext.cs
   │  └─ Repositories/
   └─ Business/
      ├─ AuthService.cs
      ├─ UserService.cs
      ├─ AmistadService.cs
      ├─ ActividadService.cs
      ├─ ActividadCompartidaService.cs
      └─ ...
```

---

## 4. Modelos de Datos (Entidades)

### Usuario (`Usuario.cs`)
```csharp
public class Usuario
{
    [PrimaryKey, AutoIncrement]
    public int IdUsuario { get; set; }
    [Unique]
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string Nombre { get; set; }
    public string Apodo { get; set; }
    public string Carrera { get; set; }
    public string Universidad { get; set; }
    public DateTime FechaRegistro { get; set; }
    public string FotoPerfilPath { get; set; }
}
```

### Actividad (`Actividad.cs`)
```csharp
public class Actividad
{
    [PrimaryKey, AutoIncrement]
    public int IdActividad { get; set; }
    [Indexed]
    public int IdUsuarioCreador { get; set; }
    public string Titulo { get; set; }
    public string Descripcion { get; set; } // Opcional
    public string Prioridad { get; set; }
    public string Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime FechaInicio { get; set; }
    public DateTime FechaFin { get; set; }
    public bool TodoElDia { get; set; }
    public string Etiquetas { get; set; }
}
```

### ActividadCompartida (`ActividadCompartida.cs`)
```csharp
public class ActividadCompartida
{
    [PrimaryKey, AutoIncrement]
    public int IdCompartirActividad { get; set; }
    [Indexed]
    public int IdActividad { get; set; }
    [Indexed]
    public int IdUsuarioPropietario { get; set; }
    [Indexed]
    public int IdUsuarioDestino { get; set; }
    public string RolCompartido { get; set; } // "Lectura", "Editor"
    public string EstadoCompartir { get; set; } // "Activo", "Inactivo"
    public DateTime FechaCompartida { get; set; }
    public DateTime FechaActualizacion { get; set; }
}
```

---

## 5. Lógica de Negocio Clave

### Compartir Eventos
El sistema soporta dos modos de compartir:
1.  **Copia (Independiente)**: Se crea un nuevo registro `Actividad` para el usuario destino, copiando los datos del original. Son eventos independientes.
2.  **Compartido (Colaborativo)**: Se crea un registro en `ActividadCompartida` vinculando el evento original con el usuario destino. Ambos acceden al mismo registro de `Actividad`.

### Amigos
- Las solicitudes de amistad deben ser aceptadas para que los usuarios aparezcan en la lista de "Mis Amigos".
- La lista de amigos tiene prioridad visual sobre las solicitudes pendientes.

---
   │  ├─ PromptyChatResponse.cs
   │  ├─ PromptyChatRequest.cs
   │  └─ PromptyChatResponse.cs
   ├─ Data/
   │  ├─ SQLiteContext.cs
   │  └─ Repositories/
   │     └─ (Repositorios implementados o integrados en servicios)
   ├─ Business/
   │  ├─ AuthService.cs
   │  ├─ UserService.cs
   │  ├─ ActividadService.cs
   │  ├─ RecordatorioService.cs
   │  ├─ ActividadCompartidaService.cs
   │  └─ AmistadService.cs
   └─ Services/
      ├─ IPromptyLiteClient.cs
      ├─ PromptyLiteHttpClient.cs
      └─ PromptyLauncher.cs
```

### Reglas arquitectónicas
- `MyPlanU.App` **solo** usa servicios (Business/Services).
- Business usa repositorios (Data) o acceso a datos directo vía SQLiteContext.
- Data usa SQLite.
- La UI **nunca** toca la base de datos directamente.
- Namespaces recomendados:
  - `MyPlanU.App.*`
  - `MyPlanU.Backend.Models`
  - `MyPlanU.Backend.Data`
  - `MyPlanU.Backend.Business`
  - `MyPlanU.Backend.Services`

---

## 4. Convenciones de nombres

**Clases:** PascalCase  
Ej.: `Usuario`, `LoginPage`, `EventosService`.

**Métodos:** PascalCase  
Ej.: `ValidarCredenciales`, `ObtenerEventosPorUsuario`.

**Variables y parámetros:** camelCase  
Ej.: `usuarioActual`, `email`, `eventoSeleccionado`.

**Campos privados:** `_camelCase`  
Ej.: `_repositorioUsuarios`.

**Páginas MAUI:**  
`LoginPage.xaml`, `EventosPage.xaml`, `ConfiguracionPage.xaml`, `RegistroPage.xaml`, `PromptyPage.xaml`.

---

## 5. Modelo de datos oficial

El sistema usa las siguientes entidades principales.

### 5.1. Usuario
- `id_usuario` (PK)  
- `nombre`  
- `apellido`  
- `apodo`  
- `email`  
- `contrasena_hash`  
- `avatar_url`  
- `pais`  
- `fecha_registro`  
- `estado_cuenta`
- `pregunta_seguridad`
- `respuesta_seguridad`

Relaciones:
- 1:1 con PROMPTY  
- 1:N con Actividad  
- N:N mediante Amistad  
- N:N mediante ActividadCompartida (como propietario o destino)

---

### 5.2. Amistad
- `id_amistad` (PK)  
- `id_usuario_principal` (FK → Usuario)  
- `id_usuario_amigo` (FK → Usuario)  
- `alias`  
- `estado_solicitud`  
- `fecha_creacion`  
- `fecha_actualizacion`

Es la tabla para solicitudes de amistad y contactos.

---

### 5.3. PROMPTY (configuración)
- `id_prompty` (PK)  
- `id_usuario` (FK → Usuario) [UNIQUE: relación 1:1]  
- `nombre_asistente`  
- `wake_word`  
- `activo`  
- `voz_nombre`  
- `velocidad_voz`  
- `volumen_voz`  
- `tono_voz`  
- `fecha_creacion`  
- `fecha_actualizacion`

Guarda preferencias del asistente del usuario.

---

### 5.4. Actividad
- `id_actividad` (PK)  
- `id_usuario_creador` (FK → Usuario)  
- `titulo`  
- `descripcion`  
- `prioridad`  
- `estado`  
- `fecha_creacion`  
- `fecha_inicio`  
- `fecha_fin`  
- `todo_el_dia`  
- `fecha_completado`  
- `nota_rapida`  
- `id_recordatorio` (FK → Recordatorio) [opcional]

Relacionado con Recordatorio y con ActividadCompartida.

---

### 5.5. Recordatorio
- `id_recordatorio` (PK)  
- `titulo`  
- `mensaje`  
- `fecha_hora`  
- `canal`  
- `activo`  
- `fecha_creacion`

Puede asociarse opcionalmente a una Actividad.

---

### 5.6. ActividadCompartida
- `id_compartir_actividad` (PK)  
- `id_actividad` (FK → Actividad)  
- `id_usuario_propietario` (FK → Usuario)  
- `id_usuario_destino` (FK → Usuario)  
- `rol_compartido`  
- `estado_compartir`  
- `fecha_compartida`  
- `fecha_actualizacion`

Permite compartir actividades con otros usuarios y administrar permisos.

### 5.7. Modelos de Chat (Prompty)
- `ChatMessage`: Representa un mensaje en el chat (Role, Content).
- `PromptyChatRequest`: Estructura para enviar peticiones al servicio de IA.
- `PromptyChatResponse`: Estructura de respuesta del servicio de IA.

---

## 6. Flujo de navegación

Flujo estándar:

1. La aplicación inicia en **LoginPage**.  
2. Opción de ir a **RegistroPage** para crear cuenta.
3. Si las credenciales son correctas → **EventosPage** (Dashboard principal).  
4. Desde **EventosPage**, el usuario puede:
   - Crear un nuevo evento → **CrearEventoPage**.
   - Abrir el asistente IA → **PromptyPage**.  
   - Ir a **ConfiguracionPage**.  
   - Cerrar sesión → vuelve a Login.  

### Flujo de Compartir Actividades
Desde **EventosPage**:
1. El usuario selecciona "Compartir" en una actividad propia.
2. Se despliega un menú de gestión:
   - **Compartir con amigo**: Muestra lista de amigos confirmados para seleccionar.
   - **Ver compartidos / Dejar de compartir**: Muestra usuarios con acceso y permite revocarlo.
3. Las actividades compartidas por otros aparecen en una sección diferenciada "Compartidas Conmigo".

La navegación está implementada con `.NET MAUI Shell` (`AppShell`).

---

## 7. Rol de este documento

Este documento define:

- Las versiones oficiales de .NET, MAUI y paquetes NuGet.  
- La estructura final de la solución.  
- Las reglas arquitectónicas entre capas.  
- El modelo de datos oficial del sistema.  
- Las convenciones de nombres.  
- El flujo de navegación autorizado.  

Todo código integrado o modificado debe ajustarse a estas reglas para asegurar coherencia y mantenibilidad del proyecto.
