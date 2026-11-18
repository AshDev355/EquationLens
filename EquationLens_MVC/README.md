# EquationLens — ASP.NET Core MVC + Identity

This is a complete MVC conversion of the supplied static EquationLens frontend.

## Included
- ASP.NET Core MVC controllers and Razor views for Home, Register, Login, Workspace and Admin.
- ASP.NET Core Identity authentication, password hashing, lockout, cookie auth and User/Admin roles.
- SQLite + Entity Framework Core persistence for accounts and per-user equation history.
- Role-protected admin dashboard and authenticated student workspace.
- Server-side linear/quadratic equation solver with step output.
- Existing EquationLens visual theme, Bootstrap layout and supplied equation images.

## Run in Visual Studio
1. Open `EquationLens.csproj` (or open the folder) in Visual Studio 2022 with the ASP.NET and web development workload.
2. Restore NuGet packages and run the project. The SQLite database `equationlens.db` is created automatically on first run.
3. Register a normal account from **Sign up**. It is assigned the `User` role automatically.

## Configure an Admin account
Do not expose a default admin password in source code. In Visual Studio Package Manager Console / terminal, set user secrets:

```bash
dotnet user-secrets init
dotnet user-secrets set "AdminSeed:Email" "admin@example.com"
dotnet user-secrets set "AdminSeed:Password" "Use-A-Strong-Password-123"
```

Restart the app. The configured account is created and assigned the `Admin` role on startup. Admin dashboard: `/Admin`.

## Supported equation examples
- `3x + 7 = 22`
- `x^2 - 9 = 0`
- `2x^2 - 5x + 3 = 0`

The current demo solver supports linear and quadratic equations in `x`. It is a basic educational parser, not a general-purpose computer algebra system.

## Important current limitations
- The image upload control currently provides a client-side image preview only. OCR / handwritten equation recognition (e.g. pix2tex) is not integrated yet; users type or correct the recognized equation manually.
- Email confirmation, password reset email delivery, external login providers and production database migration strategy are not configured.
- `EnsureCreated` is used to make the demo easy to run. For a deployed application, replace it with EF Core migrations and configure HTTPS, secrets, email and a production database.
