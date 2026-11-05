# Final Cloud Dev

ASP.NET Core MVC foundation for the final cloud submission. The assignment question is not in the repo yet. Drop it in `docs/brief/` and follow `AGENTS.md`.

Author: Christian Bulabula Emungu.

## Stack

- .NET 10 MVC
- EF Core
- SQLite on this machine, Azure SQL in production
- Azure App Service
- Optional Azure Blob, Queue, Table, and File Share

## Run locally

```bash
dotnet restore FinalCloudDev.slnx
dotnet test
dotnet run --project src/FinalCloudDev.Web
```

Open the HTTPS URL printed by the CLI. The home page shows the database provider and a sample record.

## Where an agent should look

| Task | File |
| --- | --- |
| Order of work | `AGENTS.md` |
| All project skills | `docs/SKILLS.md` |
| Cursor skills | `.cursor/skills/` |
| New entity | `docs/extend.md` |
| Deploy | `docs/azure-hosting.md` |

## Deploy

Use the Azure for Students subscription. The template stays on a B1 plan and Basic SQL so the credit lasts.

```bash
az account set --subscription "Azure for Students"
SQL_ADMIN_PASSWORD='choose-a-strong-password' ./scripts/deploy-azure.sh
```

The public URL is printed at the end of the script. Put that URL in this README once the real brief has been deployed.

## Layout

```text
src/FinalCloudDev.Domain            entities
src/FinalCloudDev.Application       services and ports
src/FinalCloudDev.Infrastructure    EF Core and Azure clients
src/FinalCloudDev.Web               MVC site
tests/FinalCloudDev.Tests           service and persistence tests
infra/main.bicep                    App Service and Azure SQL
```
