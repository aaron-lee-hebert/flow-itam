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

EF Core migrations are provider-specific. This repository maintains separate migration sets for SQLite development and PostgreSQL production; SQLite does not replace PostgreSQL integration testing.

## Database migrations

Restore the repository-local EF Core tool before creating migrations:

```sh
dotnet tool restore
```

SQLite and PostgreSQL migrations live in separate projects. Create migrations with the matching startup configuration and project:

```sh
dotnet tool run dotnet-ef migrations add MigrationName \
	--project src/FlowItam.Infrastructure.Migrations.Sqlite \
	--startup-project src/FlowItam.Web \
	--output-dir Migrations
```

Apply the local SQLite migration to the Development database with:

```sh
dotnet tool run dotnet-ef database update \
	--project src/FlowItam.Infrastructure.Migrations.Sqlite \
	--startup-project src/FlowItam.Web
```

For PostgreSQL, set `Database:Provider` to `PostgreSql` and configure `ConnectionStrings:PostgreSql` with User Secrets or environment variables before using `src/FlowItam.Infrastructure.Migrations.PostgreSql` as the migrations project. The same `database update` command applies that provider's migration set. Never commit database credentials.
