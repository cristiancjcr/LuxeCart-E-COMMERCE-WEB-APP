-- ============================================================
-- ECommerceDB - Creation & Seed Script
-- Run this script against your MS SQL Server instance.
-- It creates the database, all tables, and seeds product data.
-- ============================================================

USE master;
GO

-- Drop and recreate the database for a clean install
IF EXISTS (SELECT name FROM sys.databases WHERE name = 'ECommerceDB')
BEGIN
    ALTER DATABASE ECommerceDB SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE ECommerceDB;
END
GO

CREATE DATABASE ECommerceDB;
GO

USE ECommerceDB;
GO

-- ============================================================
-- TABLE: Users
-- Stores registered user accounts. Passwords are stored as
-- BCrypt hashes — never plain text.
-- ============================================================
CREATE TABLE Users (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    FirstName   NVARCHAR(100)  NOT NULL,
    LastName    NVARCHAR(100)  NOT NULL,
    Email       NVARCHAR(255)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(512) NOT NULL,
    Role        NVARCHAR(20)   NOT NULL DEFAULT 'Customer'
                CHECK (Role IN ('Customer', 'Admin')),
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ============================================================
-- TABLE: Products
-- The product catalog. Prices in USD stored as DECIMAL(10,2).
-- ============================================================
CREATE TABLE Products (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    Name        NVARCHAR(200)  NOT NULL,
    Description NVARCHAR(1000) NOT NULL,
    Price       DECIMAL(10,2)  NOT NULL CHECK (Price >= 0),
    Category    NVARCHAR(100)  NOT NULL,
    ImageUrl    NVARCHAR(500)  NOT NULL,
    Stock       INT            NOT NULL DEFAULT 0 CHECK (Stock >= 0),
    CreatedAt   DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ============================================================
-- TABLE: CartItems
-- Persists cart state per user. One row per product-per-user.
-- ============================================================
CREATE TABLE CartItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    UserId      INT  NOT NULL REFERENCES Users(Id) ON DELETE CASCADE,
    ProductId   INT  NOT NULL REFERENCES Products(Id) ON DELETE CASCADE,
    Quantity    INT  NOT NULL DEFAULT 1 CHECK (Quantity > 0),
    CONSTRAINT UQ_Cart_UserProduct UNIQUE (UserId, ProductId)
);
GO

-- ============================================================
-- TABLE: Orders
-- One row per placed order. TotalPrice is ALWAYS computed
-- server-side — never trusted from the client.
-- ============================================================
CREATE TABLE Orders (
    Id              INT IDENTITY(1,1) PRIMARY KEY,
    UserId          INT            NOT NULL REFERENCES Users(Id),
    TotalPrice      DECIMAL(10,2)  NOT NULL,
    ShippingAddress NVARCHAR(500)  NOT NULL,
    Status          NVARCHAR(50)   NOT NULL DEFAULT 'Pending',
    CreatedAt       DATETIME2      NOT NULL DEFAULT GETUTCDATE()
);
GO

-- ============================================================
-- TABLE: OrderItems
-- Line items for each order — snapshot of price at order time.
-- ============================================================
CREATE TABLE OrderItems (
    Id          INT IDENTITY(1,1) PRIMARY KEY,
    OrderId     INT            NOT NULL REFERENCES Orders(Id) ON DELETE CASCADE,
    ProductId   INT            NOT NULL REFERENCES Products(Id),
    Quantity    INT            NOT NULL,
    UnitPrice   DECIMAL(10,2)  NOT NULL  -- snapshot of price at order time
);
GO

PRINT 'ECommerceDB schema created successfully.';
GO
