# flow-itam
A lightweight, multi-tenant IT asset management and security governance platform for small businesses. Track equipment, ownership, assignments, security reviews, and risks in one place.

## Development database

The application connects to a separately hosted PostgreSQL server. Configure its connection string with .NET User Secrets for local development; do not put credentials in `appsettings.json` or commit them:

```sh
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Host=your-db-hostname;Port=5432;Database=flowitam_dev;Username=your-user;Password=your-password;SSL Mode=VerifyFull" --project src/FlowItam.Web
```

The application requires a configured connection string and enforces TLS certificate and hostname verification. If the database uses a private certificate authority, configure its CA certificate using Npgsql's `Root Certificate` connection-string option. Restrict development database access to authorized developer machines and production access to the application host or private network; do not expose PostgreSQL broadly to the internet.

In production, provide `ConnectionStrings__DefaultConnection` through the hosting environment or a secrets manager, using a distinct production database and credentials. Do not use .NET User Secrets in production. The `/health` endpoint checks database connectivity.
