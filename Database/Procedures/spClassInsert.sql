CREATE PROCEDURE [dbo].[spClassInsert]
	@text NVARCHAR(50)
AS
	INSERT INTO Class (
		[Text]
	)
	OUTPUT Inserted.ClassId
	VALUES (
		@text
	)

RETURN 0
