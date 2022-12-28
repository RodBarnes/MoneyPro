CREATE TABLE [dbo].[ScheduleTransaction]
(
	[ScheduleTransactionId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [RuleId] INT NOT NULL, 
    [AccountId] INT NOT NULL, 
    [PayeeId] INT NOT NULL, 
    [NextDate] DATETIME2 NOT NULL,
    [Memo] NVARCHAR(250) NULL,
    [DateEnd] DATETIME2 NULL, 
    [CountEnd] INT NULL, 
    [EnterDaysBefore] INT NULL,
)
