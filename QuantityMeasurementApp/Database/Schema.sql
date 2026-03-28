-- QuantityMeasurementDB Database Schema
-- Complete schema with Users and RefreshTokens tables
-- Run this in SQL Server Management Studio (SSMS)

CREATE DATABASE QuantityMeasurementDB;
GO

USE QuantityMeasurementDB;
GO

-- =====================================================
-- QUANTITY MEASUREMENTS TABLE
-- =====================================================
CREATE TABLE QuantityMeasurements (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    MeasurementId NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    OperationType INT NOT NULL,
    
    -- First operand fields
    FirstOperandValue FLOAT NULL,
    FirstOperandUnit NVARCHAR(20) NULL,
    FirstOperandCategory NVARCHAR(20) NULL,
    
    -- Second operand fields
    SecondOperandValue FLOAT NULL,
    SecondOperandUnit NVARCHAR(20) NULL,
    SecondOperandCategory NVARCHAR(20) NULL,
    
    -- Target unit field
    TargetUnit NVARCHAR(20) NULL,
    
    -- Source operand fields (for conversion)
    SourceOperandValue FLOAT NULL,
    SourceOperandUnit NVARCHAR(20) NULL,
    SourceOperandCategory NVARCHAR(20) NULL,
    
    -- Result fields
    ResultValue FLOAT NULL,
    ResultUnit NVARCHAR(20) NULL,
    FormattedResult NVARCHAR(200) NULL,
    IsSuccessful BIT NOT NULL,
    ErrorDetails NVARCHAR(MAX) NULL
);
GO

-- QuantityMeasurements Indexes and Constraints
CREATE UNIQUE INDEX IX_QuantityMeasurements_MeasurementId 
    ON QuantityMeasurements(MeasurementId);
GO

CREATE INDEX IX_QuantityMeasurements_CreatedAt ON QuantityMeasurements(CreatedAt);
CREATE INDEX IX_QuantityMeasurements_OperationType ON QuantityMeasurements(OperationType);
CREATE INDEX IX_QuantityMeasurements_FirstCategory ON QuantityMeasurements(FirstOperandCategory);
CREATE INDEX IX_QuantityMeasurements_SecondCategory ON QuantityMeasurements(SecondOperandCategory);
CREATE INDEX IX_QuantityMeasurements_SourceCategory ON QuantityMeasurements(SourceOperandCategory);
GO

-- =====================================================
-- USERS TABLE (Authentication)
-- =====================================================
CREATE TABLE Users (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(50) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    PasswordHash NVARCHAR(MAX) NOT NULL,
    FirstName NVARCHAR(50) NOT NULL,
    LastName NVARCHAR(50) NOT NULL,
    CreatedAt DATETIME2 NOT NULL,
    LastLoginAt DATETIME2 NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    Role NVARCHAR(50) NOT NULL DEFAULT 'User',
    FailedLoginAttempts INT NOT NULL DEFAULT 0,
    LockoutEnd DATETIME2 NULL
);
GO

-- Users Unique Constraints
ALTER TABLE Users ADD CONSTRAINT UQ_Users_Username UNIQUE (Username);
ALTER TABLE Users ADD CONSTRAINT UQ_Users_Email UNIQUE (Email);
GO

-- Users Indexes
CREATE INDEX IX_Users_Username ON Users(Username);
CREATE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Users_CreatedAt ON Users(CreatedAt);
CREATE INDEX IX_Users_IsActive ON Users(IsActive);
CREATE INDEX IX_Users_Role ON Users(Role);
GO

-- =====================================================
-- REFRESH TOKENS TABLE
-- =====================================================
CREATE TABLE RefreshTokens (
    Id BIGINT IDENTITY(1,1) PRIMARY KEY,
    UserId BIGINT NOT NULL,
    Token NVARCHAR(200) NOT NULL,
    ExpiresAt DATETIME2 NOT NULL,
    RevokedAt DATETIME2 NULL,
    RevokedByIp NVARCHAR(50) NULL,
    ReplacedByToken NVARCHAR(200) NULL,
    CreatedByIp NVARCHAR(50) NULL,
    CreatedAt DATETIME2 NOT NULL
);
GO

-- RefreshTokens Unique Constraints
ALTER TABLE RefreshTokens ADD CONSTRAINT UQ_RefreshTokens_Token UNIQUE (Token);
GO

-- RefreshTokens Foreign Key
ALTER TABLE RefreshTokens 
ADD CONSTRAINT FK_RefreshTokens_Users 
FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE;
GO

-- RefreshTokens Indexes
CREATE INDEX IX_RefreshTokens_UserId ON RefreshTokens(UserId);
CREATE INDEX IX_RefreshTokens_Token ON RefreshTokens(Token);
CREATE INDEX IX_RefreshTokens_ExpiresAt ON RefreshTokens(ExpiresAt);
CREATE INDEX IX_RefreshTokens_RevokedAt ON RefreshTokens(RevokedAt);
GO

