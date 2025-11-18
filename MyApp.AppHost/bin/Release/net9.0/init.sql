IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = ''T_TodoItems'')
BEGIN
    CREATE TABLE dbo.T_TodoItems
    (
        Id INT IDENTITY PRIMARY KEY,
        Title NVARCHAR(200) NOT NULL,
        IsDone BIT NOT NULL DEFAULT 0,
        CreatedAt DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

    INSERT INTO dbo.T_TodoItems (Title, IsDone)
    VALUES
        (''Review architecture'', 0),
        (''Implement API endpoints'', 0),
        (''Write integration tests'', 0);
END
