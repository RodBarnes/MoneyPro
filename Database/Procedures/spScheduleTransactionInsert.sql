CREATE PROCEDURE [dbo].[spScheduleTransactionInsert]
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
            INSERT INTO Payee ([Name], DateLastUsed) OUTPUT Inserted.PayeeId INTO @payees(Id) VALUES (@payee, GETDATE())
            SELECT @payeeId = Id FROM @payees
        END
    END

    SELECT @ruleId = ScheduleRuleId FROM ScheduleRule WHERE [Name] = @rule
	IF @ruleId IS NULL SET @ruleId = 0

	SELECT @accountId = AccountId FROM Account WHERE [Name] = @account
	IF @accountId IS NULL SET @accountId = 0

	INSERT INTO ScheduleTransaction(
		RuleId,
		AccountId, 
		PayeeId, 
		Memo, 
		NextDate, 
		CountEnd, 
		DateEnd,
		EnterDaysBefore
	)
	OUTPUT Inserted.ScheduleTransactionId
	VALUES(
		@ruleId,
		@accountId, 
		@payeeId, 
		@memo, 
		@nextDate, 
		@countEnd, 
		@dateEnd,
		@enterDaysBefore
	)

RETURN 0
