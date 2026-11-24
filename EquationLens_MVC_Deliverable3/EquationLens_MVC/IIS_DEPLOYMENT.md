# Local deployment on IIS (Windows)

Everything here is free and runs on your own machine. The app uses SQLite, so there is no database server to install.

## 1. One-time setup
1. **Enable IIS:** Windows Features > tick *Internet Information Services* (the defaults are enough) > OK.
2. **Install the .NET 8 Hosting Bundle** (download "Hosting Bundle" from the .NET 8 downloads page). Install it *after* IIS, then run `iisreset` in an Administrator command prompt.

## 2. Publish the app
In the project folder:
```
dotnet publish -c Release -o C:\inetpub\EquationLens
```

## 3. Create the site in IIS Manager
1. **Application Pools > Add Application Pool:** name `EquationLens`, **.NET CLR version: No Managed Code**, Integrated pipeline.
2. **Sites > Add Website:** site name `EquationLens`, application pool `EquationLens`, physical path `C:\inetpub\EquationLens`, binding `http`, port `8080`.

## 4. Give the app pool write access (the usual cause of errors)
The app writes its SQLite database and encryption keys to `App_Data`. In an Administrator command prompt:
```
mkdir C:\inetpub\EquationLens\App_Data
icacls "C:\inetpub\EquationLens\App_Data" /grant "IIS AppPool\EquationLens":(OI)(CI)M
```
(The name after `IIS AppPool\` must match your application pool.)

## 5. Optional: create the first admin account
Set two environment variables (System Properties > Environment Variables > System variables), then run `iisreset`:
```
AdminSeed__Email     admin@example.com
AdminSeed__Password  Use-A-Strong-Password-123
```
The password must meet the rules (8+ characters, upper and lower case, a digit) and must not contain the email name.

## 6. Open the site
Browse to `http://localhost:8080`. Sign up for a normal account, or log in with the admin account and go to `/Admin`.

## Troubleshooting
| Symptom | Cause and fix |
|---|---|
| HTTP 500.19 | The Hosting Bundle is missing, or was installed before IIS. Install it again and run `iisreset`. |
| HTTP 500.30 or 502.5 | The app failed to start. Create a `logs` folder in the site folder, set `stdoutLogEnabled="true"` in `web.config`, reload the site, and read `logs\stdout_*.log`. |
| "unable to open database file" | The app pool has no write access to `App_Data` (step 4). |
| You are logged out after a while or forms fail | `App_Data\keys` is not writable (step 4). |
| Page has no fonts or styles | The pages load Bootstrap and fonts from the internet. Connect to the internet or host those files locally. |
| A console warning about HTTPS redirection | Harmless on an http-only binding. |
