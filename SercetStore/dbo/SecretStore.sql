CREATE DATABASE SecretStore
GO

USE [SecretStore]
GO
/****** Object:  Table [dbo].[PasswordEntriesHistory]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordEntriesHistory](
	[Id] [int] NOT NULL,
	[Website] [nvarchar](max) NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[SysStartTime] [datetime2](7) NOT NULL,
	[SysEndTime] [datetime2](7) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
DATA_COMPRESSION = PAGE
)
GO
/****** Object:  Table [dbo].[PasswordEntries]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Website] [nvarchar](max) NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[CreatedOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
	[SysStartTime] [datetime2](7) GENERATED ALWAYS AS ROW START NOT NULL,
	[SysEndTime] [datetime2](7) GENERATED ALWAYS AS ROW END NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY],
	PERIOD FOR SYSTEM_TIME ([SysStartTime], [SysEndTime])
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
WITH
(
SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[PasswordEntriesHistory])
)
GO
ALTER TABLE [dbo].[PasswordEntries] ADD  DEFAULT (getdate()) FOR [CreatedOn]
GO
ALTER TABLE [dbo].[PasswordEntries] ADD  DEFAULT (getdate()) FOR [UpdatedOn]
GO
/****** Object:  StoredProcedure [dbo].[CreatePasswordEntry]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Create a new password entry
CREATE PROCEDURE [dbo].[CreatePasswordEntry]
    @Website NVARCHAR(MAX),
    @Username NVARCHAR(MAX),
    @Password NVARCHAR(MAX),
	@EntityId INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PasswordEntries (Website, Username, Password, CreatedOn)
    VALUES (@Website, @Username, @Password, GETDATE());

    -- Set the newly created ID
    SET @EntityId = SCOPE_IDENTITY();
END;
GO
/****** Object:  StoredProcedure [dbo].[DeletePasswordEntry]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Delete a password entry
CREATE PROCEDURE [dbo].[DeletePasswordEntry]
    @Id INT,
	@Count INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PasswordEntries
    WHERE Id = @Id;

    -- Set the number of rows affected
    SET @Count = @@ROWCOUNT;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetPasswordEntries]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Retrieve password entries
CREATE PROCEDURE [dbo].[GetPasswordEntries]
    @Id INT = NULL -- Optional parameter: If NULL, retrieve all entries; otherwise, retrieve the specific entry
AS
BEGIN
    SET NOCOUNT ON;

    IF @Id IS NULL
    BEGIN
        -- Retrieve all entries
        SELECT Id, Website, Username, Password, CreatedOn, UpdatedOn
        FROM PasswordEntries;
    END
    ELSE
    BEGIN
        -- Retrieve a specific entry
        SELECT Id, Website, Username, Password, CreatedOn, UpdatedOn
        FROM PasswordEntries
        WHERE Id = @Id;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdatePasswordEntry]    Script Date: 11/26/2024 1:16:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- Update an existing password entry
CREATE PROCEDURE [dbo].[UpdatePasswordEntry]
    @Id INT,
    @Website NVARCHAR(MAX),
    @Username NVARCHAR(MAX),
    @Password NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE PasswordEntries
    SET
        Website = @Website,
        Username = @Username,
        Password = @Password,
        UpdatedOn = GETDATE()
    WHERE Id = @Id;
END;
GO
