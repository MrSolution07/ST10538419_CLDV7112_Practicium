---
name: azure-sql
description: Persist assignment data with EF Core on SQLite locally and Azure SQL in production. Use when adding entities, migrations, connection strings, or database startup behavior.
---

# Azure SQL

`AppDbContext` is the only EF context. Local provider is SQLite so the site runs before Azure exists. Production provider is SQL Server. `AddInfrastructure` throws if Production is selected without `ConnectionStrings:AzureSql`.

SQL Server retries failed connections. Migrations live in `src/FinalCloudDev.Infrastructure/Data/Migrations` and apply on startup when the provider is SQL Server. SQLite uses `EnsureCreated` plus a single seed row.

To add a table:

1. Add the `DbSet` and `OnModelCreating` limits that match the data annotations.
2. Restore the local tool: `dotnet tool restore`.
3. Add a migration:
   `dotnet ef migrations add <Name> --project src/FinalCloudDev.Infrastructure --startup-project src/FinalCloudDev.Infrastructure --output-dir Data/Migrations`
4. Add a SQLite test that opens `Data Source=:memory:` and calls `EnsureCreated`.

Secrets:

- local: `dotnet user-secrets --project src/FinalCloudDev.Web set "ConnectionStrings:AzureSql" "<value>"`
- Azure: the Bicep template writes `ConnectionStrings__AzureSql`

Do not commit either value. Do not point Development at the student Azure SQL database unless you are testing a migration on purpose.
