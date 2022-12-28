CREATE PROCEDURE [dbo].[spSubtransactionsRead]
	@transactionId INT = NULL
AS
    SELECT 
        s.SubtransactionId, 
        s.Amount, 
        s.Memo, 
        cg.[Text] AS Category, 
        sg.[Text] AS Subcategory, 
        cl.[Text] AS Class, 
        sl.[Text] AS Subclass, 
        ax.[Name] AS XferAccount, 
        s.XferSubtransactionId, 
        s.XferTransactionId, 
        b.[Name] AS Budget 
    FROM Subtransaction s
        LEFT JOIN Account ax ON ax.AccountId = s.XferAccountId 
        LEFT JOIN Budget b ON b.BudgetId = s.BudgetId 
        LEFT JOIN Category cg on cg.CategoryId = s.CategoryId 
        LEFT JOIN Subcategory sg on sg.SubcategoryId = s.SubcategoryId 
        LEFT JOIN Class cl on cl.ClassId = s.ClassId 
        LEFT JOIN Subclass sl on sl.SubclassId = s.SubclassId 
    WHERE (@transactionId IS NULL OR TransactionId = @transactionId)
    ORDER BY TransactionId, SubtransactionId

RETURN 0
