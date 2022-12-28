CREATE PROCEDURE [dbo].[spPayeeDelete]
	@payeeId INT
AS
	DELETE FROM Payee
	WHERE PayeeId = @payeeId

RETURN 0
