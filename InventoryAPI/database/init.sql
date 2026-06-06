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
DROP TABLE IF EXISTS dbo.movements

GO
DROP TABLE IF EXISTS dbo.products

GO

DROP TABLE IF EXISTS dbo.warehouses

GO

DROP TABLE IF EXISTS dbo.users

GO

CREATE TABLE dbo.products (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    sku NVARCHAR(20) NOT NULL,
    name NVARCHAR(50) NOT NULL,
    description NVARCHAR(100) NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

GO

CREATE UNIQUE INDEX products_sku
ON dbo.products(sku);

GO

CREATE UNIQUE INDEX products_name
ON dbo.products(name);

GO

CREATE TABLE dbo.warehouses (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    [Name] NVARCHAR(50) NOT NULL,
    [Description] NVARCHAR(100) NOT NULL,
    IsActive BIT NOT NULL DEFAULT 1,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
);

GO

CREATE UNIQUE INDEX warehouses_name
ON dbo.warehouses([Name]);

GO

CREATE TABLE dbo.movements (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    ProductId INT NOT NULL,
    WarehouseId INT NOT NULL,
    Amount INT NOT NULL,
    MovementType INT NOT NULL,
    TransferReference NVARCHAR(100) NULL,
    Note NVARCHAR(100) NULL,
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
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

CREATE INDEX movements_product_id 
ON dbo.movements(ProductId);

GO

CREATE INDEX movements_warehouse_id
ON dbo.movements(WarehouseId);

GO

CREATE INDEX movements_created_at
ON dbo.movements(CreatedAt);

GO

CREATE TABLE dbo.users (
    Id INT IDENTITY(1,1) PRIMARY KEY, 
    Username NVARCHAR(50) NOT NULL,
    PasswordHash NVARCHAR(255) NOT NULL,
    Role NVARCHAR(20) NOT NULL DEFAULT 'User',
    CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
    IsActive BIT NOT NULL DEFAULT 1
);

GO

CREATE UNIQUE INDEX users_username
ON dbo.users(Username);

GO


IF NOT EXISTS (SELECT 1 FROM dbo.users)
    BEGIN
        INSERT INTO dbo.users (Username, PasswordHash, Role)
        VALUES
        ('Admin', '$2a$11$V96dygkrH2.AI2DybHlwOurBZlL/J4aHKoXlEBcbsAiZs/UiFBmla', 'Admin'), --Passwort: Admin123!
        ('User', '$2a$11$Hwx8Z.Twn93c0mnuCsaeN.k5M/3yL8RRXkA0yE5K9CfyuxHbDlMr6', 'User')   --Passwort: User123!
    END

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