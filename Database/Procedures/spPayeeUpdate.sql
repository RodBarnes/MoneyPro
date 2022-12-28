CREATE PROCEDURE [dbo].[spPayeeUpdate]
	@payeeId INT,
	@name NVARCHAR(50),
	@date DATETIME2(7) = NULL
AS
	IF @date IS NULL
		SET @date = GETDATE()

    UPDATE Payee SET 
		[Name] = @name,
		DateLastUsed = @date
    WHERE PayeeId = @payeeId

RETURN 0
