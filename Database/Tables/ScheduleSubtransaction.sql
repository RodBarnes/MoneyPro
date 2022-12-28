CREATE TABLE [dbo].[ScheduleSubtransaction]
(
	[ScheduleSubtransactionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [ScheduleTransactionId] INT NOT NULL, 
    [BudgetId] INT NULL, 
    [Amount] MONEY NOT NULL, 
    [CategoryId] INT NULL, 
    [SubcategoryId] INT NULL, 
    [ClassId] INT NULL, 
    [SubclassId] INT NULL, 
    [Memo] NVARCHAR(250) NULL
)
