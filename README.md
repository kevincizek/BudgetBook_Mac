# BudgetBook_Mac

## What is BudgetBook/Was ist BudgetBook?

BudgetBook is a small income and expense tracker I made for my Vocational School (Berufsschule). With BudgetBook you can manage transactions you've made and get statistics from those transactions.

BudgetBook ist ein kleiner Ein- und Ausgabenrechner, den ich für meine Berufsschule erstellt habe. Mit BudgetBook kannst du Buchungen verwalten und Statistiken aus diesen Buchungen einsehen.

## Instructions on how to setup

1. Clone Repository
2. Run in terminal with "dotnet run" (needs .NET SDK) to compile (otherwise Database Migration will complain, this will fail probably because database has not been migrated yet)
3. Migrate all data & seeding data with "dotnet ef database update"
4. Run in terminal again with "dotnet run" (needs .NET SDK) to run

Login Credentials for Demo Account: demo@example.com, Demo123!

Login Credentials for Admin Account: admin@example.com, Admin123!
