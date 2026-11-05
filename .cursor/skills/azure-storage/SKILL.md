---
name: azure-storage
description: Use Azure Blob, Queue, Table, or File Share through the existing store interfaces. Use when the brief requires file upload, background messages, table storage, or Azure Files.
---

# Azure storage

Inject the interface. Do not construct SDK clients in a controller.

| Need | Interface | Setting |
| --- | --- | --- |
| Upload a file and keep a URL | `IBlobStore` | `Storage:BlobConnectionString` |
| Hand work to another process | `IQueueStore` | `Storage:QueueConnectionString` |
| Store a key-addressed row outside SQL | `ITableStore` | `Storage:TableConnectionString` |
| Store a file on a share | `IFileShareStore` | `Storage:FileShareConnectionString` |

An empty setting registers `Unconfigured*Store`. Calling it throws an error that names the missing setting. That is intentional.

`scripts/deploy-azure.sh` always creates one Storage account and writes its connection string into all four settings. Relational data still goes through EF Core and Azure SQL. On startup, `CloudBootstrap` creates container `uploads`, queue `practicum-work`, table `practicum`, and file share `practicum`. The Function App trigger listens on `practicum-work`.

Blob containers are private. Do not switch them to public access. Validate file size and extension in the Application service before calling `UploadAsync`. Store the returned URL or blob name on the SQL entity.

Add a test that the matching store is the Azure implementation when the connection string is present, and the unconfigured implementation when it is empty. Follow `StorageRegistrationTests`.
