CREATE PROCEDURE [dbo].[spScheduleTransactionDelete]
	@scheduleTransactionId int
AS
	DELETE FROM ScheduleTransaction
	WHERE ScheduleTransactionId = @scheduleTransactionId

RETURN 0
