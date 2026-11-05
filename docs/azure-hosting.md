# Azure hosting

Subscription: **Azure for Students**. Account used for this foundation: `ST10538419@rcconnect.edu.za`. Role on that subscription: Owner. Credit remaining when this foundation was prepared: about **$100 USD**, expiring **26 Feb 2027**.

Region: `uaenorth`. The other allowed regions are `switzerlandnorth`, `centralindia`, `brazilsouth`, and `austriaeast`. See `docs/azure-regions.md`. `southafricanorth` is blocked by policy.

## What gets created

| Resource | SKU | Why |
| --- | --- | --- |
| App Service plan | Linux B1 | Runs .NET 10 and stays on. Cheaper than a production plan. |
| Web app | .NET 10 | The MVC site. HTTPS only. |
| Azure SQL | Basic | Relational database for EF Core. |
| Storage account | Standard LRS | Created only when `DEPLOY_STORAGE=true`. |

The web app receives:

- `ASPNETCORE_ENVIRONMENT=Production`
- `Database__Provider=SqlServer`
- `ConnectionStrings__AzureSql`
- empty storage settings, or one storage connection string when storage is enabled

The app applies EF migrations when it starts.

## Deploy from the terminal

```bash
az login
az account set --subscription "Azure for Students"
SQL_ADMIN_PASSWORD='replace-with-a-strong-password' ./scripts/deploy-azure.sh
```

Optional storage:

```bash
DEPLOY_STORAGE=true SQL_ADMIN_PASSWORD='replace-with-a-strong-password' ./scripts/deploy-azure.sh
```

The script creates `rg-final-cloud-dev`, deploys `infra/main.bicep`, allows the current public IP through the SQL firewall, publishes the site, and prints the URL. Open `/health` after the first start. The first request can take a minute while the migration runs.

Do not commit `SQL_ADMIN_PASSWORD`.

## Local secret, when you need the real database

```bash
dotnet user-secrets --project src/FinalCloudDev.Web set "ConnectionStrings:AzureSql" "Server=tcp:<server>.database.windows.net,1433;Initial Catalog=FinalCloudDev;User ID=fcdadmin;Password=<password>;Encrypt=True;TrustServerCertificate=False;"
dotnet user-secrets --project src/FinalCloudDev.Web set "Database:Provider" "SqlServer"
```

## GitHub Action

`.github/workflows/deploy.yml` runs only when someone starts it. It needs these repository secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `SQL_ADMIN_PASSWORD`

Create an app registration, grant it Contributor on the subscription or resource group, and add a federated credential for `repo:MrSolution07/Final_Cloud_Dev:ref:refs/heads/main`. The shell script is enough if that setup is not ready.

## Evidence for the marker

Keep the public HTTPS URL, a screenshot of the home page showing `SqlServer`, and a screenshot of the resource group. Do not screenshot the connection string or the SQL password.

## Tear down

```bash
az group delete --name rg-final-cloud-dev --yes
```

Deleting the resource group stops the B1 and SQL charges.
