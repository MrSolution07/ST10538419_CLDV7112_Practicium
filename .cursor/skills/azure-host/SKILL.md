---
name: azure-host
description: Deploy the MVC app to Azure App Service with Azure SQL using the Bicep template and deploy script. Use when publishing, configuring App Service, or preparing submission evidence.
---

# Azure host

Template: `infra/main.bicep`. Script: `scripts/deploy-azure.sh`. Region default: `uaenorth`. Allowed: `uaenorth`, `switzerlandnorth`, `centralindia`, `brazilsouth`, `austriaeast`. See `docs/azure-regions.md`.

Resources:

- Linux App Service plan B1
- Web app, .NET 10, HTTPS only, always on
- Azure SQL server and Basic database
- Firewall rule that allows Azure services
- Storage account, one Function App on the same B1 plan, Log Analytics, and Application Insights

The script also adds a firewall rule for the current public IP, then publishes the web app and the Function App. The site runs migrations at startup. Linux Consumption did not start in `uaenorth`, so the worker shares the B1 plan.

Before the first deploy:

1. `az login` as the student account.
2. `az account set --subscription "Azure for Students"`.
3. Choose a strong `SQL_ADMIN_PASSWORD` and do not commit it.
4. Run `SQL_ADMIN_PASSWORD='...' ./scripts/deploy-azure.sh`.

After deploy, open `/health` and the home page. Confirm `Database` shows `SqlServer`.

GitHub Action `.github/workflows/deploy.yml` is manual. It needs repository secrets `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, and `SQL_ADMIN_PASSWORD`, plus a federated credential for this repo. The shell script is enough for the submission if the Action is not set up.

Do not scale the plan, add a second region, or enable storage "just in case". The student credit is $100 and expires 26 Feb 2027.
