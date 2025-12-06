#!/bin/bash
cd "$(dirname "$0")/.."

echo "Sincronizando dependencias con uv..."
uv sync

# Lanza PROMPTY en modo interfaz gráfica sin pedir confirmación
echo "s" | uv run python main.py
