# Agent guide

This repository is the foundation for a final cloud submission. The business rules are not here yet. They arrive as documents in `docs/brief/`.

## Before writing feature code

1. Read `docs/SKILLS.md`.
2. Read every file in `docs/brief/`. If the folder only contains the placeholder, stop and ask for the question.
3. Write `docs/brief/REQUIREMENTS.md` with one row per marked requirement.
4. Implement only those rows. Use the skills for code structure, Razor UI, Azure SQL, storage, and hosting.

## Structure

- `src/FinalCloudDev.Domain` holds entities.
- `src/FinalCloudDev.Application` holds validation and use cases.
- `src/FinalCloudDev.Infrastructure` holds EF Core and Azure clients.
- `src/FinalCloudDev.Web` holds controllers and Razor views.
- `tests/FinalCloudDev.Tests` holds the automated checks.
- `infra/main.bicep` creates App Service and Azure SQL.
- `scripts/deploy-azure.sh` publishes the site.

`ReferenceRecord` is the copy pattern: entity, service, repository, controller, four views, tests. Copy it for each real entity. Remove the sample only after a real module replaces it and `dotnet test` passes.

## Local run

```bash
dotnet restore FinalCloudDev.slnx
dotnet test
dotnet run --project src/FinalCloudDev.Web
```

Development uses SQLite at `src/FinalCloudDev.Web/App_Data/local.db`. That file is gitignored.

## Azure

Deploy only after the brief's features build and the tests pass. The student subscription has about $100 of credit. The template uses a B1 plan and a Basic SQL database. Storage stays off until the brief requires it.

```bash
az account set --subscription "Azure for Students"
SQL_ADMIN_PASSWORD='choose-a-strong-password' ./scripts/deploy-azure.sh
```

Details are in `docs/azure-hosting.md`.

## Voice

Chat can be short. Pages, the README, commit messages, and documents for the lecturer stay in normal English.

## Do not

- Commit secrets or local databases.
- Call Azure SDK clients from controllers.
- Add a second UI framework.
- Provision extra Azure resources that the rubric does not require.
