# FVect Backend

## Architecture

The application is layered and consists of three layers:

1. The API Layer who is responsible for handling HTTP requests.
2. The Business layer containing business logic and queries.
3. The Data layer which is responsible for connecting to data sources such as HTTP APIs and Databases.

## Getting Started

Getting started is simple:

1. Copy 'API\appsettings.Local.Example.json' to 'API\appsettings.Local.json'
2. Change the SQL Connection string in the created 'API\appsettings.Local.json' file to your local development server.
3. Ensure you import the necessary secrets, see the secrets part of the Readme below.
4. Build & Launch the application using an IDE.

## Entity Framework Commands

To use Entity Framework commands, the 'Data.Design' project must be selected as the Startup Project, and the Data project must be selected as Default project in Visual Studio. 
To change the target database, change the connection string in 'FVectContextFactory.cs'.

Basic tutorial on commands:

1. Open Package Manager Console
	1.1 Tools
	1.2 NuGet Package Manager -> Package Manager Console
2. Default project -> Data

To reset database run following command:
	-> update-database -migration 0

To create a migration run following command (After making changes to the model):
	-> add-migration [migrationName] (without []'s)

To run migrations
	-> update-database -verbose


To fix "Unable to generate an explicit migration because the following explicit migrations are pending: [migration name].
Apply the pending explicit migrations before attempting to generate a new explicit migration."
	-> update-database -target [migration name]

Running Update-Database automatically runs seeder for data seeding

## Secrets in development

To securely share secrets, such as API keys, in development,
the [.NET Core secrets management tool](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1) is used.
Never check secrets in to source control. For instructions on how to use the tool, please refer to the Microsoft Documentation.
The following settings are currently classified as a secret and managed using the secret management tool:

| Key  | Rationale |
|------|-----------|
| `Geo:HereMapsAPIKey` | API Key used to communicate with HERE Maps. |
| `AuthNR:JWTSigningKey` | Key used to sign JWTs. Must be at least 16 characters. |

## Client identifiers

The following client identifiers have been deployed in the development environment

| Identifier | Use for |
|------------|---------|
| `3fa85f64-5717-4562-b3fc-2c963f66afa6` | Swagger UI. |
| `2a7d62cb-755c-42ae-9063-55c6b5fe793f` | Development tools that are not the Swagger UI. |
| `8f7fbd3b-3a1f-4884-ba77-77a5a8b58175` | Backoffice. |
| `eb59e004-3921-45ec-95ef-07553d4a9b85` | App. |

When developing locally, an action on the DevController can add these identifiers
to your local database.
