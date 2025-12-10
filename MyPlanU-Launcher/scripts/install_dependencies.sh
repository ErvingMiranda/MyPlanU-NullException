#!/bin/bash

# Función para instalar una dependencia si no está instalada
install_dependency() {
    if ! command -v "$1" &> /dev/null; then
        echo "$1 no está instalado. Instalando..."
        sudo apt-get install -y "$1"
    else
        echo "$1 ya está instalado."
    fi
}

# Lista de dependencias necesarias
dependencies=(
    "curl"
    "git"
    "python3"
    "python3-pip"
    "dotnet-sdk-6.0"  # Ajusta la versión según sea necesario
)

# Actualizar la lista de paquetes
echo "Actualizando la lista de paquetes..."
sudo apt-get update

# Instalar cada dependencia
for dependency in "${dependencies[@]}"; do
    install_dependency "$dependency"
done

echo "Todas las dependencias han sido instaladas."