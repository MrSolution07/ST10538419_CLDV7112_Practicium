# Allowed Azure regions

Subscription **Azure for Students** has a policy named **Allowed resource deployment regions**. A deploy outside this list fails with `RequestDisallowedByPolicy`.

Allowed values:

- `uaenorth` (default)
- `switzerlandnorth`
- `centralindia`
- `brazilsouth`
- `austriaeast`

Checked against this subscription on 5 Nov 2026:

- Azure SQL Basic and Standard exist in every region above.
- App Service (`Microsoft.Web/sites`) exists in every region above.
- Storage accounts exist in every region above.

`southafricanorth` is not in the policy, so App Service and SQL cannot be created there on this account.

`infra/main.bicep` uses `@allowed` so a later change cannot pick a blocked region. UAE North is the default because it is the nearest allowed region to South Africa.
