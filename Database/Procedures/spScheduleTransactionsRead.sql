CREATE PROCEDURE [dbo].[spScheduleTransactionsRead]
AS
	SELECT 
		t.ScheduleTransactionId, 
		r.[Name] AS [Rule], 
		a.[Name] AS Account,
		p.[Name] AS Payee, 
		t.Memo, 
		t.NextDate, 
		t.CountEnd, 
		t.DateEnd, 
		t.EnterDaysBefore, 
		(CASE WHEN SUM(s.Amount) IS NULL THEN 0 ELSE SUM(s.Amount) END) AS Amount
	FROM ScheduleTransaction t
		LEFT JOIN ScheduleRule r ON r.ScheduleRuleId = t.RuleId
		LEFT JOIN Account a ON a.AccountId = t.AccountId
		LEFT JOIN ScheduleSubtransaction s ON s.ScheduleTransactionId = t.ScheduleTransactionId
		LEFT JOIN Payee p ON p.PayeeId = t.PayeeId
	GROUP BY
		t.ScheduleTransactionId, 
		r.[Name], 
		a.[Name],
		p.[Name], 
		t.Memo, 
		t.NextDate, 
		t.CountEnd, 
		t.DateEnd,
		t.EnterDaysBefore
	ORDER BY
		t.NextDate, 
		t.ScheduleTransactionId

RETURN 0
