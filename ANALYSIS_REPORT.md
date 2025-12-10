# Reporte de Análisis de Modelos de Dominio

Este documento resume el análisis de discrepancias entre el modelo de datos "oficial" y el uso real en el código de `MyPlanU`.

## 1. Usuario (`Usuario.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdUsuario` | ✅ Usado | Clave primaria, uso generalizado. |
| `Nombre` | ✅ Usado | Uso generalizado. |
| `Apellido` | ✅ Usado | Uso generalizado. |
| `Apodo` | ✅ Usado | Uso generalizado. |
| `Email` | ✅ Usado | Uso generalizado. |
| `ContrasenaHash` | ✅ Usado | Uso generalizado. |
| `AvatarUrl` | ⚠️ Dudoso | Definido en el modelo pero **sin referencias de uso** en la lógica de negocio o UI (solo definición). |
| `Pais` | ✅ Usado | Se usa en `EditarPerfilPage` y `EditarPerfilViewModel`. |
| `FechaRegistro` | ✅ Usado | Se asigna al registrar. |
| `EstadoCuenta` | ⚠️ Dudoso | Se asigna como "Activo" en `RegistroViewModel`, pero **nunca se lee ni valida** en ninguna parte. |
| `PreguntaSeguridad` | ✅ Usado | Se usa en `RegistroPage` y `EditarPerfilPage`. |
| `RespuestaSeguridad` | ✅ Usado | Se usa en `RegistroPage` y `EditarPerfilPage`. |

**Recomendación:**
- Marcar `AvatarUrl` como `[Obsolete]`.
- Marcar `EstadoCuenta` como `[Obsolete]` (o implementar lógica real para usarlo).

## 2. Amistad (`Amistad.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdAmistad` | ✅ Usado | PK. |
| `IdUsuarioPrincipal` | ✅ Usado | FK. |
| `IdUsuarioAmigo` | ✅ Usado | FK. |
| `Alias` | ❌ No Usado | Definido en el modelo pero **sin ninguna referencia** en el código (ni lectura ni escritura). |
| `EstadoSolicitud` | ✅ Usado | Crítico para la lógica de aceptación/rechazo ("Pendiente", "Aceptada"). |
| `FechaCreacion` | ✅ Usado | Auditoría básica. |
| `FechaActualizacion` | ✅ Usado | Auditoría básica. |

**Recomendación:**
- Marcar `Alias` como `[Obsolete]`.

## 3. PromptyConfig (`PromptyConfig.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdPrompty` | ✅ Usado | PK. |
| `IdUsuario` | ✅ Usado | FK. |
| `NombreAsistente` | ✅ Usado | Configuración básica. |
| `WakeWord` | ❌ No Usado | Definido pero **sin referencias** de uso real en la lógica del asistente. |
| `Activo` | ✅ Usado | Switch de activación. |
| `VozNombre` | ❌ No Usado | Definido pero **sin referencias**. |
| `VelocidadVoz` | ❌ No Usado | Definido pero **sin referencias**. |
| `VolumenVoz` | ❌ No Usado | Definido pero **sin referencias**. |
| `TonoVoz` | ❌ No Usado | Definido pero **sin referencias**. |
| `FechaCreacion` | ✅ Usado | Auditoría. |
| `FechaActualizacion` | ✅ Usado | Auditoría. |

**Recomendación:**
- Marcar `WakeWord`, `VozNombre`, `VelocidadVoz`, `VolumenVoz`, `TonoVoz` como `[Obsolete]`. Parece que la configuración de voz avanzada no está implementada aún.

