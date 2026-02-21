# PostgreSQL Migration Guide

## What Changed

This project has been migrated from SQL Server to PostgreSQL. The following changes were made:

### 1. Package Changes
- **Removed**: `Microsoft.EntityFrameworkCore.SqlServer` (10.0.3)
- **Added**: `Npgsql.EntityFrameworkCore.PostgreSQL` (10.0.0)

### 2. Code Changes
- **Program.cs**: Changed from `UseSqlServer()` to `UseNpgsql()`
- **All Migrations**: Removed old SQL Server migrations and created new PostgreSQL migrations

### 3. Configuration Changes
- **appsettings.json**: Updated connection string to PostgreSQL format
- **.env.development**: Updated database configuration variables
- **.env.example**: Updated with PostgreSQL connection parameters

## Connection String Format

The new PostgreSQL connection string format:
```
Host=localhost;Port=5432;Database=RedCodeDb;Username=postgres;Password=postgres
```

## Prerequisites

Before running the application, ensure you have:

1. **PostgreSQL installed** (version 12 or higher recommended)
2. **PostgreSQL server running** on localhost:5432
3. **Valid PostgreSQL credentials** (update in appsettings.json or environment variables)

## Database Setup

The application is configured to automatically create the database on first execution:

1. The `DbInitializer.SeedAsync()` method in Program.cs calls `context.Database.MigrateAsync()`
2. This will:
   - Create the database if it doesn't exist
   - Apply all pending migrations
   - Seed initial data (books and quotations)

## Running the Application

1. **Update the connection string** in `appsettings.json` with your PostgreSQL credentials
2. **Run the application**:
   ```bash
   dotnet run --project Red_Code/Red_Code.csproj
   ```
3. The database will be created automatically on first startup

## Verifying the Migration

To verify the database was created successfully:

1. Connect to PostgreSQL using pgAdmin or psql
2. Check for the `RedCodeDb` database
3. Verify the following tables exist:
   - AspNetUsers
   - AspNetRoles
   - AspNetUserRoles
   - AspNetUserClaims
   - AspNetUserLogins
   - AspNetUserTokens
   - AspNetRoleClaims
   - Books
   - Quotations

## Manual Migration (if needed)

If you need to apply migrations manually:

```bash
# Apply migrations
dotnet ef database update --project Red_Code/Red_Code.csproj

# Create a new migration (if you modify entities)
dotnet ef migrations add MigrationName --project Red_Code/Red_Code.csproj

# Remove last migration
dotnet ef migrations remove --project Red_Code/Red_Code.csproj
```

## Compatibility

This migration maintains full compatibility with:
- ✅ ASP.NET Core Identity (10.0.3)
- ✅ Entity Framework Core (10.0.x)
- ✅ JWT Authentication
- ✅ All existing controllers and services
- ✅ .NET 10.0

## Troubleshooting

### Connection Issues
If you get connection errors:
1. Verify PostgreSQL is running: `pg_isready`
2. Check credentials in appsettings.json
3. Ensure PostgreSQL is listening on port 5432

### Migration Issues
If migrations fail:
1. Check PostgreSQL user has CREATE DATABASE privileges
2. Verify the database doesn't already exist with conflicting schema
3. Try dropping the database and rerunning: `DROP DATABASE IF EXISTS RedCodeDb;`

### Authentication Issues
The user authentication system remains unchanged. All existing Identity features work the same way with PostgreSQL.
