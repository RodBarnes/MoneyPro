CREATE PROCEDURE [dbo].[spSubcategoryUpdate]
	@subcategoryId INT,
	@text NVARCHAR(50),
	@tax BIT
AS
	UPDATE Subcategory
	SET [Text] = @text, Tax = @tax
	WHERE SubcategoryId = @subcategoryId

RETURN 0
