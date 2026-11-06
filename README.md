# ArgiStream IoT

Management site for ArgiStream IoT, student ST10538419. Farmers, agronomists, and admins sign in to a role dashboard. Soil readings, irrigation commands, and environmental logs live in Azure Table Storage. Drone images live in a private blob container. Device status lives in Azure SQL.

Public host, once deployed: `https://st10538419.azurewebsites.net`

## Sign in

The sign-in page lists these tester accounts. The password in SQL is a hash.

| Username | Password | Role | Dashboard |
| --- | --- | --- | --- |
| farmer | Farmer-field-1 | Farmer | Field dashboard, telemetry, irrigation, drone upload |
| agronomist | Agronomist-field-1 | Agronomist | Environmental logs, devices, drone gallery |
| admin | Admin-field-1 | Admin | Accounts, devices, hosting status |

## Run locally

```bash
dotnet restore ST10538419_CLDV7112_Practicium.slnx
dotnet test
dotnet run --project src/ST10538419_CLDV7112_Practicium.Web
```

Local data is SQLite at `src/ST10538419_CLDV7112_Practicium.Web/App_Data/local.db`. That file is gitignored. If an older file is still there, delete it so the account and device tables are created again.

Telemetry and image upload stay off until `Functions:BaseUrl`, `Functions:SharedSecret`, and the storage connection strings are set. The dashboards say so in plain text.

## Azure

One resource group, `rg-st10538419-cldv7112`, in `uaenorth`. The plan is Linux **B1** with one worker. Linux App Service has no Free tier, so B1 is the cheapest plan that stays online. SQL stays on Basic. Storage stays on Standard LRS.

```bash
az account set --subscription "Azure for Students"
./scripts/deploy-azure.sh
```

The script keeps the SQL password and the function shared secret in `infra/.deploy.env`. That file is gitignored. Delete the resource group when the submission no longer needs to stay online:

```bash
az group delete --name rg-st10538419-cldv7112 --yes
```

## Load test

`scripts/load-test.sh` sends concurrent requests to `/health` and `/Account/Login` for 12 minutes. Azure autoscale uses a 5 minute window, so a shorter run does not move the instance count.

```bash
chmod +x scripts/load-test.sh
./scripts/load-test.sh https://st10538419.azurewebsites.net
```

### Result

Ran on 6 November 2026 against `https://st10538419.azurewebsites.net` for 721 seconds, 20 workers, hitting `/health` and `/Account/Login`.

- Requests: 20520
- Success: 20518
- Failed: 2 (both were connection timeouts)
- CPU: plan average peaked at 80.5% in the minute of 08:33 UTC, above the 20% scale-out rule
- Instance count during that run: the plan was S1 with 2 workers. It is now B1 with 1 worker so the credit lasts.
- Met the bar: the site stayed up under the load. Two requests timed out.

The autoscale rule was removed after that run. B1 cannot autoscale, and a scale-out rule would put the expensive plan back.
