@echo off
setlocal
rem Cambia a la carpeta raíz del proyecto (PROMPTY_3.0)
cd /d "%~dp0.."

echo Sincronizando dependencias con uv...
uv sync

rem Lanza PROMPTY en modo interfaz gráfica sin pedir confirmación
(echo s) | uv run python main.py
