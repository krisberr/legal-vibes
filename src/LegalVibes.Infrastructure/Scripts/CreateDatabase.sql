-- Create LegalVibes Database
USE [master]
GO

-- Drop database if it exists
IF EXISTS (SELECT * FROM sys.databases WHERE name = 'LegalVibesDb')
BEGIN
    ALTER DATABASE [LegalVibesDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [LegalVibesDb];
END
GO

-- Create database
CREATE DATABASE [LegalVibesDb]
COLLATE SQL_Latin1_General_CP1_CI_AS;
GO

-- Set recovery model and other database options
ALTER DATABASE [LegalVibesDb] SET RECOVERY SIMPLE;
ALTER DATABASE [LegalVibesDb] SET MULTI_USER;
GO

-- Use the new database
USE [LegalVibesDb]
GO

-- Create schema for our application
CREATE SCHEMA [LegalVibes]
GO

-- Print success message
PRINT 'LegalVibesDb database has been created successfully.'
GO 