## 4. Actividad (`Actividad.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdActividad` | ✅ Usado | PK. |
| `IdUsuarioCreador` | ✅ Usado | FK. |
| `Titulo` | ✅ Usado | Básico. |
| `Descripcion` | ✅ Usado | Básico. |
| `Prioridad` | ✅ Usado | Básico. |
| `Estado` | ✅ Usado | Básico. |
| `FechaCreacion` | ✅ Usado | Básico. |
| `FechaInicio` | ✅ Usado | Básico. |
| `FechaFin` | ✅ Usado | Básico. |
| `TodoElDia` | ✅ Usado | Básico. |
| `FechaCompletado` | ✅ Usado | Lógica de completado. |
| `NotaRapida` | ⚠️ Dudoso | Se copia en `ActividadCompartidaService` pero no parece tener uso en la UI principal de creación/edición. |
| `Etiquetas` | ✅ Usado | Se usa intensivamente en `CrearEventoViewModel` (parseo de string con pipes `|`). |
| `IdRecordatorio` | ⚠️ Dudoso | Existe la propiedad y la relación lógica, pero no hay código que enlace activamente una Actividad con un Recordatorio (no hay FK constraint real en SQLite ni lógica de navegación). |

**Recomendación:**
- Mantener `NotaRapida` y `IdRecordatorio` por ahora, pero revisar si realmente se van a implementar.

## 5. Recordatorio (`Recordatorio.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdRecordatorio` | ✅ Usado | PK. |
| `Titulo` | ✅ Usado | Básico. |
| `Mensaje` | ✅ Usado | Básico. |
| `FechaHora` | ✅ Usado | Básico. |
| `Canal` | ✅ Usado | Básico. |
| `Activo` | ✅ Usado | Básico. |
| `FechaCreacion` | ✅ Usado | Básico. |

**Nota:** Aunque la entidad `Recordatorio` existe y tiene repositorio/servicio, su integración con `Actividad` es muy débil (solo un campo ID opcional sin uso claro).

## 6. ActividadCompartida (`ActividadCompartida.cs`)

| Propiedad | Estado | Análisis |
| :--- | :--- | :--- |
| `IdCompartirActividad` | ✅ Usado | PK. |
| `IdActividad` | ✅ Usado | FK. |
| `IdUsuarioPropietario` | ✅ Usado | FK. |
| `IdUsuarioDestino` | ✅ Usado | FK. |
| `RolCompartido` | ✅ Usado | Se usa ("Lectura", "Editor"). |
| `EstadoCompartir` | ✅ Usado | Se usa ("Activo", "Inactivo"). |
| `FechaCompartida` | ✅ Usado | Auditoría. |
| `FechaActualizacion` | ✅ Usado | Auditoría. |

**Estado:** Entidad bien utilizada y coherente con la lógica de negocio.

---

## Plan de Acción

Se procederá a marcar con `[Obsolete]` las propiedades detectadas como no usadas para evitar su uso futuro sin romper la compilación actual, y se añadirán comentarios explicativos.

1.  **Usuario.cs**: `AvatarUrl`, `EstadoCuenta`.
2.  **Amistad.cs**: `Alias`.
3.  **PromptyConfig.cs**: `WakeWord`, `VozNombre`, `VelocidadVoz`, `VolumenVoz`, `TonoVoz`.

## Ejecución de Limpieza (Refactorización)

**Fecha:** 10 de Diciembre de 2025

Se ha realizado una limpieza completa del modelo de dominio y la documentación, eliminando definitivamente las propiedades marcadas anteriormente como obsoletas o no usadas.

**Cambios realizados:**

1.  **Eliminación de código:**
    *   `Usuario.cs`: Eliminadas `AvatarUrl` y `EstadoCuenta`.
    *   `Amistad.cs`: Eliminada `Alias`.
    *   `PromptyConfig.cs`: Eliminadas `WakeWord`, `VozNombre`, `VelocidadVoz`, `VolumenVoz`, `TonoVoz`.
    *   Se eliminaron las referencias en `RegistroViewModel.cs`.

2.  **Base de Datos:**
    *   Se generó la migración `Migration001_RemoveUnusedFields` para actualizar el esquema de SQLite (recreación de tablas para eliminar columnas).

3.  **Documentación (`INTEGRATION.md`):**
    *   Actualizadas las definiciones de las entidades para reflejar los campos eliminados.
    *   Unificada la sección de Prompty (Configuración y Modelos de Chat) en el punto 5.3.
    *   Agregada nota sobre el estado de integración de `Recordatorio`.

Este documento (`ANALYSIS_REPORT.md`) queda como registro histórico del análisis previo a la limpieza.
