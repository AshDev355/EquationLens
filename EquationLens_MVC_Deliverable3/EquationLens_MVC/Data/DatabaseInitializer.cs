using Dapper;

namespace EquationLens.Data;

/// <summary>Creates the application tables (not the Identity tables) with Dapper.</summary>
public static class DatabaseInitializer
{
    private const string Schema = @"
CREATE TABLE IF NOT EXISTS Equations (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId    TEXT NOT NULL,
    Equation  TEXT NOT NULL,
    Result    TEXT NOT NULL,
    Source    TEXT NOT NULL DEFAULT 'typed',
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_Equations_User_Created ON Equations (UserId, CreatedAt);

CREATE TABLE IF NOT EXISTS SolutionSteps (
    Id         INTEGER PRIMARY KEY AUTOINCREMENT,
    EquationId INTEGER NOT NULL,
    StepNumber INTEGER NOT NULL,
    Text       TEXT NOT NULL,
    FOREIGN KEY (EquationId) REFERENCES Equations(Id) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_SolutionSteps_Equation ON SolutionSteps (EquationId, StepNumber);

CREATE TABLE IF NOT EXISTS ActivityLogs (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    UserId    TEXT NULL,
    Action    TEXT NOT NULL,
    Detail    TEXT NOT NULL DEFAULT '',
    CreatedAt TEXT NOT NULL,
    FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE SET NULL
);
CREATE INDEX IF NOT EXISTS IX_ActivityLogs_Created ON ActivityLogs (CreatedAt);";

    public static async Task InitializeAsync(IDbConnectionFactory factory)
    {
        using var conn = factory.CreateConnection();
        await conn.ExecuteAsync(Schema);
    }
}
