CREATE PROCEDURE [dbo].[spInstitutionDelete]
	@institutionId INT
AS
	DELETE FROM Institution 
	WHERE InstitutionId = @institutionId

RETURN 0
