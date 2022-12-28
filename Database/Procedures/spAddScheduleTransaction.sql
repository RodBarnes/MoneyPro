CREATE PROCEDURE [dbo].[spAddScheduleTransaction]
	@ruleName NVARCHAR(50),
	@accountName NVARCHAR(50),
	@payeeName NVARCHAR(50)
AS
	DECLARE
		@accountId INT, 
		@ruleId INT, 
		@nextDate DATETIME2, 
		@lastTransactionId INT, 
		@payeeId INT, 
		@memo NVARCHAR(250)

	DECLARE @idtable TABLE (Id INT, PayeeId INT)

	SELECT @accountId = AccountId 
		FROM Account 
		WHERE [Name] = @accountName

	SELECT @payeeId = PayeeId 
		FROM Payee 
		WHERE [Name] = @payeeName

	SELECT @lastTransactionId = MAX(TransactionId), @nextDate = dbo.fnGetNextScheduleDate(MAX([Date]), @ruleName)
		FROM BankTransaction
		WHERE PayeeId = @payeeId AND AccountId = @accountId

	IF @accountId > 0 AND @payeeId > 0 AND @lastTransactionId > 0
	BEGIN
		SELECT @ruleId = ScheduleRuleId 
			FROM ScheduleRule 
			WHERE [Name] = @ruleName

		SELECT @memo = Memo 
			FROM BankTransaction 
			WHERE TransactionId = @lastTransactionId

		INSERT INTO ScheduleTransaction ([RuleId], [NextDate], [AccountId], [PayeeId], [Memo], [DateEnd], [CountEnd], [EnterDaysBefore])
			OUTPUT Inserted.ScheduleTransactionId, @payeeId INTO @idtable(Id, PayeeId)
			VALUES (@ruleId, @nextDate, @accountId, @payeeId, @memo, NULL, NULL, NULL)

		INSERT INTO ScheduleSubtransaction (ScheduleTransactionId, Amount, BudgetId, CategoryId, SubcategoryId, ClassId, SubclassId, Memo)
		SELECT (SELECT Id FROM @idtable WHERE PayeeId = @payeeId), Amount, BudgetId, CategoryId, SubcategoryId, ClassId, SubclassId, Memo 
			FROM Subtransaction WHERE TransactionId = @lastTransactionId
	END
	SELECT @accountId = NULL, @payeeId = NULL, @lastTransactionId = NULL

RETURN 0
