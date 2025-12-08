# MyPlanU - NullException

**MyPlanU** es una aplicación de productividad y calendario diseñada específicamente para estudiantes, ayudándoles a organizar sus actividades, recordatorios y eventos académicos de manera eficiente.

Esta versión del proyecto integra **PROMPTY Lite**, una versión ligera del asistente de escritorio inteligente PROMPTY 3.0, que permite a los usuarios interactuar con una IA conversacional directamente desde la aplicación para resolver dudas o generar ideas.

---

## 🏗️ Arquitectura y Estructura del Proyecto

La solución sigue una arquitectura limpia dividida en capas para asegurar mantenibilidad y escalabilidad.

### Estructura de la Solución (`MyPlanU.sln`)

*   **`MyPlanU.App/` (Frontend - .NET MAUI)**
    *   Es la aplicación cliente multiplataforma (Android/Windows).
    *   Sigue el patrón **MVVM** (Model-View-ViewModel).
    *   **Pages**: Contiene las vistas XAML (`LoginPage`, `EventosPage`, `PromptyPage`, etc.).
    *   **ViewModels**: Lógica de presentación y estado de las vistas.
    *   **Services**: Servicios de cliente, incluyendo `PromptyLiteHttpClient` para comunicarse con la API de Python.

*   **`MyPlanU.Backend/` (Backend & Data - .NET Class Library)**
    *   Contiene la lógica de negocio y acceso a datos.
    *   **Models**: Definición de entidades (`Usuario`, `Actividad`, `Recordatorio`).
    *   **Data**: Contexto de base de datos SQLite y repositorios.
    *   **Business**: Servicios de dominio (`ActividadService`, `AuthService`).

*   **`PROMPTY_3.0/` (Módulo de IA - Python)**
    *   Contiene el código fuente de PROMPTY 3.0 adaptado para funcionar como servicio.
    *   Expone una **API REST** (FastAPI) que MyPlanU consume localmente.
    *   Utiliza **Hugging Face** como proveedor de inteligencia artificial.

---

## 🛠️ Dependencias y Requisitos

