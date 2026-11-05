---
name: mvc-ui
description: Build or change ASP.NET Core MVC Razor screens using the existing layout, form pattern, and visual system. Use for pages, forms, tables, empty states, navigation, and accessibility.
---

# MVC UI

This is a server-rendered MVC site. Do not introduce React, Tailwind, or a second component library.

Use:

- `Views/Shared/_Layout.cshtml` for the shell
- `wwwroot/css/site.css` for color, type, spacing, and hit targets
- `Views/Shared/_RecordForm.cshtml` as the form-field pattern
- Bootstrap only for the grid, forms, buttons, and the mobile menu

Every page:

- set `ViewData["Title"]`
- one `h1`
- primary action uses `btn btn-accent`
- forms include `asp-validation-summary`, labels, and `_ValidationScriptsPartial`
- lists have an empty state and a search box when the data can grow
- destructive posts use `[ValidateAntiForgeryToken]` and a confirm step

Keep contrast on the paper background. Do not use the default purple Bootstrap theme. Touch targets stay at least 44px. Do not convey state by color alone; the status text already says Ready or Off.

When the brief names the business, change `Site:Name` and `Site:Tagline` in `appsettings.json` and replace foundation wording on Home. Leave the layout structure in place.

UI copy is normal English, even if chat is terse.
