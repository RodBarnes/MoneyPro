CREATE PROCEDURE [dbo].[spClassUpdate]
	@classId INT,
	@text NVARCHAR(50)
AS
	UPDATE Class
	SET [Text] = @text
	WHERE ClassId = @classId

RETURN 0
