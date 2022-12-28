CREATE TABLE [dbo].[Investment]
(
	[Ticker] NVARCHAR(10) NOT NULL PRIMARY KEY, 
    [Company] NVARCHAR(50) NOT NULL, 
    [Source] NVARCHAR(100) NULL
)
