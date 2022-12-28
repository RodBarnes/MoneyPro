CREATE TABLE [dbo].[Subtransaction]
(
    [SubtransactionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [TransactionId] INT NOT NULL, 
    [BudgetId] INT NULL, 
    [Amount] MONEY NOT NULL, 
    [CategoryId] INT NULL,
    [SubcategoryId] INT NULL,
    [ClassId] INT NULL,
    [SubclassId] INT NULL,
    [Memo] NVARCHAR(250) NULL,
    [XferSubtransactionId] INT NULL DEFAULT 0, 
    [XferTransactionId] INT NULL DEFAULT 0, 
    [XferAccountId] INT NULL DEFAULT 0
)
