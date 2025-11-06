-- sql/create_temp_user.sql
-- 1) Crear base de datos de prueba
IF DB_ID('ETLTestDB') IS NULL
    CREATE DATABASE ETLTestDB;
GO

USE ETLTestDB;
GO

-- 2) Crear tabla staging (si no la generaste antes)
IF OBJECT_ID('dbo.staging','U') IS NULL
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
GO

-- 3) Crear login y usuario SQL temporal con permisos solo en ETLTestDB
IF NOT EXISTS (SELECT * FROM sys.server_principals WHERE name = 'etl_user')
BEGIN
    CREATE LOGIN etl_user WITH PASSWORD = 'TempPass!2025';
END
GO

USE ETLTestDB;
GO

IF NOT EXISTS (SELECT * FROM sys.database_principals WHERE name = 'etl_user')
BEGIN
    CREATE USER etl_user FOR LOGIN etl_user;
    -- Conceder permisos mínimos: INSERT y SELECT en staging
    GRANT SELECT, INSERT ON dbo.staging TO etl_user;
END
GO
