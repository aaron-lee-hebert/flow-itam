# flow-itam
A lightweight, multi-tenant IT asset management and security governance platform for small businesses. Track equipment, ownership, assignments, security reviews, and risks in one place.

## Development database

Development defaults to a local SQLite database at `src/FlowItam.Web/flowitam.dev.db`; that file is ignored by Git. Production defaults to PostgreSQL. SQLite is only allowed in the Development environment.

To use a separately hosted PostgreSQL server during development, override the provider and store the connection string in .NET User Secrets. Do not put credentials in `appsettings.json` or commit them:

```sh
dotnet user-secrets set "Database:Provider" "PostgreSql" --project src/FlowItam.Web
dotnet user-secrets set "ConnectionStrings:PostgreSql" "Host=your-db-hostname;Port=5432;Database=flowitam_dev;Username=your-user;Password=your-password;SSL Mode=VerifyFull" --project src/FlowItam.Web
```

The application requires a configured connection string and enforces TLS certificate and hostname verification. If the database uses a private certificate authority, configure its CA certificate using Npgsql's `Root Certificate` connection-string option. Restrict development database access to authorized developer machines and production access to the application host or private network; do not expose PostgreSQL broadly to the internet.

In production, provide `ConnectionStrings__PostgreSql` through the hosting environment or a secrets manager, using a distinct production database and credentials. Do not use .NET User Secrets in production. The `/health` endpoint checks database connectivity.

EF Core migrations are provider-specific. When schema migrations are introduced, maintain and test migrations for both SQLite development and PostgreSQL production; SQLite does not replace PostgreSQL integration testing.
