CREATE PROCEDURE [dbo].[spSubclassesRead]
AS
	SELECT
		SubclassId, 
		[Text]
	FROM Subclass
	ORDER BY [Text]

RETURN 0
