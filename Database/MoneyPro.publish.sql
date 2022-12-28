﻿/*
Deployment script for D:\SOURCE\BITBUCKET\MONEYPRO\WPF\MONEYPRO.MDF

This code was generated by a tool.
Changes to this file may cause incorrect behavior and will be lost if
the code is regenerated.
*/

GO
SET ANSI_NULLS, ANSI_PADDING, ANSI_WARNINGS, ARITHABORT, CONCAT_NULL_YIELDS_NULL, QUOTED_IDENTIFIER ON;

SET NUMERIC_ROUNDABORT OFF;


GO
:setvar DatabaseName "D:\SOURCE\BITBUCKET\MONEYPRO\WPF\MONEYPRO.MDF"
:setvar DefaultFilePrefix "D_\SOURCE\BITBUCKET\MONEYPRO\WPF\MONEYPRO.MDF_"
:setvar DefaultDataPath "C:\Users\rodba\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB"
:setvar DefaultLogPath "C:\Users\rodba\AppData\Local\Microsoft\Microsoft SQL Server Local DB\Instances\MSSQLLocalDB"

GO
:on error exit
GO
/*
Detect SQLCMD mode and disable script execution if SQLCMD mode is not supported.
To re-enable the script after enabling SQLCMD mode, execute the following:
SET NOEXEC OFF; 
*/
:setvar __IsSqlCmdEnabled "True"
GO
IF N'$(__IsSqlCmdEnabled)' NOT LIKE N'True'
    BEGIN
        PRINT N'SQLCMD mode must be enabled to successfully execute this script.';
        SET NOEXEC ON;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET AUTO_CLOSE OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
USE [$(DatabaseName)];


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET ANSI_NULLS ON,
                ANSI_PADDING ON,
                ANSI_WARNINGS ON,
                ARITHABORT ON,
                CONCAT_NULL_YIELDS_NULL ON,
                QUOTED_IDENTIFIER ON,
                ANSI_NULL_DEFAULT ON,
                CURSOR_DEFAULT LOCAL,
                AUTO_SHRINK OFF 
            WITH ROLLBACK IMMEDIATE;
    END


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET PAGE_VERIFY NONE,
                DISABLE_BROKER 
            WITH ROLLBACK IMMEDIATE;
    END


GO
ALTER DATABASE [$(DatabaseName)]
    SET TARGET_RECOVERY_TIME = 0 SECONDS 
    WITH ROLLBACK IMMEDIATE;


GO
IF EXISTS (SELECT 1
           FROM   [master].[dbo].[sysdatabases]
           WHERE  [name] = N'$(DatabaseName)')
    BEGIN
        ALTER DATABASE [$(DatabaseName)]
            SET QUERY_STORE (CLEANUP_POLICY = (STALE_QUERY_THRESHOLD_DAYS = 367)) 
            WITH ROLLBACK IMMEDIATE;
    END


GO
PRINT N'Rename refactoring operation with key 3e25a8b3-20ec-4c8b-acea-e399634eb4bd is skipped, element [dbo].[Split].[Id] (SqlSimpleColumn) will not be renamed to Transactionid';


GO
PRINT N'Rename refactoring operation with key 8b08c6bb-4d30-494d-be19-bde0da2fe0d4 is skipped, element [dbo].[Budget].[Amound] (SqlSimpleColumn) will not be renamed to Amount';


GO
PRINT N'Rename refactoring operation with key 2d409f8b-bf3a-4c1f-bd33-ad74cb9cb94b, e966f6bc-ab73-402d-b4a8-9f2bf13f22e9 is skipped, element [dbo].[ScheduledType].[Rule] (SqlSimpleColumn) will not be renamed to ScheduledRuleId';


GO
PRINT N'Rename refactoring operation with key 27bd162b-e8df-407a-a7c5-13eecf55f225 is skipped, element [dbo].[Split].[SplitId] (SqlSimpleColumn) will not be renamed to Id';


GO
PRINT N'Rename refactoring operation with key 836f414f-55ad-48c4-ae7d-f4f38ed1c41a is skipped, element [dbo].[Split].[Memo] (SqlSimpleColumn) will not be renamed to Note';


GO
PRINT N'Rename refactoring operation with key 2bf6610c-5570-451f-b481-87de997e53e8 is skipped, element [dbo].[Account].[Name] (SqlSimpleColumn) will not be renamed to AccountTypeId';


GO
PRINT N'Rename refactoring operation with key 43bc5093-565f-4d52-893c-9c0295b717d6 is skipped, element [dbo].[Transaction].[Note] (SqlSimpleColumn) will not be renamed to Memo';


GO
PRINT N'Creating [dbo].[Account]...';


