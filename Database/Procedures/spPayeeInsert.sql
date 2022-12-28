CREATE PROCEDURE [dbo].[spPayeeInsert]
	@name NVARCHAR(50),
	@date DATETIME2(7) = NULL
AS
	IF @date IS NULL
		SET @date = GETDATE()

	INSERT INTO Payee (
		[Name],
		DateLastUsed
	)
	OUTPUT Inserted.PayeeId
	VALUES (
		@name,
		@date
	)
RETURN 0
