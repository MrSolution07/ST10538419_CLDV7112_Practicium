#!/usr/bin/env bash
set -euo pipefail

LOCATION="${LOCATION:-uaenorth}"
RG="${RG:-rg-st10538419-cldv7112}"
FN_RG="${FN_RG:-${RG}-fn}"
PREFIX="${PREFIX:-st10538419}"
ROOT="$(cd "$(dirname "$0")/.." && pwd)"
cd "$ROOT"
SECRET_FILE="$ROOT/infra/.deploy.env"

if [[ -z "${SQL_ADMIN_PASSWORD:-}" && -f "$SECRET_FILE" ]]; then
  set -a
  # shellcheck disable=SC1090
  source "$SECRET_FILE"
  set +a
fi

if [[ -z "${SQL_ADMIN_PASSWORD:-}" ]]; then
  umask 077
  SQL_ADMIN_PASSWORD="Aa1$(openssl rand -hex 12)!"
  printf 'SQL_ADMIN_PASSWORD=%s\n' "$SQL_ADMIN_PASSWORD" > "$SECRET_FILE"
  echo "Wrote a SQL admin password to infra/.deploy.env. That file stays off git."
fi

az group create --name "$RG" --location "$LOCATION"
az group create --name "$FN_RG" --location "$LOCATION"

az deployment group create \
  --resource-group "$RG" \
  --template-file infra/main.bicep \
  --parameters infra/main.parameters.json \
  --parameters namePrefix="$PREFIX" sqlAdminPassword="$SQL_ADMIN_PASSWORD" functionResourceGroup="$FN_RG"

WEB_NAME="$(az deployment group show \
  --resource-group "$RG" \
  --name main \
  --query properties.outputs.webAppName.value -o tsv)"
SQL_SERVER="$(az deployment group show \
  --resource-group "$RG" \
  --name main \
  --query properties.outputs.sqlServerName.value -o tsv)"
FUNCTION_NAME="$(az deployment group show \
  --resource-group "$RG" \
  --name main \
  --query properties.outputs.functionAppName.value -o tsv)"

if [[ -z "$WEB_NAME" || -z "$SQL_SERVER" || -z "$FUNCTION_NAME" ]]; then
  echo "Deployment finished, but outputs were missing. Check the resource group in the portal." >&2
  exit 1
fi

CLIENT_IP="$(curl -fsS https://api.ipify.org)"
az sql server firewall-rule create \
  --resource-group "$RG" \
  --server "$SQL_SERVER" \
  --name "Client-${CLIENT_IP//./-}" \
  --start-ip-address "$CLIENT_IP" \
  --end-ip-address "$CLIENT_IP"

rm -rf "$ROOT/publish" "$ROOT/publish-functions"
dotnet publish "$ROOT/src/ST10538419_CLDV7112_Practicium.Web/ST10538419_CLDV7112_Practicium.Web.csproj" --configuration Release --output "$ROOT/publish"
WEB_ZIP="$(mktemp -d)/web.zip"
(cd "$ROOT/publish" && zip -qr "$WEB_ZIP" .)
az webapp deploy \
  --resource-group "$RG" \
  --name "$WEB_NAME" \
  --src-path "$WEB_ZIP" \
  --type zip

dotnet publish "$ROOT/src/ST10538419_CLDV7112_Practicium.Functions/ST10538419_CLDV7112_Practicium.Functions.csproj" --configuration Release --output "$ROOT/publish-functions"
FN_ZIP="$(mktemp -d)/functions.zip"
(cd "$ROOT/publish-functions" && zip -qr "$FN_ZIP" .)
az functionapp deploy \
  --resource-group "$FN_RG" \
  --name "$FUNCTION_NAME" \
  --src-path "$FN_ZIP" \
  --type zip

HOST_NAME="$(az webapp show --resource-group "$RG" --name "$WEB_NAME" --query defaultHostName -o tsv)"
FUNCTION_HOST="$(az functionapp show --resource-group "$FN_RG" --name "$FUNCTION_NAME" --query defaultHostName -o tsv)"
echo "Site https://${HOST_NAME}"
echo "Health https://${HOST_NAME}/health"
echo "Functions https://${FUNCTION_HOST}/api/health"

for _ in 1 2 3 4 5 6 7 8 9 10 11 12; do
  if curl -fsS "https://${HOST_NAME}/health" >/dev/null; then
    echo "Web health check passed."
    break
  fi
  sleep 20
done
