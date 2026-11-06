# Theory

## 1. Disaster recovery and storage redundancy

Redundancy is set on the storage account. Every blob, table, queue and file in that account gets the same option. Copies are made by Azure. The application does not copy them.

**Locally redundant storage (LRS)** keeps three synchronous copies in one data centre in the primary region. Durability is eleven nines (99.999999999% over a year). It survives a disk or server failure. It does not survive loss of that data centre or region. It is the cheapest option and has no cross-region lag [1] (Microsoft referemce here).

**Geo-redundant storage (GRS)** starts as LRS in the primary region, then copies the data asynchronously to a paired region hundreds of kilometres away, where it is stored as LRS again. Durability is sixteen nines (99.99999999999999% over a year). A regional outage can be failed over to the secondary. The secondary is not readable until failover. Recent writes can be missing because replication is asynchronous. GRS costs more because the data is stored twice and egress is charged for the copy [1]  (Microsoft referemce here)..

**Historical crop harvest records** should use **GRS**. A season cannot be measured again. The write rate is low, so the extra cost is small next to the cost of losing the archive.

**Temporary sensor readings** should use **LRS**. Soil sensors send a new reading continuously. Losing one data centre drops a short window, and the next reading replaces it. LRS avoids geo-replication cost and lag on that volume.

One account cannot be both. Harvest archives and live telemetry need two accounts if both options are required. This project’s account is `Standard_LRS` in `infra/main.bicep`, which matches the high-volume sensor and drone workload on a student subscription.

## 2. Stateless App Service and Functions

Horizontal scale adds instances behind a load balancer. The next request, or the next function invocation, can land on any instance. An instance can also be removed while the app is under load. Anything that exists only in that instance’s memory or disk is gone for every other instance [2] [3]  (Microsoft referemce here)..

That is why App Service and Functions stay stateless. Shared state belongs in Azure SQL, Blob Storage, Table Storage, or Queue Storage.

**Session data on the web server.** In-process session lives in one worker’s memory. The login request can set the session on instance A. The next click can hit instance B, which has no session, so the user looks logged out or loses the field they just selected. Sticky sessions pin the user to one instance. Extra instances then do not share that user’s load, and a restart still drops the session. AgriStream does not keep the farmer’s dashboard in server memory. Telemetry is read from the `sensorreadings` table on each request.

**Files on the web server.** A drone image saved on one instance’s disk is invisible to the other instances, to the Function App, and to a new instance after scale-in. The local disk is also wiped when the worker is replaced. AgriStream uploads images to the `drone-images` blob container. The blob trigger then sees the file no matter which web instance accepted the upload.

The same rule applies to Functions. `PostIrrigationFunction` and `ProcessIrrigationFunction` do not share memory. The HTTP function writes the command to the `irrigationcommands` table and the `irrigation-commands` queue. The queue function runs later, possibly on another worker, and updates the device row in Azure SQL. A static field or a local file would not survive that hand-off.

## 3. Triggers and input/output bindings

A trigger starts the function. An input binding reads extra data at the start. An output binding writes a result at the end. All three are declared on the function. The function does not poll [4].

This pipeline declares triggers only. Reads and writes go through `IFarmTables`, `IWorkQueue`, and `IDeviceService`. That is the same job as input and output bindings, done in application code instead of binding attributes.

| | Definition | Purpose | AgriStream |
| --- | --- | --- | --- |
| Trigger | The event that invokes the function and supplies the event payload. | Run work when something happens, without a polling loop. | `DroneImageAuditFunction`: blob trigger on `drone-images/{name}`. `GetTelemetryFunction`: HTTP GET `telemetry`. `PostIrrigationFunction`: HTTP POST `irrigation`. `ProcessIrrigationFunction`: queue trigger on `irrigation-commands`. |
| Input binding | A parameter that reads from another Azure service when the invocation starts. | Bring in data the trigger itself does not carry. | No input-binding attribute is declared. The read that fills this role is `GetTelemetryFunction`, which calls `ITelemetryQuery` and returns the latest rows from `sensorreadings`. |
| Output binding | A parameter or return value that writes to another Azure service when the invocation ends. | Publish a result without storage client code in the function. | No output-binding attribute is declared. `ImageAuditProcessor` upserts `imageaudits` with status Received. `IrrigationIntake` upserts `irrigationcommands` as Pending and sends the queue message. `IrrigationProcessor` sets the device to Irrigating or Needs Attention in Azure SQL and marks the command Processed. |

## References

1. Microsoft Learn, “Data redundancy”, Azure Storage. <https://learn.microsoft.com/en-us/azure/storage/common/storage-redundancy>
2. Microsoft Learn, “Scale up an app in Azure App Service”. <https://learn.microsoft.com/en-us/azure/app-service/manage-scale-up>
3. Microsoft Learn, “Azure Functions scale and hosting”. <https://learn.microsoft.com/en-us/azure/azure-functions/functions-scale>
4. Microsoft Learn, “Azure Functions triggers and bindings concepts”. <https://learn.microsoft.com/en-us/azure/azure-functions/functions-triggers-bindings>
