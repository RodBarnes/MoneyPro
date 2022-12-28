CREATE PROCEDURE [dbo].[spScheduleRulesRead]
AS
	SELECT
		ScheduleRuleId,
		[Name], 
		Number, 
		Interval 
	FROM ScheduleRule 
	ORDER BY ScheduleRuleId

RETURN 0
