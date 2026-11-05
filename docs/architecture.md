# Architecture

The web app is ASP.NET Core MVC on .NET 10. Data access is EF Core. The production database is Azure SQL. The production host is Linux App Service.

```text
Browser
  -> ST10538419_CLDV7112_Practicium.Web (controllers, Razor)
    -> ST10538419_CLDV7112_Practicium.Application (services)
      -> ST10538419_CLDV7112_Practicium.Infrastructure (EF Core, Azure Storage)
        -> SQLite locally, or Azure SQL in production
```

Domain has no dependency on EF or ASP.NET. Application defines the repository ports. Infrastructure implements them. The web project references both Application and Infrastructure so composition stays in `Program.cs`.

`ReferenceRecord` is a complete vertical slice:

- entity and timestamps
- service validation
- EF mapping and repository
- list, create, edit, details
- archive instead of a hard delete
- unit tests plus a SQLite persistence test

New brief entities should follow that slice. A requirement that is only a file upload should still store the SQL row in this shape and call `IBlobStore` from the service, not from the controller.

Startup:

- Development and an empty Azure SQL connection string use SQLite and `EnsureCreated`.
- `Database:Provider=SqlServer` uses `MigrateAsync`.
- Production refuses to boot unless the provider is SQL Server and the connection string is set.

Health: `GET /health` checks the database.
