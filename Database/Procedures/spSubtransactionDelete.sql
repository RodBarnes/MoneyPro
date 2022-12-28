CREATE PROCEDURE [dbo].[spSubtransactionDelete]
	@subtransactionId INT
AS
	DELETE FROM Subtransaction 
	WHERE SubtransactionId = @subtransactionId

RETURN 0
