This repository has been updated to target .NET 10 (RC) across projects.

Notes:
- EF Core and Npgsql are using release-candidate versions (10.0.0-rc.2) to match the .NET 10 stack. These are pinned in `Directory.Build.props`.
- Microsoft.OpenApi is pinned to 2.0.0 and Microsoft.AspNetCore.OpenApi to 10.0.0.
- If you prefer only stable packages, revert EF Core and Npgsql to 9.x and restore Aspire provider.

Next steps:
- When EF Core / Npgsql GA (10.x) releases are available, update the version properties in `Directory.Build.props` to the GA versions.
- Optionally add policy checks or Dependabot for package upgrades.
