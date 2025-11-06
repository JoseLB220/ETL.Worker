-- sql/create_staging_table.sql
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[staging]') AND type in (N'U'))
BEGIN
    CREATE TABLE dbo.staging (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        Source NVARCHAR(50),
        UserName NVARCHAR(200),
        Rating INT,
        Comment NVARCHAR(MAX),
        CreatedAt DATETIME2,
        InsertedAt DATETIME2 DEFAULT SYSUTCDATETIME()
    );
END
