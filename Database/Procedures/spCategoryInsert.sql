CREATE PROCEDURE [dbo].[spCategoryInsert]
	@text NVARCHAR(50),
	@tax BIT
AS
	INSERT INTO Category (
		[Text],
		Tax
	)
	OUTPUT Inserted.CategoryId
	VALUES (
		@text,
		@tax
	)

RETURN 0
