CREATE PROCEDURE [dbo].[spScheduleTransactionUpdate]
	@scheduleTransactionId INT,
	@rule NVARCHAR(50),
	@account NVARCHAR(50),
	@payee NVARCHAR(50),
	@memo NVARCHAR(250),
	@nextDate DATETIME2(7),
	@countEnd INT = NULL,
	@dateEnd DATETIME2 = NULL,
	@enterDaysBefore INT = NULL
AS
    DECLARE @payeeId INT, @ruleId INT, @accountId INT
	DECLARE @payees TABLE (Id INT)

	IF @payee = '' OR @payee IS NULL
    BEGIN
        SET @payeeId = 0
    END
    ELSE
    BEGIN
        SELECT @payeeId = PayeeId FROM Payee WHERE [Name] = @payee
        IF @payeeId IS NULL
        BEGIN
            INSERT INTO Payee ([Name]) OUTPUT Inserted.PayeeId INTO @payees(Id) VALUES (@payee)
            SELECT @payeeId = Id FROM @payees
        END
    END

    SELECT @ruleId = ScheduleRuleId FROM ScheduleRule WHERE [Name] = @rule
	IF @ruleId IS NULL SET @ruleId = 0

	SELECT @accountId = AccountId FROM Account WHERE [Name] = @account
	IF @accountId IS NULL SET @accountId = 0

	UPDATE ScheduleTransaction SET
		RuleId = @ruleId,
		AccountId = @accountId, 
		PayeeId = @payeeId, 
		Memo = @memo, 
		NextDate = @nextDate, 
		CountEnd = @countEnd, 
		DateEnd = @dateEnd,
		EnterDaysBefore = @enterDaysBefore
	WHERE ScheduleTransactionId = @scheduleTransactionId

RETURN 0
