# Extend the foundation

Use this when the brief names a new thing to store, for example `Product`.

1. Add `Product` in `src/FinalCloudDev.Domain/Entities`. Inherit `Entity` if it needs an id and timestamps.
2. Add `ProductInput`, `IProductRepository`, `IProductService`, and `ProductService` under Application. Return `ServiceResult<Product>` from create and update. Validate in the service.
3. Register the service in `FinalCloudDev.Application/DependencyInjection.cs`.
4. Map `Product` in `AppDbContext`. Add `ProductRepository` and register it in the Infrastructure `DependencyInjection`.
5. Add a migration:

```bash
dotnet tool restore
dotnet ef migrations add AddProduct --project src/FinalCloudDev.Infrastructure --startup-project src/FinalCloudDev.Infrastructure --output-dir Data/Migrations
```

6. Add `ProductsController`, a form model, and list, create, edit, and details views. Copy `ReferenceRecords` and `_RecordForm.cshtml`.
7. Add a nav link in `_Layout.cshtml`.
8. Add service tests and one SQLite persistence test.
9. Run `dotnet test`.
10. Change `Site:Name` when the brief names the business. Replace the home-page foundation text with the real introduction.

File upload belongs in the service: validate the file, call `IBlobStore`, save the URL on the entity. Turn storage on only then, with `DEPLOY_STORAGE=true`.

Delete `ReferenceRecord` after the real module is in place and nothing still links to it.
