CREATE PROCEDURE [dbo].[spClassDelete]
	@classId INT
AS
	DELETE FROM Class 
	WHERE ClassId = @classId

RETURN 0
