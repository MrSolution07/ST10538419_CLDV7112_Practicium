#!/usr/bin/env bash
set -euo pipefail

LOCATION="${LOCATION:-uaenorth}"
RG="${RG:-rg-final-cloud-dev}"
PREFIX="${PREFIX:-fcd}"
DEPLOY_STORAGE="${DEPLOY_STORAGE:-false}"

if [[ -z "${SQL_ADMIN_PASSWORD:-}" ]]; then
  echo "Set SQL_ADMIN_PASSWORD to a strong password before deploying." >&2
  exit 1
fi

az group create --name "$RG" --location "$LOCATION"

az deployment group create \
  --resource-group "$RG" \
  --template-file infra/main.bicep \
  --parameters infra/main.parameters.json \
  --parameters namePrefix="$PREFIX" sqlAdminPassword="$SQL_ADMIN_PASSWORD" deployStorage="$DEPLOY_STORAGE"

WEB_NAME="$(az deployment group show \
  --resource-group "$RG" \
  --name main \
  --query properties.outputs.webAppName.value -o tsv)"
SQL_SERVER="$(az deployment group show \
  --resource-group "$RG" \
  --name main \
  --query properties.outputs.sqlServerName.value -o tsv)"

if [[ -z "$WEB_NAME" || -z "$SQL_SERVER" ]]; then
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

dotnet publish src/FinalCloudDev.Web/FinalCloudDev.Web.csproj --configuration Release --output ./publish
az webapp deploy \
  --resource-group "$RG" \
  --name "$WEB_NAME" \
  --src-path ./publish \
  --type zip

HOST_NAME="$(az webapp show --resource-group "$RG" --name "$WEB_NAME" --query defaultHostName -o tsv)"
echo "Deployed https://${HOST_NAME}"
echo "Health https://${HOST_NAME}/health"
