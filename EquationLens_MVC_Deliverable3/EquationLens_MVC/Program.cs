using Dapper;
using EquationLens.Data;
using EquationLens.Identity;
using EquationLens.Models;
using EquationLens.Repositories;
using EquationLens.Services;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

// ---- Database path (absolute, so it works the same under IIS) ----
var csb = new SqliteConnectionStringBuilder(builder.Configuration.GetConnectionString("DefaultConnection"));
if (!Path.IsPathRooted(csb.DataSource))
    csb.DataSource = Path.Combine(builder.Environment.ContentRootPath, csb.DataSource);
Directory.CreateDirectory(Path.GetDirectoryName(csb.DataSource)!);
var connectionString = csb.ToString();

// ---- EF Core: Identity tables only ----
builder.Services.AddDbContext<ApplicationDbContext>(o => o.UseSqlite(connectionString));

// ---- Dapper + repositories (dependency injection) ----
SqlMapper.AddTypeHandler(new DateTimeHandler());
builder.Services.AddSingleton<IDbConnectionFactory>(new SqliteConnectionFactory(connectionString));
builder.Services.AddScoped<IEquationRepository, EquationRepository>();
builder.Services.AddScoped<ISolutionStepRepository, SolutionStepRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IActivityLogRepository, ActivityLogRepository>();

// ---- Services ----
builder.Services.AddScoped<IEquationSolverService, EquationSolverService>();
builder.Services.AddScoped<IRecognitionClient, DemoRecognitionClient>(); // swap for a real recognizer later

// ---- Customized Identity ----
builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
{
    options.Password.RequiredLength = 8;
    options.Password.RequireDigit = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.User.RequireUniqueEmail = true;
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddClaimsPrincipalFactory<AppClaimsPrincipalFactory>()
.AddPasswordValidator<NoPersonalInfoPasswordValidator>();

builder.Services.ConfigureApplicationCookie(o =>
{
    o.LoginPath = "/Account/Login";
    o.AccessDeniedPath = "/Account/AccessDenied";
    o.Cookie.Name = "EquationLens.Auth";
    o.Cookie.HttpOnly = true;
    o.ExpireTimeSpan = TimeSpan.FromDays(7);
    o.SlidingExpiration = true;
});

// AJAX calls send the anti-forgery token in this header.
builder.Services.AddAntiforgery(o => o.HeaderName = "RequestVerificationToken");

// Keep data-protection keys on disk so logins survive an IIS app pool recycle.
builder.Services.AddDataProtection()
    .SetApplicationName("EquationLens")
    .PersistKeysToFileSystem(new DirectoryInfo(Path.Combine(builder.Environment.ContentRootPath, "App_Data", "keys")));

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var sp = scope.ServiceProvider;
    await sp.GetRequiredService<ApplicationDbContext>().Database.EnsureCreatedAsync(); // Identity tables
    await DatabaseInitializer.InitializeAsync(sp.GetRequiredService<IDbConnectionFactory>()); // Dapper tables
    await IdentitySeeder.SeedAsync(sp, app.Configuration);
}

app.MapControllerRoute(name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
