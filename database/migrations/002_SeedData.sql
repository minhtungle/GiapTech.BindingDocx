-- Migration 002: Seed Data
-- Default user: admin / Admin@123456
-- BCrypt hash of 'Admin@123456'

USE GiapTechBindingDocx;
GO

-- Seed admin user (password: Admin@123456)
IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    DECLARE @AdminId UNIQUEIDENTIFIER = NEWID();
    INSERT INTO Users (Id, Username, Email, PasswordHash, IsActive, CreatedAt, UpdatedAt)
    VALUES (
        @AdminId,
        'admin',
        'admin@giaptech.vn',
        '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TiGHTGLnAZiVi1kT7XfyrMQG1mxO',
        1,
        GETUTCDATE(),
        GETUTCDATE()
    );

    INSERT INTO UserTokens (UserId, CurrentToken, CreatedAt, UpdatedAt)
    VALUES (@AdminId, 100, GETUTCDATE(), GETUTCDATE());
END
GO

-- Seed 6 profile groups
IF NOT EXISTS (SELECT 1 FROM ProfileGroups)
BEGIN
    INSERT INTO ProfileGroups (Name, Description, SortOrder, IsActive, CreatedAt)
    VALUES
        (N'Nhóm hồ sơ 1', N'Nhóm hồ sơ mặc định 1', 1, 1, GETUTCDATE()),
        (N'Nhóm hồ sơ 2', N'Nhóm hồ sơ mặc định 2', 2, 1, GETUTCDATE()),
        (N'Nhóm hồ sơ 3', N'Nhóm hồ sơ mặc định 3', 3, 1, GETUTCDATE()),
        (N'Nhóm hồ sơ 4', N'Nhóm hồ sơ mặc định 4', 4, 1, GETUTCDATE()),
        (N'Nhóm hồ sơ 5', N'Nhóm hồ sơ mặc định 5', 5, 1, GETUTCDATE()),
        (N'Nhóm hồ sơ 6', N'Nhóm hồ sơ mặc định 6', 6, 1, GETUTCDATE());
END
GO

-- Seed token packages
IF NOT EXISTS (SELECT 1 FROM TokenPackages)
BEGIN
    INSERT INTO TokenPackages (Name, TokenAmount, PricePerToken, TotalPrice, SortOrder, IsActive, CreatedAt)
    VALUES
        (N'Gói 1 Token',    1,    10000, 10000,   1, 1, GETUTCDATE()),
        (N'Gói 500 Token',  500,   7000, 3500000, 2, 1, GETUTCDATE()),
        (N'Gói 1000 Token', 1000,  5000, 5000000, 3, 1, GETUTCDATE());
END
GO
