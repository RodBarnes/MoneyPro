CREATE PROCEDURE [dbo].[spPayeesDefault]
AS
	SELECT
		[Name] 
	FROM Payee 
	WHERE [DateLastUsed] IS NULL

RETURN 0
