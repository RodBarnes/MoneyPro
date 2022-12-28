CREATE PROCEDURE [dbo].[spPayeesRead]
AS
	SELECT
		PayeeId,
		[Name], 
		DateLastUsed
	FROM Payee 
	ORDER BY [Name]

RETURN 0
