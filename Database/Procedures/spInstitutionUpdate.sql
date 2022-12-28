CREATE PROCEDURE [dbo].[spInstitutionUpdate]
	@institutionId INT,
	@name NVARCHAR(100), 
	@url NVARCHAR(500), 
	@email NVARCHAR(200), 
	@phone NVARCHAR(20), 
	@street NVARCHAR(100), 
	@city NVARCHAR(100), 
	@state NVARCHAR(10), 
	@zip NVARCHAR(10)
AS
	UPDATE Institution
		SET [Name] = @name,
		[URL] = @url, 
		Email = @email, 
		Phone = @phone, 
		Street = @street, 
		City = @city, 
		[State] = @state, 
		Zip = @zip 
    WHERE InstitutionId = @institutionId

RETURN 0
