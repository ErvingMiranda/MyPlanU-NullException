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

## 2. Paquetes NuGet recomendados

### Para `MyPlanU.App` (MAUI)
- `CommunityToolkit.Maui`
- `CommunityToolkit.Mvvm`
- `SQLitePCLRaw.bundle_e_sqlite3`
- `sqlite-net-pcl`

### Para `MyPlanU.Backend` (Class Library)
- `SQLitePCLRaw.bundle_e_sqlite3`
- `sqlite-net-pcl`
- `Microsoft.Extensions.DependencyInjection`
- `Microsoft.Extensions.Logging`
- `Microsoft.Extensions.Configuration` *(si aplica)*

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
│  ├─ MauiProgram.cs
│  ├─ Pages/
│  │  ├─ LoginPage.xaml(.cs)
│  │  ├─ EventosPage.xaml(.cs)
│  │  └─ ConfiguracionPage.xaml(.cs)
│  ├─ ViewModels/
│  │  ├─ LoginViewModel.cs
│  │  ├─ EventosViewModel.cs
│  │  └─ ConfiguracionViewModel.cs
│  └─ Platforms/              # Generado automáticamente por MAUI
│
└─ MyPlanU.Backend/           # Lógica de negocio y acceso a datos
   ├─ Models/
   │  ├─ Usuario.cs
   │  ├─ Amistad.cs
   │  ├─ PromptyConfig.cs
   │  ├─ Actividad.cs
   │  ├─ Recordatorio.cs
   │  └─ ActividadCompartida.cs
   ├─ Data/
   │  ├─ SQLiteContext.cs
   │  └─ Repositories/
   │     ├─ IUserRepository.cs / UserRepository.cs
   │     ├─ IActividadRepository.cs / ActividadRepository.cs
   │     ├─ IRecordatorioRepository.cs / RecordatorioRepository.cs
   │     └─ IActividadCompartidaRepository.cs / ActividadCompartidaRepository.cs
   └─ Business/
      ├─ AuthService.cs
      ├─ ActividadService.cs
      ├─ AmistadService.cs
      ├─ PromptyService.cs
      ├─ RecordatorioService.cs
      └─ ActividadCompartidaService.cs
```

### Reglas arquitectónicas
- `MyPlanU.App` **solo** usa servicios (Business).
- Business usa repositorios (Data).
- Data usa SQLite.
- La UI **nunca** toca la base de datos directamente.
- Namespaces recomendados:
  - `MyPlanU.App.*`
  - `MyPlanU.Backend.Models`
  - `MyPlanU.Backend.Data`
  - `MyPlanU.Backend.Business`

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
`LoginPage.xaml`, `EventosPage.xaml`, `ConfiguracionPage.xaml`.

---

## 5. Modelo de datos oficial

El sistema usa seis entidades principales. Los nombres aquí son los oficiales y se reflejarán tanto en la BD como en las clases C#.

### 5.1. Usuario
- `id_usuario` (PK)  
- `nombre`  
- `apellido`  
- `apodo`  
- `email`  
- `contrasena_hash`  
- `avatar_url`  
- `pais`  
- `zona_horaria`  
- `fecha_registro`  
- `estado_cuenta`

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

---

## 6. Flujo de navegación

Flujo estándar:

1. La aplicación inicia en **LoginPage**.  
2. Si las credenciales son correctas → **EventosPage**.  
3. Desde **EventosPage**, el usuario puede:
   - Abrir PROMPTY.  
   - Ir a **ConfiguracionPage** (placeholder por ahora).  
   - Cerrar sesión → vuelve a Login.  
   - Salir del sistema.

La navegación será implementada con el modelo recomendado de `.NET MAUI Shell` o `NavigationPage`.

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
