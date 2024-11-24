CREATE DATABASE SecretStore
GO

USE [SecretStore]
GO
/****** Object:  Table [dbo].[PasswordEntries]    Script Date: 11/23/2024 11:54:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PasswordEntries](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Website] [nvarchar](max) NULL,
	[Username] [nvarchar](max) NULL,
	[Password] [nvarchar](max) NULL,
	[CreateOn] [datetime] NOT NULL,
	[UpdatedOn] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
ALTER TABLE [dbo].[PasswordEntries] ADD  DEFAULT (getdate()) FOR [CreateOn]
GO
ALTER TABLE [dbo].[PasswordEntries] ADD  DEFAULT (getdate()) FOR [UpdatedOn]
GO
ALTER TABLE [dbo].[PasswordEntries]
ADD 
    SysStartTime DATETIME2 GENERATED ALWAYS AS ROW START NOT NULL,
    SysEndTime DATETIME2 GENERATED ALWAYS AS ROW END NOT NULL,
    PERIOD FOR SYSTEM_TIME (SysStartTime, SysEndTime)
GO
ALTER TABLE [dbo].[PasswordEntries]
SET (SYSTEM_VERSIONING = ON (HISTORY_TABLE = [dbo].[PasswordEntriesHistory]))
GO
/****** Object:  StoredProcedure [dbo].[CreatePasswordEntry]    Script Date: 11/23/2024 11:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- Create a new password entry
CREATE PROCEDURE [dbo].[CreatePasswordEntry]
    @Website NVARCHAR(MAX),
    @Username NVARCHAR(MAX),
    @Password NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON;

    INSERT INTO PasswordEntries (Website, Username, Password, CreateOn)
    VALUES (@Website, @Username, @Password, GETDATE());

    -- Return the newly created ID
    SELECT SCOPE_IDENTITY() AS NewId;
END;
GO
/****** Object:  StoredProcedure [dbo].[DeletePasswordEntry]    Script Date: 11/23/2024 11:54:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Delete a password entry
CREATE PROCEDURE [dbo].[DeletePasswordEntry]
    @Id INT
AS
BEGIN
    SET NOCOUNT ON;

    DELETE FROM PasswordEntries
    WHERE Id = @Id;

    -- Return the number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
/****** Object:  StoredProcedure [dbo].[GetPasswordEntries]    Script Date: 11/23/2024 11:54:43 PM ******/
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
        SELECT Id, Website, Username, Password, CreateOn, UpdatedOn
        FROM PasswordEntries;
    END
    ELSE
    BEGIN
        -- Retrieve a specific entry
        SELECT Id, Website, Username, Password, CreateOn, UpdatedOn
        FROM PasswordEntries
        WHERE Id = @Id;
    END
END;
GO
/****** Object:  StoredProcedure [dbo].[UpdatePasswordEntry]    Script Date: 11/23/2024 11:54:43 PM ******/
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

    -- Return the number of rows affected
    SELECT @@ROWCOUNT AS RowsAffected;
END;
GO
