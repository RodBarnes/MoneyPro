CREATE PROCEDURE [dbo].[spCategoryDelete]
	@categoryId INT
AS
	DELETE FROM Category 
	WHERE CategoryId = @categoryId

RETURN 0
