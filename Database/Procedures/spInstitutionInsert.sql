CREATE PROCEDURE [dbo].[spInstitutionInsert]
	@name NVARCHAR(100), 
	@url NVARCHAR(500), 
	@email NVARCHAR(200), 
	@phone NVARCHAR(20), 
	@street NVARCHAR(100), 
	@city NVARCHAR(100), 
	@state NVARCHAR(10), 
	@zip NVARCHAR(10)
AS
    INSERT INTO Institution (
		[Name], 
		[URL], 
		Email, 
		Phone, 
		Street, 
		City, 
		[State], 
		Zip
	) 
	OUTPUT Inserted.InstitutionId 
	VALUES (
		@name, 
		@url, 
		@email, 
		@phone, 
		@street, 
		@city, 
		@state, 
		@zip
	)

RETURN 0
