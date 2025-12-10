#!/bin/bash

# Verificar si las dependencias están instaladas
if ! command -v python3 &> /dev/null; then
    echo "Python3 no está instalado. Instalando..."
    sudo apt update
    sudo apt install python3 -y
fi

if ! command -v pip3 &> /dev/null; then
    echo "Pip no está instalado. Instalando..."
    sudo apt install python3-pip -y
fi

# Ejecutar el script de instalación de dependencias
if [ -f "./scripts/install_dependencies.sh" ]; then
    bash ./scripts/install_dependencies.sh
else
    echo "El script de instalación de dependencias no se encontró."
    exit 1
fi

# Ejecutar el script de verificación del entorno
if [ -f "./scripts/check_env.py" ]; then
    python3 ./scripts/check_env.py
else
    echo "El script de verificación del entorno no se encontró."
    exit 1
fi

# Iniciar la aplicación
echo "Iniciando la aplicación..."
# Aquí se debe agregar el comando para iniciar la aplicación, por ejemplo:
# dotnet run o el comando específico para su aplicación.