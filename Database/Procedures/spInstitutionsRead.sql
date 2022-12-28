CREATE PROCEDURE [dbo].[spInstitutionsRead]
AS
	SELECT
		InstitutionId, 
		[Name], 
		[URL], 
		Email, 
		Phone, 
		Street, 
		City, 
		[State], 
		Zip 
	FROM Institution 
	ORDER BY [Name]

RETURN 0
