CREATE PROCEDURE [dbo].[spInvTransactionUpdate]
	@transactionId INT,
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
	UPDATE InvTransaction
		SET [Date] = @date, 
		Memo = @memo, 
		[Status] = @status, 
		[Action] = @action, 
		[Security] = @ticker, 
		[Description] = @desc, 
		Price = @price, 
		Quantity = @qty, 
		Commission = @commission, 
		TransferAmount = @xferAmount 
	WHERE TransactionId = @transactionId

RETURN 0
