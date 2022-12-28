CREATE PROCEDURE [dbo].[spSubcategoryDelete]
	@subcategoryId INT
AS
	DELETE FROM Subcategory 
	WHERE SubcategoryId = @subcategoryId

RETURN 0
