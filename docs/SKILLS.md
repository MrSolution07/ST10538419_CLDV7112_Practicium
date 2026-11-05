# Skills for this submission

Read this file before writing feature code. Cursor also loads each skill from `.cursor/skills/`. When a skill and this file disagree, follow the skill for that task and this file for the order of work.

## Order

1. `read-brief` — understand the question before coding.
2. `solid-code` — extend the existing layers.
3. `mvc-ui` — Razor screens that match the shell already in the repo.
4. `azure-sql` — persist entities on Azure SQL.
5. `azure-storage` — blobs, queues, tables, or files only if the brief asks.
6. `azure-host` — deploy to App Service without wasting the $100 student credit.
7. `caveman` — keep chat short. Keep README, UI copy, commits, and lecturer-facing docs in normal English.

## caveman

Use terse chat while building. Drop filler. Keep technical names exact.

Do not write the website, README, commit messages, or submission documents in that terse style. A marker reads those.

## read-brief

The question lands in `docs/brief/`. Extract only what the brief and rubric require:

- module code, due date, and required evidence
- pages, roles, and entities
- Azure services that are mandatory
- data rules, validation, and file types
- what must be deployed, and what a screenshot must show

Write the extract to `docs/brief/REQUIREMENTS.md` as a table: requirement, marks, file or type that will satisfy it, done or not.

Do not add Azure resources the brief does not ask for. Storage stays off until a requirement needs it.

If the brief conflicts with the sample `ReferenceRecord` module, follow the brief. Keep the layer split.

## solid-code

Layers:

- `FinalCloudDev.Domain` — entities only
- `FinalCloudDev.Application` — inputs, service rules, repository ports
- `FinalCloudDev.Infrastructure` — EF Core, Azure SDK clients
- `FinalCloudDev.Web` — controllers, view models, Razor
- `tests/FinalCloudDev.Tests` — service rules and one persistence test per new aggregate

Controllers bind a form, call a service, and map `ServiceResult` onto `ModelState`. Controllers do not use `DbContext`.

Every new entity copies the reference-record slice: entity, input, repository, service, controller, list/create/edit/details views, tests. Validate in the service even when the form also has data annotations.

Pass `CancellationToken`. Use async EF calls. Do not swallow exceptions. Do not commit secrets.

## mvc-ui

Server-rendered Razor. Use `_Layout.cshtml`, `site.css`, and `_RecordForm.cshtml` as the pattern.

Each screen needs a title, one clear heading, a validation summary on forms, labels tied to inputs, and an empty state on lists. Keep buttons at least 44px tall. Confirm archive or delete. Do not add a second CSS framework. Do not put business rules only in JavaScript.

Rename the visible product in `Site:Name` and `Site:Tagline` when the brief names the business. Do not leave "Foundation" copy on the pages that the marker will open.

## azure-sql

Local development uses SQLite (`Database:Provider=Sqlite`). Production uses Azure SQL (`Database:Provider=SqlServer` and `ConnectionStrings:AzureSql`).

`DatabaseStartup` creates the SQLite file in Development and runs `MigrateAsync` against Azure SQL in Production.

Add a `DbSet`, map the entity in `AppDbContext.OnModelCreating`, then:

```bash
dotnet tool restore
dotnet ef migrations add <Name> --project src/FinalCloudDev.Infrastructure --startup-project src/FinalCloudDev.Infrastructure --output-dir Data/Migrations
```

Never put a real connection string in `appsettings.json`. Use `dotnet user-secrets` locally and App Service settings in Azure. The Bicep template sets those settings.

## azure-storage

Four ports already exist:

- `IBlobStore` for files that need a URL
- `IQueueStore` for work another process will handle
- `ITableStore` for key-value rows that are not the relational model
- `IFileShareStore` for SMB-style files

Empty connection strings register an unconfigured store that throws a clear error. Set the matching `Storage:*ConnectionString` only when the brief requires that service. Deploy storage with `DEPLOY_STORAGE=true` so the credit is not spent on an unused account.

## azure-host

Cheap defaults are already in `infra/main.bicep`: Linux App Service B1, Azure SQL Basic, region `southafricanorth`. Storage is off unless requested.

Deploy from a machine that is already logged in with `az login`:

```bash
SQL_ADMIN_PASSWORD='a-strong-password' ./scripts/deploy-azure.sh
```

The script creates `rg-final-cloud-dev`, deploys Bicep, opens the SQL firewall for the current IP, publishes the site, and prints the HTTPS URL. The app applies EF migrations on startup.

GitHub deploy is `.github/workflows/deploy.yml`. It needs OIDC secrets documented in `docs/azure-hosting.md`. Do not deploy until the brief's features are in the build, so a broken site is not what the marker opens.

## After the build

Run `dotnet test`. Update `docs/brief/REQUIREMENTS.md` so every required row points at a page or Azure resource. Commit in small steps. Push `main`.
