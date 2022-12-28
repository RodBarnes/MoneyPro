CREATE PROCEDURE [dbo].[spScheduleSubtransactionsRead]
	@scheduleTransactionId INT = 0
AS
    SELECT 
        s.ScheduleSubtransactionId, 
        s.ScheduleTransactionId, 
        s.Amount, 
        s.Memo, 
        cg.[Text] AS Category, 
        sg.[Text] AS Subcategory, 
        cl.[Text] AS Class, 
        sl.[Text] AS Subclass, 
        b.[Name] AS Budget 
    FROM ScheduleSubtransaction s
        LEFT JOIN Budget b ON b.BudgetId = s.BudgetId 
        LEFT JOIN Category cg on cg.CategoryId = s.CategoryId 
        LEFT JOIN Subcategory sg on sg.SubcategoryId = s.SubcategoryId 
        LEFT JOIN Class cl on cl.ClassId = s.ClassId 
        LEFT JOIN Subclass sl on sl.SubclassId = s.SubclassId 
    WHERE (@scheduleTransactionId = 0) OR (ScheduleTransactionId = @scheduleTransactionId)
    ORDER BY ScheduleTransactionId, ScheduleSubtransactionId

RETURN 0
