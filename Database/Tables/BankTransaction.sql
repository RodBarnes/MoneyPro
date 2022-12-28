CREATE TABLE [dbo].[BankTransaction]
(
	[TransactionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [AccountId] INT NOT NULL, 
    [PayeeId] INT NULL, 
    [Date] DATETIME2 NOT NULL, 
    [Reference] NVARCHAR(50) NULL, 
    [Memo] NVARCHAR(250) NULL, 
    [Status] NCHAR(1) NOT NULL DEFAULT 'N', 
    [Void] BIT NULL DEFAULT 0
)
