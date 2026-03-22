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

DROP TABLE IF EXISTS dbo.movements

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

CREATE TABLE dbo.movements (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    ProductId INT NOT NULL,
    WarehouseId INT NOT NULL,
    Amount INT NOT NULL,
    MovementType INT NOT NULL,
    TransferReference NVARCHAR(100) NULL,
    Note NVARCHAR(100) NULL,
    created_at DATETIME2 NOT NULL DEFAULT GETDATE(),
    CONSTRAINT FK_movements_productId FOREIGN KEY(ProductId)
        REFERENCES dbo.products(Id)
        ON UPDATE NO ACTION
        ON DELETE NO ACTION,
    CONSTRAINT FK_movements_warehouseId FOREIGN KEY(WarehouseId)
        REFERENCES dbo.warehouses(Id)
        ON UPDATE NO ACTION
        ON DELETE NO ACTION
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

IF NOT EXISTS (SELECT 1 FROM dbo.movements)
BEGIN
    INSERT INTO dbo.movements 
    (ProductId, WarehouseId, Amount, MovementType, TransferReference, Note)
    VALUES
    -- Initial stock (Wareneingang)
    (1, 1, 100, 1, NULL, 'Initial stock Coca-Cola Cologne'),
    (2, 1, 150, 1, NULL, 'Initial stock Pepsi Cologne'),
    (3, 2, 200, 1, NULL, 'Initial stock Fanta Berlin'),
    (4, 3, 120, 1, NULL, 'Initial stock Sprite Munich'),
    (5, 4, 180, 1, NULL, 'Initial stock 7UP Hamburg'),

    -- Verkäufe (Stock Out)
    (1, 1, 10, 2, NULL, 'Sold 10 Coca-Cola'),
    (2, 1, 20, 2, NULL, 'Sold 20 Pepsi'),
    (3, 2, 15, 2, NULL, 'Sold 15 Fanta'),

    -- Nachschub
    (1, 2, 80, 1, NULL, 'Restock Coca-Cola Berlin'),
    (3, 3, 60, 1, NULL, 'Restock Fanta Munich');
END
GO