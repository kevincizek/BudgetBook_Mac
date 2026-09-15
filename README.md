# BudgetBook_Mac

1. Clone Repository
2. Run in terminal with "dotnet run" (needs .NET SDK) to compile (otherwise Database Migration will complain, this will fail probably because database has not been migrated yet)
3. Migrate all data & seeding data with "dotnet ef database update"
4. Run in terminal again with "dotnet run" (needs .NET SDK) to run
4. Log in with Demo Account (migrated via Data Seeding Database Update): demo@example.com, Demo123!
