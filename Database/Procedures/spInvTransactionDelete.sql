CREATE PROCEDURE [dbo].[spInvTransactionDelete]
	@transactionId INT
AS
	DELETE FROM InvTransaction 
	WHERE TransactionId = @transactionId

RETURN 0
