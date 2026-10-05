#!/usr/bin/env bash
# Одна команда для macOS / Linux: ставит .NET 8 при необходимости и запускает API + фронт.
set -euo pipefail

ROOT="$(cd "$(dirname "$0")" && pwd)"
LOCAL_DOTNET="$ROOT/.dotnet"
export DOTNET_CLI_TELEMETRY_OPTOUT=1
export DOTNET_NOLOGO=1

has_sdk8() {
  command -v dotnet >/dev/null 2>&1 && dotnet --list-sdks 2>/dev/null | grep -q '^8\.'
}

if ! has_sdk8; then
  echo ".NET 8 не найден — скачиваю SDK в папку .dotnet ..."
  mkdir -p "$LOCAL_DOTNET"
  curl -sSL https://dot.net/v1/dotnet-install.sh | bash /dev/stdin --channel 8.0 --install-dir "$LOCAL_DOTNET"
  export DOTNET_ROOT="$LOCAL_DOTNET"
  export PATH="$LOCAL_DOTNET:$PATH"
fi

echo "Восстанавливаю пакеты..."
dotnet restore "$ROOT/POPs.sln"

echo
echo "Запускаю API (http://localhost:5080) и сайт (http://localhost:5081)"
echo "Остановка: Ctrl+C"
echo "Логины: admin/admin  или  student/student"
echo

cleanup() {
  if [[ -n "${API_PID:-}" ]]; then kill "$API_PID" 2>/dev/null || true; fi
  if [[ -n "${WEB_PID:-}" ]]; then kill "$WEB_PID" 2>/dev/null || true; fi
}
trap cleanup EXIT INT TERM

(
  cd "$ROOT/src/POPs.Api"
  dotnet run --launch-profile http
) &
API_PID=$!

(
  cd "$ROOT/src/POPs.Web"
  dotnet run --launch-profile http
) &
WEB_PID=$!

wait_http() {
  local url="$1"
  local i=0
  while [[ $i -lt 90 ]]; do
    if curl -fsS "$url" >/dev/null 2>&1; then
      return 0
    fi
    sleep 1
    i=$((i + 1))
  done
  echo "Не дождался ответа от $url"
  return 1
}

wait_http "http://localhost:5080/swagger/index.html"
wait_http "http://localhost:5081/"

if command -v open >/dev/null 2>&1; then
  open "http://localhost:5081"
elif command -v xdg-open >/dev/null 2>&1; then
  xdg-open "http://localhost:5081" >/dev/null 2>&1 || true
fi

echo "Готово. Сайт: http://localhost:5081   Swagger: http://localhost:5080/swagger"
wait
