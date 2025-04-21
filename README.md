# SurveyApp

SurveyApp is a secure, high-performance API for managing user-created surveys with options and votes. Designed with a clean layered architecture, robust validation, and token-based authentication. Ideal for demonstrating enterprise-level backend design with modern .NET practices.

## Overview

The goal was to design a modular, extensible backend using:

- ASP.NET 8 Web API
- iBATIS (SQLBatis.NET)
- MSSQL
- JWT Authentication with Swagger support
- FluentValidation
- Swagger + xUnit
- Global Exception Handling
- Health check endpoint (\`/healthz\`)

Users can:

- Register/Login via JWT (returns token on registration)
- Create surveys with 2-5 options
- Vote only once per survey
- View public results with percentages
- List/delete their own surveys (owner-only deletion)

## Project Structure

```
SurveyApp/
├── SurveyApp.API/
│   ├── Configuration/
│   ├── Controllers/
│   ├── DAOs/
│   ├── DTOs/
│   ├── LocalPackages/
│   ├── Middleware/
│   ├── Models/
│   ├── Properties/
│   ├── Services/
│   ├── SqlMaps/
│   ├── Validators/
│   ├── appsettings.json
│   ├── Program.cs
│   └── SurveyApp.API.csproj
├── SurveyApp.API.Tests/
├── db.sql          # MSSQL-compatible schema and seed script
├── surveydb.bak    # MSSQL DB backup with seeded data
├── SurveyApp.sln
└── .gitignore
```
## Testing

Tested with xUnit using a seeded MSSQL instance. Includes unit and integration tests covering registration, login, auth edge cases, survey creation, voting rules, vote submission, and result accuracy (including percentages). Run tests with:

dotnet test --logger:"console;verbosity=detailed"

## Diagrams

Available in public Figma:
[https://www.figma.com/design/bipcukLDKjcuigB1njVPnE/SurveyApi](https://www.figma.com/design/bipcukLDKjcuigB1njVPnE/SurveyApi)
