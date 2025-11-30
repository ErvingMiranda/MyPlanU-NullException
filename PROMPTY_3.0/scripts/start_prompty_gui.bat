@echo off
setlocal
rem Cambia a la carpeta raíz del proyecto (PROMPTY_3.0)
cd /d "%~dp0.."

rem Activa el entorno virtual si existe
if exist "venv\Scripts\activate.bat" (
    call "venv\Scripts\activate.bat"
)

rem Inicia el servidor API en segundo plano (ventana minimizada)
start "Prompty API Server" /min python -m uvicorn api.server:app --host 0.0.0.0 --port 8000

rem Lanza PROMPTY en modo interfaz gráfica sin pedir confirmación
(echo s) | python main.py
