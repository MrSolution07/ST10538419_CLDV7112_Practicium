# Submission checklist

Fill this in after the brief is read. Do not tick a row that the running site does not prove.

- [ ] `docs/brief/REQUIREMENTS.md` maps every marked requirement
- [ ] Each entity follows the Domain / Application / Infrastructure / Web split
- [ ] `dotnet test` passes
- [ ] Pages use the existing layout, labels, validation, and empty states
- [ ] Production uses Azure SQL, not the local SQLite file
- [ ] Storage is enabled only if the brief asks for blobs, queues, tables, or files
- [ ] `./scripts/deploy-azure.sh` has been run against Azure for Students
- [ ] `/health` returns healthy on the public site
- [ ] The README contains the public URL and how to sign in, if the brief adds accounts
- [ ] No connection string, password, or `.db` file is in git
