CREATE PROCEDURE [dbo].[spSubcategoriesRead]
AS
	SELECT
		SubcategoryId, 
		[Text],
		Tax
	FROM Subcategory
	ORDER BY [Text]

RETURN 0
