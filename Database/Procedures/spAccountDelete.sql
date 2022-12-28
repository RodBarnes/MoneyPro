CREATE PROCEDURE [dbo].[spAccountDelete]
	@accountId INT
AS
    DELETE FROM Account 
	WHERE AccountId = @accountId

RETURN 0
