# Одна команда для Windows: ставит .NET 8 при необходимости и запускает API + фронт.
$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Path
$LocalDotnet = Join-Path $Root ".dotnet"
$env:DOTNET_CLI_TELEMETRY_OPTOUT = "1"
$env:DOTNET_NOLOGO = "1"

function Test-Sdk8 {
    $dotnet = Get-Command dotnet -ErrorAction SilentlyContinue
    if (-not $dotnet) { return $false }
    $sdks = & dotnet --list-sdks 2>$null
    return ($sdks | Where-Object { $_ -like "8.*" }).Count -gt 0
}

if (-not (Test-Sdk8)) {
    Write-Host ".NET 8 не найден — скачиваю SDK в папку .dotnet ..."
    New-Item -ItemType Directory -Force -Path $LocalDotnet | Out-Null
    $installer = Join-Path $env:TEMP "dotnet-install.ps1"
    Invoke-WebRequest -Uri "https://dot.net/v1/dotnet-install.ps1" -OutFile $installer
    & $installer -Channel 8.0 -InstallDir $LocalDotnet
    $env:DOTNET_ROOT = $LocalDotnet
    $env:PATH = "$LocalDotnet;$env:PATH"
}

Write-Host "Восстанавливаю пакеты..."
dotnet restore (Join-Path $Root "POPs.sln")

Write-Host ""
Write-Host "Запускаю API (http://localhost:5080) и сайт (http://localhost:5081)"
Write-Host "Два окна можно закрыть, чтобы остановить приложение."
Write-Host "Логины: admin/admin  или  student/student"
Write-Host ""

$api = Start-Process -FilePath "dotnet" -ArgumentList "run","--launch-profile","http" `
    -WorkingDirectory (Join-Path $Root "src\POPs.Api") -PassThru
$web = Start-Process -FilePath "dotnet" -ArgumentList "run","--launch-profile","http" `
    -WorkingDirectory (Join-Path $Root "src\POPs.Web") -PassThru

function Wait-Http([string]$Url) {
    for ($i = 0; $i -lt 90; $i++) {
        try {
            Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 2 | Out-Null
            return
        } catch {
            Start-Sleep -Seconds 1
        }
    }
    throw "Не дождался ответа от $Url"
}

Wait-Http "http://localhost:5080/swagger/index.html"
Wait-Http "http://localhost:5081/"
Start-Process "http://localhost:5081"

Write-Host "Готово. Сайт: http://localhost:5081   Swagger: http://localhost:5080/swagger"
Write-Host "Нажмите Ctrl+C в этом окне, чтобы остановить оба процесса."

try {
    Wait-Process -Id @($api.Id, $web.Id)
} finally {
    if (-not $api.HasExited) { Stop-Process -Id $api.Id -Force -ErrorAction SilentlyContinue }
    if (-not $web.HasExited) { Stop-Process -Id $web.Id -Force -ErrorAction SilentlyContinue }
}
