CREATE TABLE [dbo].[Account]
(
	[AccountId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountTypeId] INT NOT NULL, 
    [InstitutionId] INT NOT NULL, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Number] NVARCHAR(50) NULL, 
    [StartingBalance] MONEY NULL DEFAULT 0, 
    [Status] INT NULL DEFAULT 0 
)
