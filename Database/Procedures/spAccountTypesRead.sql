CREATE PROCEDURE [dbo].[spAccountTypesRead]
AS
	SELECT
		AccountTypeId, 
		ImportName, 
		DisplayName 
	FROM AccountType 
	ORDER BY DisplayName

RETURN 0
