--Checke bei jedem Containerstart, ob die Datenbank und die Tabelle existieren, wenn nicht, werden sie erstellt
IF DB_ID('InventoryAPI') IS NULL
BEGIN
    CREATE DATABASE InventoryAPI;
END
GO

USE InventoryAPI;
GO

--DROP um aktuelle Änderungen zu übernehmen, z.B. neue Spalten oder geänderte Datentypen
--Durch das ausführen bei Dockerstart werden die Änderungen automatisch übernommen
DROP TABLE IF EXISTS dbo.products

GO

DROP TABLE IF EXISTS dbo.warehouses

GO

CREATE TABLE dbo.products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    sku NVARCHAR(20) NOT NULL,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(100) NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE()
);

GO

CREATE TABLE dbo.warehouses (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    is_active BIT NOT NULL DEFAULT 1,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE()

);

GO
-- Füge Beispielprodukte hinzu, wenn die Tabelle leer ist
IF NOT EXISTS (SELECT 1 FROM dbo.products)
    BEGIN
        INSERT INTO dbo.products (sku,name,description)
        VALUES
        ('coca-cola','Coca-Cola','A carbonated soft drink produced by The Coca-Cola Company.'),
        ('pepsi','Pepsi','A carbonated soft drink produced by PepsiCo.'),
        ('fanta','Fanta','A carbonated soft drink produced by PepsiCo.'),
        ('sprite','Sprite','A carbonated soft drink produced by The Coca-Cola Company.'),
        ('7up','7UP','A carbonated soft drink produced by The Coca-Cola Company.');
    END
GO

IF NOT EXISTS (SELECT 1 FROM dbo.warehouses)
BEGIN
    INSERT INTO dbo.warehouses ([Name],[Description])
    VALUES
    ('Main Warehouse Cologne', 'Primary distribution center for western Germany'),
    ('Berlin Storage Hub', 'Handles inventory for eastern Germany and Berlin region'),
    ('Munich Cold Storage', 'Temperature-controlled warehouse for beverages and perishables'),
    ('Hamburg Port Warehouse', 'Imports and exports via Hamburg harbor'),
    ('Overflow Storage NRW', 'Used during peak demand seasons for additional capacity');
END
GO