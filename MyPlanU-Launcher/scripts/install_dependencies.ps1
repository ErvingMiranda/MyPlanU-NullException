# Este archivo es un script de PowerShell que se encarga de instalar las dependencias necesarias para la aplicación.

# Verifica si las herramientas requeridas están instaladas y las instala si no lo están.

# Lista de herramientas requeridas
$requiredTools = @("dotnet", "git", "curl")

# Función para verificar si una herramienta está instalada
function Check-Tool {
    param (
        [string]$tool
    )
    $toolPath = Get-Command $tool -ErrorAction SilentlyContinue
    return $toolPath -ne $null
}

# Función para instalar una herramienta
function Install-Tool {
    param (
        [string]$tool
    )
    switch ($tool) {
        "dotnet" {
            Write-Host "Instalando .NET SDK..."
            # Comando para instalar .NET SDK
            Start-Process "winget" -ArgumentList "install Microsoft.DotNet.SDK" -Wait
        }
        "git" {
            Write-Host "Instalando Git..."
            # Comando para instalar Git
            Start-Process "winget" -ArgumentList "install Git.Git" -Wait
        }
        "curl" {
            Write-Host "Instalando Curl..."
            # Comando para instalar Curl
            Start-Process "winget" -ArgumentList "install Curl.Curl" -Wait
        }
        default {
            Write-Host "No se puede instalar $tool. Verifique el nombre de la herramienta."
        }
    }
}

# Verificar e instalar herramientas requeridas
foreach ($tool in $requiredTools) {
    if (-not (Check-Tool $tool)) {
        Write-Host "$tool no está instalado. Instalando..."
        Install-Tool $tool
    } else {
        Write-Host "$tool ya está instalado."
    }
}

Write-Host "Todas las dependencias necesarias están instaladas."