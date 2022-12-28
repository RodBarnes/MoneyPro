CREATE TABLE [dbo].[AccountType] (
    [AccountTypeId] INT           IDENTITY (1, 1) NOT NULL,
    [ImportName]          NVARCHAR (20) NOT NULL,
    [DisplayName]   NVARCHAR (50) NULL,
    PRIMARY KEY CLUSTERED ([AccountTypeId] ASC)
);

