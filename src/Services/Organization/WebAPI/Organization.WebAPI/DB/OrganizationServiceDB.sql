-- =============================================================================
-- Organization Service Database (SQL Server)
-- =============================================================================
-- Database      : OrganizationDB
-- Responsibility: Company profiles and organization management
-- SQL Server    : 2019+
-- =============================================================================

IF DB_ID('OrganizationDB') IS NULL
BEGIN
    CREATE DATABASE OrganizationDB;
END
GO

USE OrganizationDB;
GO

-- =============================================================================
-- Organizations
-- =============================================================================
CREATE TABLE Organizations
(
    Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    Name NVARCHAR(200) NOT NULL,
    Industry NVARCHAR(100) NULL,
    Website NVARCHAR(256) NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(20) NULL,
    Address NVARCHAR(512) NULL,
    LogoUrl NVARCHAR(512) NULL,

    CreatedOn DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NULL
);
GO

-- =============================================================================
-- Organization Members
-- =============================================================================
CREATE TABLE OrganizationMembers
(
    Id BIGINT IDENTITY(1,1) NOT NULL PRIMARY KEY,

    OrganizationId BIGINT NOT NULL,
    UserId BIGINT NOT NULL,

    FullName NVARCHAR(200) NOT NULL,
    Email NVARCHAR(256) NULL,
    Phone NVARCHAR(20) NULL,

    Role NVARCHAR(30) NOT NULL,

    CreatedOn DATETIME2 NOT NULL,
    CreatedBy NVARCHAR(100) NULL,
    UpdatedOn DATETIME2 NULL,
    UpdatedBy NVARCHAR(100) NULL,
    IsActive BIT NULL,

    CONSTRAINT FK_OrganizationMembers_Organizations
        FOREIGN KEY (OrganizationId)
        REFERENCES Organizations(Id)
        ON DELETE CASCADE,

    CONSTRAINT CK_OrganizationMembers_Role
        CHECK (Role IN ('OrganizationAdmin', 'Recruiter')),

    CONSTRAINT UQ_OrganizationMembers_Organization_User
        UNIQUE (OrganizationId, UserId)
);
GO

-- =============================================================================
-- Indexes
-- =============================================================================
CREATE INDEX IX_OrganizationMembers_OrganizationId
ON OrganizationMembers(OrganizationId);
GO

CREATE INDEX IX_OrganizationMembers_UserId
ON OrganizationMembers(UserId);
GO