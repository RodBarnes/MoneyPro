CREATE PROCEDURE [dbo].[spCategoryUpdate]
	@categoryId INT,
	@text NVARCHAR(50),
	@tax BIT
AS
	UPDATE Category
		SET [Text] = @text,
		Tax = @tax
	WHERE CategoryId = @categoryId

RETURN 0
