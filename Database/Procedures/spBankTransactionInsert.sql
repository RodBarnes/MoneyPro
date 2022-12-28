CREATE PROCEDURE [dbo].[spBankTransactionInsert]
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
            INSERT INTO Payee ([Name], DateLastUsed) OUTPUT Inserted.PayeeId INTO @payees(Id) VALUES (@payee, GETDATE())
            SELECT @payeeId = Id FROM @payees
        END
    END

	INSERT INTO BankTransaction(
		AccountId, 
		[Date], 
		Memo, 
		[Status], 
		PayeeId, 
		Reference,
		Void
	)
	OUTPUT Inserted.TransactionId
	VALUES(
		@accountId, 
		@date, 
		@memo, 
		@status, 
		@payeeId, 
		@reference,
		@void
	)

RETURN 0
