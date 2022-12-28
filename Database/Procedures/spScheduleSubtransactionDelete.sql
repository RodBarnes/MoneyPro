CREATE PROCEDURE [dbo].[spScheduleSubtransactionDelete]
	@scheduleSubtransactionId INT
AS
	DELETE FROM ScheduleSubtransaction 
	WHERE ScheduleSubtransactionId = @scheduleSubtransactionId

RETURN 0
