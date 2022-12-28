CREATE PROCEDURE [dbo].[spSubtransactionsDeleteForTransaction]
	@transactionId INT
AS
	DELETE FROM Subtransaction 
	WHERE TransactionId = @transactionId

RETURN 0
