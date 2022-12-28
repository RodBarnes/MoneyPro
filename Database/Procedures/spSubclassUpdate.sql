CREATE PROCEDURE [dbo].[spSubclassUpdate]
	@subclassId INT,
	@text NVARCHAR(50)
AS
	UPDATE Subclass
	SET [Text] = @text
	WHERE SubclassId = @subclassId

RETURN 0
