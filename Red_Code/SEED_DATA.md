# Database Seed Data

This document describes the initial data that is automatically populated in the database when the application starts for the first time.

## 📚 Books (5 entries)

| ID | Title | Author |
|----|-------|--------|
| 1 | Clean Code: A Handbook of Agile Software Craftsmanship | Robert C. Martin |
| 2 | The Pragmatic Programmer: Your Journey to Mastery | Andrew Hunt and David Thomas |
| 3 | Design Patterns: Elements of Reusable Object-Oriented Software | Erich Gamma, Richard Helm, Ralph Johnson, John Vlissides |
| 4 | Refactoring: Improving the Design of Existing Code | Martin Fowler |
| 5 | Domain-Driven Design: Tackling Complexity in the Heart of Software | Eric Evans |

## 💬 Quotations (5 entries)

| ID | Quotation | Author |
|----|-----------|--------|
| 1 | Any fool can write code that a computer can understand. Good programmers write code that humans can understand. | Martin Fowler |
| 2 | First, solve the problem. Then, write the code. | John Johnson |
| 3 | Code is like humor. When you have to explain it, it's bad. | Cory House |
| 4 | Clean code always looks like it was written by someone who cares. | Robert C. Martin |
| 5 | The best error message is the one that never shows up. | Thomas Fuchs |

## How Seeding Works

The seeding process is implemented in `Infrastructure/Data/DbInitializer.cs` and is called automatically when the application starts.

### Seeding Logic:

1. **Database Migration**: Ensures all migrations are applied
2. **Duplicate Check**: Verifies if data already exists (checks both Books and Quotations)
3. **Data Insertion**: If no data exists, adds 5 books and 5 quotations
4. **Error Handling**: Logs any errors during the seeding process

### Code Location:

- **Seeder Class**: `Red_Code/Infrastructure/Data/DbInitializer.cs`
- **Startup Call**: `Red_Code/Program.cs` (lines 41-55)

### Behavior:

- ✅ Runs automatically on application startup
- ✅ Idempotent (safe to run multiple times)
- ✅ Only seeds if database is empty
- ✅ Applies pending migrations first
- ✅ Logs errors without crashing the application

## Testing the Seed Data

After starting the application for the first time, you can verify the seed data:

### Get All Books
```bash
GET https://localhost:7104/api/books
```

### Get All Quotations
```bash
GET https://localhost:7104/api/quotations
```

Both endpoints should return 5 entries each on the first run.

## Resetting Seed Data

To re-seed the database:

1. Drop the database:
```bash
dotnet ef database drop --project Red_Code/Red_Code.csproj
```

2. Restart the application (database will be recreated and seeded automatically)

Or manually delete data and restart:
```sql
DELETE FROM Quotations;
DELETE FROM Books;
DBCC CHECKIDENT ('Books', RESEED, 0);
DBCC CHECKIDENT ('Quotations', RESEED, 0);
```
