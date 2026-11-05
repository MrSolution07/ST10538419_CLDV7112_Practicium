---
name: read-brief
description: Read the assignment question, rubric, and any attached documents, then turn them into a requirement map before coding. Use when a brief, PDF, Word file, rubric, or question is added.
---

# Read the brief

1. Read every file in `docs/brief/` and any document the user attaches. PDFs and Word files included.
2. Extract only stated requirements. Quote the constraint when it is easy to misread (due date, group size, required Azure service, banned library).
3. Write `docs/brief/REQUIREMENTS.md` with columns: requirement, marks if given, where it will live in this repo, status.
4. List required Azure services. Leave every other Azure service off.
5. List entities, pages, roles, uploads, and background work.
6. If the brief is ambiguous, ask one concrete question. Do not invent a feature to fill the gap.
7. Then follow `solid-code`, `mvc-ui`, and the Azure skill that matches each required service.

The sample `ReferenceRecord` module is a pattern, not a product. Replace its visible copy when the brief names the real business. Delete the sample only after the real module has tests and the solution still builds.
