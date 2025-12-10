@echo off
REM Verificar si las dependencias están instaladas
call scripts\install_dependencies.ps1

REM Verificar el entorno
python scripts\check_env.py

REM Iniciar la aplicación
start MyPlanU.App.exe