# MyPlanU Launcher

## Descripción
MyPlanU Launcher es una herramienta diseñada para facilitar el inicio de la aplicación MyPlanU asegurando que todas las dependencias necesarias estén instaladas y configuradas correctamente en el sistema del usuario.

## Estructura del Proyecto
El proyecto contiene los siguientes archivos:

- **scripts/install_dependencies.ps1**: Script de PowerShell para instalar las dependencias necesarias en sistemas Windows.
- **scripts/install_dependencies.sh**: Script de shell para instalar las dependencias necesarias en sistemas Unix/Linux.
- **scripts/check_env.py**: Script en Python que verifica el entorno del sistema y las variables de entorno necesarias.
- **launcher.bat**: Script por lotes de Windows que ejecuta el launcher y llama a los scripts de instalación de dependencias.
- **launcher.sh**: Script de shell que ejecuta el launcher en sistemas Unix/Linux y asegura que las dependencias estén instaladas.

## Instrucciones de Uso

### Para Windows:
1. Asegúrate de tener PowerShell instalado.
2. Ejecuta el archivo `launcher.bat` haciendo doble clic o desde la línea de comandos.
3. El script verificará e instalará las dependencias necesarias antes de iniciar la aplicación.

### Para Unix/Linux:
1. Asegúrate de tener un terminal de shell disponible.
2. Ejecuta el archivo `launcher.sh` desde la terminal:
   ```bash
   chmod +x launcher.sh
   ./launcher.sh
   ```
3. El script verificará e instalará las dependencias necesarias antes de iniciar la aplicación.

## Notas
- Asegúrate de tener permisos adecuados para ejecutar scripts en tu sistema.
- Si encuentras algún problema, verifica los logs generados por los scripts de instalación para obtener más información sobre las dependencias que no se pudieron instalar.