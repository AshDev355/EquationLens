# EquationLens, ASP.NET Core MVC (Deliverable 3)

Builds on the MVC project from Deliverable 2. Recognition is a demo (see "Limits"); linear and quadratic solving with steps is real.

## Where each requirement lives
| Requirement | Files |
|---|---|
| **Layout and sections** | `Views/Shared/_Layout.cshtml` (sections `Styles`, `PageHeader`, `Scripts`), `_AdminLayout.cshtml` (nested layout, section `AdminActions`), partials `_Navbar`, `_Footer` |
| **Identity customization** | `Models/ApplicationUser.cs` (FullName, CreatedAt, LastLoginAt), `Models/ApplicationRole.cs` (Description), renamed tables in `Data/ApplicationDbContext.cs`, `Identity/AppClaimsPrincipalFactory.cs` (FullName claim), `Identity/NoPersonalInfoPasswordValidator.cs`, `Identity/IdentitySeeder.cs` (roles and optional admin), cookie settings in `Program.cs` |
| **Dapper** | `Data/` (connection factory, schema, date handler) and every class in `Repositories/`. EF Core is used only for the Identity tables |
| **Repositories + DI** | `Repositories/Interfaces/`: `IEquationRepository`, `ISolutionStepRepository`, `IUserRepository`, `IActivityLogRepository`. Registered in `Program.cs` and injected into controllers. Also `IEquationSolverService` and `IRecognitionClient` |
| **AJAX** | `wwwroot/js/workspace.js` (recognize, solve, history, steps, delete, clear), `admin.js` (live search, paging, activity refresh), `register.js` (email availability). Partial views are returned by `History`, `UsersTable`, `ActivityFeed`. Anti-forgery token sent in a header |
| **IIS** | `IIS_DEPLOYMENT.md`, `web.config` |

## Run locally
Open the folder in Visual Studio 2022 (or run `dotnet run`). The database is created at `App_Data/equationlens.db` on first run. This is a new location, so the old `equationlens.db` from Deliverable 2 is not used.

First admin (optional), in the project folder:
```
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "Use-A-Strong-Password-123"
```

## Limits
- Image recognition is a **demo**: it returns a sample equation (based on the file name), not text read from your image. The user reviews and edits it before solving. A real recognizer can be added later as another `IRecognitionClient` class registered in `Program.cs`.
- The solver handles linear and quadratic equations in `x` only.
- `EnsureCreated` is used instead of migrations to keep setup simple.
