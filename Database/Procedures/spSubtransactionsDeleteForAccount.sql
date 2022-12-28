CREATE PROCEDURE [dbo].[spSubtransactionsDeleteForAccount]
	@accountId INT
AS
    DELETE Subtransaction FROM Subtransaction 
    JOIN BankTransaction ON BankTransaction.TransactionId = Subtransaction.TransactionId 
    WHERE BankTransaction.AccountId = @accountId

RETURN 0
