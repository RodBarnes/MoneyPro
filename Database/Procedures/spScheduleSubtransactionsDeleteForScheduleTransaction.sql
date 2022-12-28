CREATE PROCEDURE [dbo].[spScheduleSubtransactionsDeleteForScheduleTransaction]
	@scheduleTransactionId INT
AS
	DELETE FROM ScheduleSubtransaction 
	WHERE ScheduleTransactionId = @scheduleTransactionId

RETURN 0
