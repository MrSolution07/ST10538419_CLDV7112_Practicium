# ST10538419 CLDV7112 Practicium

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
dotnet restore ST10538419_CLDV7112_Practicium.slnx
dotnet test
dotnet run --project src/ST10538419_CLDV7112_Practicium.Web
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
src/ST10538419_CLDV7112_Practicium.Domain            entities
src/ST10538419_CLDV7112_Practicium.Application       services and ports
src/ST10538419_CLDV7112_Practicium.Infrastructure    EF Core and Azure clients
src/ST10538419_CLDV7112_Practicium.Web               MVC site
tests/ST10538419_CLDV7112_Practicium.Tests           service and persistence tests
infra/main.bicep                    App Service and Azure SQL
```
