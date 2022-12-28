CREATE PROCEDURE [dbo].[spInvTransactionsRead]
	@accountId INT
AS
	SELECT 
		TransactionId, 
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
	FROM InvTransaction 
	WHERE AccountId = @accountId

RETURN 0
