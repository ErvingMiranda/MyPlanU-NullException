#!/bin/bash
# Script para iniciar la API de PROMPTY en Linux/Mac

# Cambiar al directorio raíz del proyecto (un nivel arriba de scripts/)
cd "$(dirname "$0")/.."

echo "Iniciando PROMPTY API..."

# Verificar si existe uv
if ! command -v uv &> /dev/null; then
    echo "uv no está instalado. Intentando usar python3 directamente..."
    python3 -m uvicorn api.server:app --reload --port 8000
else
    uv run uvicorn api.server:app --reload --port 8000
fi
