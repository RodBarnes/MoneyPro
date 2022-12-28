--exec spScheduleTransactionsRead
--exec spScheduleSubtransactionsRead 3
--exec spScheduleTransactionDelete 2
--exec spBankTransactionsRead 2

--exec spScheduleRulesRead

--exec spScheduleTransactionsRead
--exec spScheduleSubtransactionsRead
/*
delete from ScheduleTransaction
delete from ScheduleSubtransaction
*/

--declare @nextdate datetime, @enddate datetime
--select @nextdate = GETDATE(), @enddate = GETDATE()+1
--exec spScheduleTransactionInsert 99, 99, 'new payee', 'memo', @nextdate
--exec spScheduleTransactionsRead

--declare @nextdate datetime, @enddate datetime
--select @nextdate = GETDATE()-1, @enddate = GETDATE()-2
--exec spScheduleTransactionUpdate 4, 88, 88, 'old payee', 'omem', @nextdate, 88, @enddate, 88
--exec spScheduleTransactionsRead

--exec spScheduleTransactionDelete 2013
--exec spScheduleSubtransactionDelete 1006

/*
exec spScheduleSubtransactionsRead 5

exec spScheduleSubtransactionInsert 5, 'Icategory', 'Isubcategory', 'Iclass', 'Isubclass', 'Imemo', 'Ibudget', 99.99
exec spScheduleSubtransactionsRead 5

exec spScheduleSubtransactionUpdate 4, 5, 'Ucategory', 'Usubcategory', 'Uclass', 'Usubclass', 'Umemo', 'Ubudget', 88.88
exec spScheduleSubtransactionsRead 5

exec spScheduleSubtransactionDelete 4
exec spScheduleSubtransactionsRead 5

exec spScheduleSubtransactionsDeleteForScheduleTransaction 5
exec spScheduleSubtransactionsRead 5
*/

