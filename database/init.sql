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

CREATE TABLE dbo.products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    sku NVARCHAR(20) NOT NULL,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(100) NULL,
    is_active BIT NOT NULL DEFAULT 0,
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