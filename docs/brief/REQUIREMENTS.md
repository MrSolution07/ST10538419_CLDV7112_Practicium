# Requirements

One row per item in `question.md`. Marks are blank because the question does not print a mark scheme. Status is what this repository can prove today.

| Requirement | Marks | Where it lives | Status |
| --- | --- | --- | --- |
| a. App Service host `st10538419.azurewebsites.net` |  | `infra/main.bicep` web app name `st10538419`. Screenshot: App Service overview. | Template ready, deploy not run |
| b. Azure SQL users with username, password hash, and role Farmer, Agronomist, or Admin. MVC shows `SqlServer` when connected. |  | `UserAccounts` table, login page, hosting card on each dashboard. | Built locally. Production connection shows after deploy. |
| c. Blob container `drone-images` for drone images. Tables `sensorreadings`, `irrigationcommands`, `environmentallogs`. |  | Upload on the Farmer and Agronomist pages. `CloudBootstrap` creates the container and tables. | Built. Containers are created on the first production start. |
| d. Exactly four Functions: blob audit to `imageaudits`, HTTP GET telemetry, HTTP POST irrigation command into the table and the queue, queue trigger updates `Devices` in SQL. |  | `src/ST10538419_CLDV7112_Practicium.Functions`. Screenshot: Function App function list. | Built. Health function removed. |
| e. Concurrent load against the App Service for 12 minutes. |  | `scripts/load-test.sh`. 20520 requests, 2 timeouts. | Run on 6 Nov 2026 |
| f. Application Insights during that load, plus a short written result. |  | App Insights is connected (`Logs: Ready` on the operations page). CPU from Azure Monitor is in the README. | Written. Portal chart screenshot still to capture. |
| g. CPU autoscale on the App Service plan, minimum 1, maximum 2, scale out above 20%. |  | Load ran on S1. CPU peak 80.5%. Rule removed afterwards. Plan is now B1 with 1 worker so the site can stay online. | Load evidence recorded. Live rule removed to save credit. |
