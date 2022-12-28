CREATE PROCEDURE [dbo].[spBankTransactionDelete]
	@transactionId int
AS
	DELETE FROM BankTransaction
	WHERE TransactionId = @transactionId

RETURN 0
