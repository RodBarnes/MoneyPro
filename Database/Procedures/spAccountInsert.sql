CREATE PROCEDURE [dbo].[spAccountInsert]
	@accountTypeId INT, 
	@name NVARCHAR(50),
	@institution NVARCHAR(100), 
	@number NVARCHAR(50), 
	@balance MONEY, 
	@status INT
AS
	DECLARE @institutionId INT
	DECLARE @institutions TABLE (Id INT)

	IF @institution = ''
	BEGIN
		SET @institutionId = 0
	END
	ELSE
	BEGIN
		SELECT @institutionId = InstitutionId FROM Institution WHERE [Name] = @institution
		IF @institutionId IS NULL
		BEGIN
			INSERT INTO Institution ([Name]) OUTPUT Inserted.InstitutionId INTO @institutions(Id) VALUES (@institution)
			SELECT @institutionId = Id FROM @institutions
		END
	END

	INSERT INTO Account(
		AccountTypeId, 
		[Name], 
		InstitutionId, 
		Number, 
		StartingBalance, 
		[Status]
	)
    OUTPUT Inserted.AccountId
    VALUES(
		@accountTypeId, 
		@name,
		@institutionId, 
		@number, 
		@balance, 
		@status
	)

RETURN 0
