@echo off
setlocal
rem Cambia a la carpeta raíz del proyecto (PROMPTY_3.0)
cd /d "%~dp0.."

echo Sincronizando dependencias con uv...
uv sync

echo Iniciando servidor API con uv...
uv run uvicorn api.server:app --host 0.0.0.0 --port 8000 --reload
