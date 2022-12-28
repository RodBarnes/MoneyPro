CREATE PROCEDURE [dbo].[spCategoriesRead]
AS
	SELECT
		CategoryId, 
		[Text],
		Tax
	FROM Category
	ORDER BY [Text]

RETURN 0
