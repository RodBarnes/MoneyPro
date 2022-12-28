CREATE PROCEDURE [dbo].[spClassesRead]
AS
	SELECT
		ClassId, 
		[Text]
	FROM Class
	ORDER BY [Text]

RETURN 0
