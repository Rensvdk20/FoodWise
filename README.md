
# FoodWise

FoodWise is a project aimed at reducing food waste by allowing students to buy surplus food from the campus canteens. This project is built using .NET 6, uses SQL Server as the database and includes both a REST API and a GraphQL endpoint.

## Technologies
- **[.NET 6](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)**
    - **[Entity Framework](https://learn.microsoft.com/en-us/ef/)**
    - **[Identity Framework](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity)**
    
- **[SCSS](https://sass-lang.com/)**

---

## Requirements

To run the code in this repository, you need to have the following:

- **.NET (v6)**
- **SQL Server**

## Setup

 - Clone the repository:
    ```bash
    git clone https://github.com/Rensvdk20/Foodwise.git
    ```

- Restore dependencies and build the solution:
    ```bash
    dotnet restore
    dotnet build
    ```

- Set up environment variables:
    - Create an `appsettings.json` file in the `Portal/` directory.
    - Add the following to the file:

        ```json
        {
            "Logging": {
                "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
                }
            },
            "AllowedHosts": "*",
            "ConnectionStrings": {
                "EFConnection": "<Your EF database connection string>",
                "IFConnection": "<Your IF database connection string>"
            }
        }

        ```

    - Create an `appsettings.json` file in the `WebApi/` directory.
    - Add the following to the file:

        ```json
        {
            "Logging": {
                "LogLevel": {
                "Default": "Information",
                "Microsoft.AspNetCore": "Warning"
                }
            },
            "AllowedHosts": "*",
            "ConnectionStrings": {
                "EFConnection": "<Your EF database connection string>",
            }
        }

        ```

## Usage

To start the application, run:
```bash
# Web Application
cd Portal/
dotnet run --project Portal/Portal.csproj

# API
cd WebApi/
dotnet run --project WebApi/WebApi.csproj
```