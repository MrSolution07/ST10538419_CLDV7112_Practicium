# Azure hosting

Subscription: **Azure for Students**. Account used for this foundation: `ST10538419@rcconnect.edu.za`. Role on that subscription: Owner. Credit remaining when this foundation was prepared: about **$100 USD**, expiring **26 Feb 2027**.

Region: `uaenorth`. The other allowed regions are `switzerlandnorth`, `centralindia`, `brazilsouth`, and `austriaeast`. See `docs/azure-regions.md`. `southafricanorth` is blocked by policy.

## What gets created

| Resource | SKU | Why |
| --- | --- | --- |
| App Service plan | Linux B1 | Cheapest Linux plan. Free and Shared tiers are Windows-only, so this site cannot sit on F1. |
| Web app | .NET 10, name `st10538419` | The MVC site. Host `st10538419.azurewebsites.net`. HTTPS only. |
| Azure SQL | Basic | Users and the device registry. |
| Storage account | Standard LRS | Blobs, queues, tables, and the existing file share port. |
| Function App | .NET isolated, name `st10538419-functions`, same B1 plan | Four functions. Consumption Linux would not start in this region, so it shares the site plan. |
| Log Analytics + Application Insights | PerGB2018, component name `st10538419-insights` | Web and Functions logs land here, not only on the local console. |

The web app receives:

- `ASPNETCORE_ENVIRONMENT=Production`
- `Database__Provider=SqlServer`
- `ConnectionStrings__AzureSql`
- `Storage__BlobConnectionString`, `Storage__QueueConnectionString`, `Storage__TableConnectionString`, and `Storage__FileShareConnectionString`
- `APPLICATIONINSIGHTS_CONNECTION_STRING`

The Function App receives `AzureWebJobsStorage`, the SQL connection, the storage connection strings, and the same Application Insights connection string. The web app and the Function App share `Functions__SharedSecret`. Startup creates container `drone-images`, queue `irrigation-commands`, tables `sensorreadings`, `irrigationcommands`, `environmentallogs`, and `imageaudits`, and file share `practicum`. The app applies EF migrations when it starts. Production refuses to boot if SQL, storage, or Application Insights is missing.

## Deploy from the terminal

```bash
az login
az account set --subscription "Azure for Students"
SQL_ADMIN_PASSWORD='replace-with-a-strong-password' ./scripts/deploy-azure.sh
```

If `SQL_ADMIN_PASSWORD` is unset, the script writes a generated password to `infra/.deploy.env` and reuses it on the next run. That file is gitignored.

The script creates `rg-st10538419-cldv7112`, deploys `infra/main.bicep`, allows the current public IP through the SQL firewall, publishes the site and the Function App, and prints the URLs. Open `/health` after the first start. The first request can take a minute while the migration runs. Delete that group when it is no longer the submission, so a stopped or duplicate stack does not keep spending the credit.

Do not commit `SQL_ADMIN_PASSWORD`.

## Local secret, when you need the real database

```bash
dotnet user-secrets --project src/ST10538419_CLDV7112_Practicium.Web set "ConnectionStrings:AzureSql" "Server=tcp:<server>.database.windows.net,1433;Initial Catalog=ST10538419_CLDV7112_Practicium;User ID=st10538419;Password=<password>;Encrypt=True;TrustServerCertificate=False;"
dotnet user-secrets --project src/ST10538419_CLDV7112_Practicium.Web set "Database:Provider" "SqlServer"
```

## GitHub Action

`.github/workflows/deploy.yml` runs only when someone starts it. It needs these repository secrets:

- `AZURE_CLIENT_ID`
- `AZURE_TENANT_ID`
- `AZURE_SUBSCRIPTION_ID`
- `SQL_ADMIN_PASSWORD`

Create an app registration, grant it Contributor on the subscription or resource group, and add a federated credential for `repo:MrSolution07/ST10538419_CLDV7112_Practicium:ref:refs/heads/main`. The shell script is enough if that setup is not ready.

## Evidence for the marker

Keep the public HTTPS URL, a screenshot of the home page showing `SqlServer`, and a screenshot of the resource group. Do not screenshot the connection string or the SQL password.

## Tear down

```bash
az group delete --name rg-st10538419-cldv7112 --yes
```

Deleting the resource group stops the B1 and SQL charges.
