CREATE TABLE [dbo].[InvTransaction]
(
	[TransactionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId] INT NOT NULL, 
    [Date] DATETIME2 NOT NULL, 
    [Memo] NVARCHAR(250) NULL, 
    [Status] NCHAR(1) NOT NULL,
    [Action] NVARCHAR(50) NOT NULL, 
    [Security] NVARCHAR(100) NOT NULL, 
    [Description] NVARCHAR(50) NULL, 
    [Price] MONEY NOT NULL, 
    [Quantity] DECIMAL(10, 4) NOT NULL, 
    [Commission] MONEY NULL, 
    [TransferAmount] MONEY NULL
)
