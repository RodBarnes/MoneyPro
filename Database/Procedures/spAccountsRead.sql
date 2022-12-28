CREATE PROCEDURE [dbo].[spAccountsRead]
	@status INT = 0
AS
	SELECT
		a.AccountId, 
		a.AccountTypeId, 
		i.[Name] AS Institution, 
		a.[Name], 
		a.Number, 
		a.StartingBalance, 
		a.[Status]
	FROM Account a
		LEFT JOIN Institution i ON i.InstitutionId = a.InstitutionId
	WHERE a.[Status] <= @status

RETURN 0
