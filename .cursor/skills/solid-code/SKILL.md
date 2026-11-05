---
name: solid-code
description: Add features by copying the reference-record slice across Domain, Application, Infrastructure, Web, and tests. Use when implementing assignment behavior, entities, validation, or bug fixes.
---

# Solid code

Keep the four projects. Do not put EF Core or Azure SDK calls in controllers.

For each new entity:

1. Domain type under `ST10538419_CLDV7112_Practicium.Domain/Entities`, inheriting `Entity` when it is a row with an id and timestamps.
2. Input record and `I*Repository` / `I*Service` in Application.
3. Service method returns `ServiceResult<T>` for creates and updates. Validate there.
4. EF configuration in `AppDbContext` and a repository in Infrastructure.
5. Form model plus list, create, edit, and details views.
6. Tests for the failure rules and one SQLite round trip.

Controllers: validate the form, call the service, copy `result.Error` into `ModelState`, redirect on success.

Pass `CancellationToken` through. Use `async` EF methods. Update `UpdatedAt` on every change.

Do not catch an exception and return a success result. Do not commit connection strings, publish profiles, or `.db` files.

Match the names already used: `ReferenceRecordService`, `ReferenceRecordRepository`, `ReferenceRecordsController`.
