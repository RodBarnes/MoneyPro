CREATE TABLE [dbo].[Institution]
(
    [InstitutionId] INT IDENTITY (1, 1) NOT NULL,
    [Name] NVARCHAR (100) NOT NULL,
    [URL] NVARCHAR (500) NULL,
    [Email] NVARCHAR (200) NULL,
    [Phone] NVARCHAR (20) NULL,
    [Street] NVARCHAR(100) NULL, 
    [City] NVARCHAR(100) NULL, 
    [State] NVARCHAR(10) NULL, 
    [Zip] NVARCHAR(10) NULL, 
    PRIMARY KEY CLUSTERED ([InstitutionId] ASC)
)
