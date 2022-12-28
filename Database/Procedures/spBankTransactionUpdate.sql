CREATE PROCEDURE [dbo].[spBankTransactionUpdate]
	@transactionId INT,
	@accountId INT,
	@date DATETIME2(7),
	@memo NVARCHAR(250),
	@status NCHAR(1),
	@payee NVARCHAR(50),
	@reference NVARCHAR(50),
	@void BIT
AS
    DECLARE @payeeId INT
	DECLARE @payees TABLE (Id INT)

	IF @date IS NULL
		SET @date = GETDATE()

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

	UPDATE BankTransaction SET
		AccountId = @accountId, 
		[Date] = @date, 
		Memo = @memo, 
		[Status] = @status, 
		PayeeId = @payeeId, 
		Reference = @reference,
		Void = @void
	WHERE TransactionId = @transactionId

RETURN 0
