CREATE PROCEDURE [dbo].[spAccountTypeRead]
	@accountTypeId INT
AS
	SELECT
		ImportName, 
		DisplayName 
	FROM AccountType 
	WHERE AccountTypeId = @accountTypeId

RETURN 0
