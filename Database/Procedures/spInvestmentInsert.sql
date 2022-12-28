CREATE PROCEDURE [dbo].[spInvestmentInsert]
	@ticker NVARCHAR(100),
	@company NVARCHAR(50),
	@source NVARCHAR(100)
AS
	INSERT INTO Investment(
		Ticker, 
		Company, 
		[Source]
	)
    VALUES(
		@ticker, 
		@company, 
		@source
	)

RETURN 0
