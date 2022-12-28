CREATE PROCEDURE [dbo].[spSubclassInsert]
	@text NVARCHAR(50)
AS
	INSERT INTO Subclass (
		[Text]
	)
	OUTPUT Inserted.SubclassId
	VALUES (
		@text
	)

RETURN 0
