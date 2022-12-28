CREATE PROCEDURE [dbo].[spSubclassDelete]
	@subclassId INT
AS
	DELETE FROM Subclass 
	WHERE SubclassId = @subclassId

RETURN 0
