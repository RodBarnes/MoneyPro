CREATE PROCEDURE [dbo].[spSubcategoryInsert]
	@text NVARCHAR(50),
	@tax BIT
AS
	INSERT INTO Subcategory (
		[Text],
		Tax
	)
	OUTPUT Inserted.SubcategoryId
	VALUES (
		@text,
		@tax
	)

RETURN 0
