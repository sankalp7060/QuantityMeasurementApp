-- QuantityMeasurementDB Database Schema
-- Using proper naming conventions (no numbers in column names)
-- Run this in SQL Server Management Studio (SSMS)

CREATE DATABASE QuantityMeasurementDB;
GO

USE QuantityMeasurementDB;
GO

-- Main measurements table with proper column names
CREATE TABLE QuantityMeasurements (
    MeasurementId NVARCHAR(50) PRIMARY KEY,
    CreatedAt DATETIME2 NOT NULL,
    OperationType INT NOT NULL,
    
    -- First operand fields (formerly Quantity1Value)
    FirstOperandValue FLOAT NULL,
    FirstOperandUnit NVARCHAR(20) NULL,
    FirstOperandCategory NVARCHAR(20) NULL,
    
    -- Second operand fields (formerly Quantity2Value)
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

-- Indexes for performance
CREATE INDEX IX_QuantityMeasurements_CreatedAt ON QuantityMeasurements(CreatedAt);
CREATE INDEX IX_QuantityMeasurements_OperationType ON QuantityMeasurements(OperationType);
CREATE INDEX IX_QuantityMeasurements_FirstCategory ON QuantityMeasurements(FirstOperandCategory);
CREATE INDEX IX_QuantityMeasurements_SecondCategory ON QuantityMeasurements(SecondOperandCategory);
CREATE INDEX IX_QuantityMeasurements_SourceCategory ON QuantityMeasurements(SourceOperandCategory);
GO



-- Stored procedure for getting statistics
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

-- View for easy reporting
CREATE VIEW vw_MeasurementSummary AS
SELECT 
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


-- Audit table for tracking changes (optional)
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

-- Sample queries for testing
-- SELECT * FROM QuantityMeasurements ORDER BY CreatedAt DESC;
-- SELECT * FROM vw_MeasurementSummary WHERE IsSuccessful = 1;
-- EXEC sp_GetMeasurementStatistics;
-- ALTER DATABASE QuantityMeasurementDB SET OFFLINE WITH ROLLBACK IMMEDIATE;
-- ALTER DATABASE QuantityMeasurementDB SET ONLINE;