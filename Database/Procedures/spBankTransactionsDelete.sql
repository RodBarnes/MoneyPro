CREATE PROCEDURE [dbo].[spBankTransactionsDelete]
	@accountId int
AS
	DELETE FROM BankTransaction 
	WHERE AccountId = @accountId

RETURN 0
