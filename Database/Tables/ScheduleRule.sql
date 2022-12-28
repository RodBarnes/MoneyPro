CREATE TABLE [dbo].[ScheduleRule]
(
	[ScheduleRuleId] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] NVARCHAR(50) NOT NULL, 
    [Number] FLOAT NULL, 
    [Interval] NVARCHAR(50) NULL
)
