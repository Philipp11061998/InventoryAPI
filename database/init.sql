--Checke bei jedem Containerstart, ob die Datenbank und die Tabelle existieren, wenn nicht, werden sie erstellt
IF DB_ID('InventoryAPI') IS NULL
BEGIN
    CREATE DATABASE InventoryAPI;
END
GO

USE InventoryAPI;
GO

IF OBJECT_ID('dbo.products', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.products (
        Id INT IDENTITY(1,1) PRIMARY KEY,
        sku NVARCHAR(20) NOT NULL,
        name NVARCHAR(50) NOT NULL,
        description NVARCHAR(100) NOT NULL,
        is_active BIT NOT NULL DEFAULT 0,
        created_at DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

-- Füge Beispielprodukte hinzu, wenn die Tabelle leer ist
IF NOT EXISTS (SELECT 1 FROM dbo.products)
    BEGIN
        INSERT INTO dbo.products (sku,name,description)
        VALUES
        ('coca-cola','Coca-Cola','A carbonated soft drink produced by The Coca-Cola Company.'),
        ('pepsi','Pepsi','A carbonated soft drink produced by PepsiCo.');
    END
GO