GO
CREATE TABLE [dbo].[Account] (
    [Id]            INT           NOT NULL,
    [AccountTypeId] INT           NOT NULL,
    [InstitutionId] INT           NOT NULL,
    [Name]          NVARCHAR (50) NOT NULL,
    [Number]        NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[AccountType]...';


GO
CREATE TABLE [dbo].[AccountType] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[BankTransaction]...';


GO
CREATE TABLE [dbo].[BankTransaction] (
    [Id]            INT           NOT NULL,
    [AccountId]     INT           NOT NULL,
    [PayeeId]       INT           NULL,
    [BudgetId]      INT           NULL,
    [TaxCategoryId] INT           NULL,
    [Date]          DATETIME2 (7) NOT NULL,
    [Amount]        MONEY         NOT NULL,
    [Reference]     NVARCHAR (50) NULL,
    [Memo]          NVARCHAR (50) NULL,
    [Cleared]       BIT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Budget]...';


GO
CREATE TABLE [dbo].[Budget] (
    [Id]     INT           NOT NULL,
    [Name]   NVARCHAR (50) NOT NULL,
    [Amount] MONEY         NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Category]...';


GO
CREATE TABLE [dbo].[Category] (
    [Id]   INT        NOT NULL,
    [Text] NCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Institution]...';


GO
CREATE TABLE [dbo].[Institution] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[InvTransaction]...';


GO
CREATE TABLE [dbo].[InvTransaction] (
    [Id]             INT           NOT NULL,
    [AccountId]      INT           NOT NULL,
    [PayeeId]        INT           NULL,
    [BudgetId]       INT           NULL,
    [TaxCategoryId]  INT           NULL,
    [Date]           DATETIME2 (7) NOT NULL,
    [Amount]         MONEY         NOT NULL,
    [Memo]           NVARCHAR (50) NULL,
    [Cleared]        BIT           NULL,
    [Action]         NCHAR (10)    NULL,
    [Security]       NCHAR (50)    NULL,
    [Description]    NCHAR (50)    NULL,
    [Price]          MONEY         NULL,
    [Quantity]       INT           NULL,
    [Commission]     MONEY         NULL,
    [TransferAmount] MONEY         NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Payee]...';


GO
CREATE TABLE [dbo].[Payee] (
    [Id]           INT           NOT NULL,
    [Name]         NVARCHAR (50) NOT NULL,
    [DateLastUsed] DATETIME2 (7) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Scheduled]...';


GO
CREATE TABLE [dbo].[Scheduled] (
    [Id]              INT           NOT NULL,
    [AccountId]       INT           NOT NULL,
    [ScheduledTypeId] INT           NOT NULL,
    [Name]            NVARCHAR (50) NOT NULL,
    [Amount]          MONEY         NOT NULL,
    [DateStart]       DATETIME2 (7) NULL,
    [DateEnd]         DATETIME2 (7) NULL,
    [CountEnd]        INT           NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[ScheduledRule]...';


GO
CREATE TABLE [dbo].[ScheduledRule] (
    [Id]   INT           NOT NULL,
    [Rule] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[ScheduledType]...';


GO
CREATE TABLE [dbo].[ScheduledType] (
    [Id]              INT           NOT NULL,
    [ScheduledRuleId] INT           NOT NULL,
    [Name]            NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Split]...';


GO
CREATE TABLE [dbo].[Split] (
    [Id]            INT           NOT NULL,
    [TransactionId] INT           NOT NULL,
    [BudgetId]      INT           NULL,
    [TaxCategoryId] INT           NULL,
    [Amount]        MONEY         NOT NULL,
    [Note]          NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Tag]...';


GO
CREATE TABLE [dbo].[Tag] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[TaxCategory]...';


GO
CREATE TABLE [dbo].[TaxCategory] (
    [Id]   INT           NOT NULL,
    [Name] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
PRINT N'Creating [dbo].[Transaction-Tag]...';


GO
CREATE TABLE [dbo].[Transaction-Tag] (
    [Id]            INT NOT NULL,
    [TransactionId] INT NOT NULL,
    [TagId]         INT NOT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
-- Refactoring step to update target server with deployed transaction logs

IF OBJECT_ID(N'dbo.__RefactorLog') IS NULL
BEGIN
    CREATE TABLE [dbo].[__RefactorLog] (OperationKey UNIQUEIDENTIFIER NOT NULL PRIMARY KEY)
    EXEC sp_addextendedproperty N'microsoft_database_tools_support', N'refactoring log', N'schema', N'dbo', N'table', N'__RefactorLog'
END
GO
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '8b08c6bb-4d30-494d-be19-bde0da2fe0d4')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('8b08c6bb-4d30-494d-be19-bde0da2fe0d4')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '2d409f8b-bf3a-4c1f-bd33-ad74cb9cb94b')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('2d409f8b-bf3a-4c1f-bd33-ad74cb9cb94b')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '3e25a8b3-20ec-4c8b-acea-e399634eb4bd')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('3e25a8b3-20ec-4c8b-acea-e399634eb4bd')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '27bd162b-e8df-407a-a7c5-13eecf55f225')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('27bd162b-e8df-407a-a7c5-13eecf55f225')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '836f414f-55ad-48c4-ae7d-f4f38ed1c41a')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('836f414f-55ad-48c4-ae7d-f4f38ed1c41a')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = 'e966f6bc-ab73-402d-b4a8-9f2bf13f22e9')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('e966f6bc-ab73-402d-b4a8-9f2bf13f22e9')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '2bf6610c-5570-451f-b481-87de997e53e8')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('2bf6610c-5570-451f-b481-87de997e53e8')
IF NOT EXISTS (SELECT OperationKey FROM [dbo].[__RefactorLog] WHERE OperationKey = '43bc5093-565f-4d52-893c-9c0295b717d6')
INSERT INTO [dbo].[__RefactorLog] (OperationKey) values ('43bc5093-565f-4d52-893c-9c0295b717d6')

GO

GO
PRINT N'Update complete.';


GO