-- =====================================================
-- QUANTITY MEASUREMENT AUDIT TABLE (Optional)
-- =====================================================
CREATE TABLE QuantityMeasurementAudit (
    AuditId INT IDENTITY(1,1) PRIMARY KEY,
    MeasurementId NVARCHAR(50) NOT NULL,
    ActionType NVARCHAR(20) NOT NULL,
    OldValues NVARCHAR(MAX) NULL,
    NewValues NVARCHAR(MAX) NULL,
    ChangedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    ChangedBy NVARCHAR(50) NULL
);
GO

-- Audit table foreign key
ALTER TABLE QuantityMeasurementAudit
ADD CONSTRAINT FK_QuantityMeasurementAudit_QuantityMeasurements
FOREIGN KEY (MeasurementId) REFERENCES QuantityMeasurements(MeasurementId);
GO

-- Audit table indexes
CREATE INDEX IX_QuantityMeasurementAudit_MeasurementId ON QuantityMeasurementAudit(MeasurementId);
CREATE INDEX IX_QuantityMeasurementAudit_ChangedAt ON QuantityMeasurementAudit(ChangedAt);
GO

-- =====================================================
-- STORED PROCEDURES
-- =====================================================

-- Stored procedure for getting measurement statistics
CREATE PROCEDURE sp_GetMeasurementStatistics
AS
BEGIN
    SELECT 
        COUNT(*) as TotalRecords,
        SUM(CASE WHEN IsSuccessful = 1 THEN 1 ELSE 0 END) as SuccessfulOperations,
        SUM(CASE WHEN IsSuccessful = 0 THEN 1 ELSE 0 END) as FailedOperations,
        MIN(CreatedAt) as FirstOperation,
        MAX(CreatedAt) as LastOperation,
        COUNT(DISTINCT FirstOperandCategory) as CategoriesUsed
    FROM QuantityMeasurements;
    
    SELECT 
        OperationType,
        COUNT(*) as RecordCount
    FROM QuantityMeasurements
    GROUP BY OperationType
    ORDER BY OperationType;
END
GO

-- Stored procedure for getting user statistics
CREATE PROCEDURE sp_GetUserStatistics
AS
BEGIN
    SELECT 
        COUNT(*) as TotalUsers,
        SUM(CASE WHEN IsActive = 1 THEN 1 ELSE 0 END) as ActiveUsers,
        SUM(CASE WHEN IsActive = 0 THEN 1 ELSE 0 END) as InactiveUsers,
        MIN(CreatedAt) as FirstUser,
        MAX(CreatedAt) as LatestUser
    FROM Users;
    
    SELECT 
        Role,
        COUNT(*) as UserCount
    FROM Users
    GROUP BY Role
    ORDER BY Role;
END
GO

-- =====================================================
-- VIEWS
-- =====================================================

-- View for measurement summary
CREATE VIEW vw_MeasurementSummary AS
SELECT 
    Id,
    MeasurementId,
    CreatedAt,
    CASE OperationType
        WHEN 0 THEN 'Compare'
        WHEN 1 THEN 'Convert'
        WHEN 2 THEN 'Add'
        WHEN 3 THEN 'Subtract'
        WHEN 4 THEN 'Divide'
    END as OperationName,
    FirstOperandValue,
    FirstOperandUnit,
    FirstOperandCategory,
    SecondOperandValue,
    SecondOperandUnit,
    SecondOperandCategory,
    SourceOperandValue,
    SourceOperandUnit,
    SourceOperandCategory,
    TargetUnit,
    ResultValue,
    ResultUnit,
    FormattedResult,
    IsSuccessful,
    ErrorDetails
FROM QuantityMeasurements;
GO

-- View for user summary
CREATE VIEW vw_UserSummary AS
SELECT 
    Id,
    Username,
    Email,
    FirstName,
    LastName,
    CreatedAt,
    LastLoginAt,
    IsActive,
    Role,
    FailedLoginAttempts,
    CASE 
        WHEN LockoutEnd > GETUTCDATE() THEN 'Locked'
        ELSE 'Active'
    END as AccountStatus
FROM Users;
GO

-- =====================================================
-- SAMPLE QUERIES FOR TESTING
-- =====================================================

-- View recent measurements
SELECT TOP 20 * FROM QuantityMeasurements 
ORDER BY CreatedAt DESC;
GO

-- View all users
SELECT * FROM Users
ORDER BY CreatedAt DESC;
GO

-- View active refresh tokens
SELECT * FROM RefreshTokens 
WHERE RevokedAt IS NULL AND ExpiresAt > GETUTCDATE()
ORDER BY CreatedAt DESC;
GO

-- View locked accounts
SELECT * FROM Users 
WHERE LockoutEnd > GETUTCDATE() OR IsActive = 0;
GO

-- Run statistics procedures
EXEC sp_GetMeasurementStatistics;
EXEC sp_GetUserStatistics;
GO

-- View summaries
SELECT * FROM vw_MeasurementSummary;
SELECT * FROM vw_UserSummary;
GO

SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME IN ('QuantityMeasurements', 'Users', 'RefreshTokens', 'AuditLogs')
ORDER BY TABLE_NAME;

USE QuantityMeasurementDB;
UPDATE Users SET Role = 'Admin' WHERE Email = 'sankalpagarwal8057@gmail.com';
SELECT * FROM Users WHERE Email = 'sankalpagarwal8057@gmail.com';
SELECT Id, Username, Email, Role, FirstName, LastName FROM Users;

