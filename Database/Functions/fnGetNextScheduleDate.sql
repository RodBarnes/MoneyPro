CREATE FUNCTION [dbo].[fnGetNextScheduleDate]
(
	@date DATETIME2,
	@ruleName NVARCHAR(50)
)
RETURNS DATETIME2
AS
BEGIN
	DECLARE
		@nextDate DATETIME2,
		@number FLOAT,
		@interval NVARCHAR(20)

	SELECT @number = Number, @interval = Interval FROM ScheduleRule WHERE [Name] = @ruleName

	IF @interval = 'day'
	BEGIN
		SET @nextDate = DATEADD(day, @number, @date)
	END
	ELSE IF @interval = 'week'
	BEGIN
		SET @nextDate = DATEADD(week, @number, @date)
	END
	ELSE IF @interval = 'month'
	BEGIN
		IF CAST(@number AS INT) = @number
		BEGIN
			SET @nextDate = DATEADD(month, @number, @date)
		END
		ELSE
		BEGIN
			SET @nextDate = DATEADD(day, DAY(EOMONTH(@date)) * @number, @date)
		END
	END
	ELSE IF @interval = 'year'
	BEGIN
		SET @nextDate = DATEADD(year, @number, @date)
	END

	RETURN @nextDate
END