### Para MyPlanU (App)
*   **.NET 10 SDK** (o la versión compatible con MAUI especificada en el proyecto).
*   Visual Studio 2022 (o VS Code con extensiones de C# y MAUI) con las cargas de trabajo de MAUI instaladas.

### Para PROMPTY 3.0 (IA)
PROMPTY funciona como un servicio local en Python. No es necesario para compilar la app, pero sí para usar las funciones de IA.

*   **Python 3.10+** instalado.
*   **uv** (Gestor de paquetes de Python rápido).

Para preparar el entorno de Python:
1.  Abre una terminal en la carpeta `PROMPTY_3.0`.
2.  Ejecuta `uv sync` para instalar todas las dependencias necesarias en un entorno virtual.

---

## ✨ Funcionalidades

### Actuales
*   **Gestión de Usuarios**: Registro e inicio de sesión seguro.
*   **Gestión de Eventos**: Crear, editar, eliminar y listar eventos académicos.
    *   **Descripción Opcional**: Agiliza la creación de eventos rápidos.
    *   **Confirmaciones**: Mensajes claros de éxito al crear, editar o eliminar.
    *   **Seguridad**: Confirmación antes de eliminar eventos importantes.
*   **Sistema de Amigos**: Buscar usuarios, enviar solicitudes y gestionar amistades.
    *   **Privacidad**: Mensajes de solicitud discretos.
    *   **Organización**: Lista de amigos priorizada sobre solicitudes pendientes.
*   **Compartir Actividades**:
    *   **Modo Copia (Independiente)**: Envía una copia del evento a un amigo para que lo gestione por su cuenta.
    *   **Modo Compartido (Colaborativo)**: Comparte el evento real para que ambos puedan editarlo y ver los cambios en tiempo real.
*   **Integración con PROMPTY Lite**: Chat con asistente de IA para consultas rápidas.
*   **Configuración Básica**:
    *   Cambio de tema (Claro/Oscuro).
    *   Cambio de contraseña.
    *   Edición de perfil.

### En Desarrollo (Próximamente)
*   Sistema de Notificaciones y Alertas.
*   Creación automática de eventos desde el chat de IA.

---

## 🔑 Configuración del Token de Hugging Face

Por razones de seguridad, **este repositorio NO incluye claves de API**. Para utilizar las funciones inteligentes de PROMPTY, necesitas tu propio token de Hugging Face (es gratuito).

### ¿Cómo configurarlo?
No necesitas editar archivos manualmente. MyPlanU incluye un flujo de configuración integrado:

1.  Abre la aplicación MyPlanU.
2.  Intenta acceder a la sección de **PROMPTY**.
3.  Si no tienes un token configurado, la app te pedirá que lo ingreses.
4.  El token se validará contra la API y, si es correcto, se guardará localmente de forma segura (en `config_local.json`, que es ignorado por Git).

> **Nota:** Si no configuras un token, podrás usar la app normalmente, pero al intentar entrar al chat de PROMPTY recibirás un aviso para configurarlo.

---

## 🚀 Instrucciones de Ejecución

Sigue estos pasos para levantar todo el ecosistema (App + IA).

### 1. Clonar el repositorio
```bash
git clone https://github.com/ErvingMiranda/MyPlanU-NullException.git
cd MyPlanU-NullException
```

### 2. Levantar la API de PROMPTY (Backend IA)
Es necesario que este servicio esté corriendo para que la app pueda hablar con la IA.

**En Windows:**
*   Ve a la carpeta `PROMPTY_3.0/scripts`.
*   Ejecuta `start_prompty_api.bat`.

**En Linux/Mac:**
*   Ve a la carpeta `PROMPTY_3.0/scripts`.
*   Ejecuta `chmod +x start_prompty_api.sh` (si es necesario).
*   Ejecuta `./start_prompty_api.sh`.
*   Ejecuta `start_prompty_api.bat`.

**En Linux/Mac (o terminal manual):**
```bash
cd PROMPTY_3.0
uv run uvicorn api.server:app --reload --port 8000
```
*Verás que el servidor inicia en `http://127.0.0.1:8000`.*

### 3. Ejecutar MyPlanU
1.  Abre `MyPlanU.sln` en Visual Studio o la carpeta en VS Code.
2.  Restaura los paquetes NuGet (debería ser automático).
3.  Selecciona tu dispositivo de destino (Android Emulator o Windows Machine).
4.  Ejecuta la aplicación (F5).

### 4. Probar la integración
1.  Inicia sesión en MyPlanU (puedes registrar un usuario nuevo).
2.  Ve a la pantalla de Eventos.
3.  Toca el botón para abrir **PROMPTY**.
4.  Sigue las instrucciones para ingresar tu token de Hugging Face.
5.  ¡Empieza a chatear con tu asistente!

---

## 📄 Licencias

Este proyecto es una colaboración que integra dos bases de código distintas:

*   **MyPlanU**: Propiedad del equipo **NullException**.
*   **PROMPTY 3.0**: Propiedad del equipo **PROMPTY**.

El uso de PROMPTY Lite dentro de MyPlanU ha sido autorizado por los creadores originales. Ambas partes del código se distribuyen respetando la autoría de sus respectivos equipos.

---

## 👥 Créditos e Historia

Este proyecto es la culminación del esfuerzo de tres equipos diferentes a lo largo del tiempo, uniendo tecnologías y visiones, con **Erving Miranda** como hilo conductor en todos ellos (11 colaboradores en total).

### 1. Equipo PROMPTY (Tecnología de IA Original)
Creadores del asistente de escritorio PROMPTY 3.0, cuya tecnología "Lite" impulsa la inteligencia de esta aplicación.
*   **Owen Bravo**
*   **María Carrasco**
*   **Liang Zúñiga**
*   **Erving Miranda**

### 2. Equipo FluxBoard (Prototipo Móvil - Hackathon Nicaragua 2025)
Desarrollaron la primera versión móvil de la agenda (sin IA) para la competencia Hackathon Nicaragua 2025.
*   **Garry Johann Velasquez Cruz**
*   **Erving Josué Miranda Ríos**
*   **Sonia del Carmen Pérez Valdivia**
*   **Mirna Samanta Ulloa**
*   **Gerson Isaías Ramírez**

### 3. Equipo NullException (Desarrolladores de MyPlanU Actual)
El equipo actual que ha integrado todo, creando la versión definitiva de MyPlanU con la potencia de PROMPTY.
*   **Erving Miranda**
*   **Fernando Zapata**
*   **Mery López**
*   **Osman Cerpas**

**MyPlanU** combina la gestión de productividad académica con la potencia de **PROMPTY** para ofrecer una experiencia única a los estudiantes.
