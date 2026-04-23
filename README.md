# LinguaJourney

LinguaJourney is an ASP.NET Core MVC foreign language learning app built around three connected learning modules:

- `Language Tracks` for target-language goals and milestones
- `Lessons` for structured study sessions
- `Practice Logs` for reflections, review dates, and performance tracking

## Stack

- ASP.NET Core MVC (.NET 8)
- Entity Framework Core
- SQL Server / SQL Server Express
- ASP.NET Core Identity

## Main Features

- Username/password authentication
- Full CRUD flows with details pages
- Two layout pages
- View models and validation attributes
- MVC data transfer usage with `ViewBag`, `ViewData`, `TempData`, and strongly typed models
- Code-first database setup through `ApplicationDbContext`

## Project Structure

- `Controllers/` contains MVC controllers for account, dashboard, tracks, lessons, and practice logs
- `Models/` contains EF Core entities and the Identity user model
- `ViewModels/` contains form/list/details view models
- `Views/` contains Razor pages and shared layouts
- `Data/` contains the EF Core database context

## Run Locally

1. Install .NET 8 SDK
2. Make sure SQL Server Express is available
3. Update the connection string in `appsettings.json` if needed
4. Run:

```powershell
dotnet build
dotnet run
```

By default, the app has been tested locally at:

```text
http://127.0.0.1:5057
```

## Notes

- The project folder name remains `CourseNotesSharing`, but the app itself has been reworked into `LinguaJourney`
- The compiled assembly name is set to `LinguaJourney`
