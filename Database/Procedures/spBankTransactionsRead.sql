CREATE PROCEDURE [dbo].[spBankTransactionsRead]
	@accountId INT = NULL,
	@status NCHAR = 'C'
AS
	SELECT 
		t.TransactionId, 
		t.[Date], 
		t.Memo, 
		t.[Status], 
		p.[Name] AS Payee, 
		t.Reference, 
		t.Void, 
		(CASE WHEN SUM(s.Amount) IS NULL THEN 0 ELSE SUM(s.Amount) END) AS Amount
	FROM BankTransaction t
		LEFT JOIN Subtransaction s ON s.TransactionId = t.TransactionId
		LEFT JOIN Payee p ON p.PayeeId = t.PayeeId
	WHERE (@accountId IS NULL OR t.AccountId = @accountId)
	 AND (t.[Status] = @status OR t.[Status] IN ('N','C'))
	GROUP BY
		t.TransactionId, 
		t.[Date], 
		t.Memo, 
		t.[Status], 
		p.[Name], 
		t.Reference,
		t.Void
	ORDER BY
		t.[Date], 
		t.TransactionId

RETURN 0
