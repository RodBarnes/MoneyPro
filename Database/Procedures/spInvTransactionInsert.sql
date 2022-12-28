CREATE PROCEDURE [dbo].[spInvTransactionInsert]
	@accountId INT, 
	@date DATETIME2(7), 
	@memo NVARCHAR(250), 
	@status NCHAR(1), 
	@action NVARCHAR(50), 
	@ticker NVARCHAR(100), 
	@desc NVARCHAR(50), 
	@price MONEY, 
	@qty DECIMAL(10,4), 
	@commission MONEY, 
	@xferAmount MONEY
AS
	INSERT INTO InvTransaction(
		AccountId, 
		[Date], 
		Memo, 
		[Status], 
		[Action], 
		[Security], 
		[Description], 
		Price, 
		Quantity, 
		Commission, 
		TransferAmount
	) 
	OUTPUT Inserted.TransactionId 
	VALUES(
		@accountId, 
		@date, 
		@memo, 
		@status, 
		@action, 
		@ticker, 
		@desc, 
		@price, 
		@qty, 
		@commission, 
		@xferAmount
	)

RETURN 0
