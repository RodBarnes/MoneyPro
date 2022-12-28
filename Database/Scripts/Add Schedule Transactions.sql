--This is a utility script to populate all of the scheduled transactions
--It will be run once in production but may be used multiple times in development

select * from ScheduleRule

--declare @accountId int, @transactionId int
--select @accountId = 1, @transactionId = 75
--select a.[Name], s.SubtransactionId as SubId, b.TransactionId as TrnId,
--    s.XferSubtransactionId as XferSubId, s.XferTransactionId as XferTrnId, xa.[Name] as XferAccount,
--    b.[Date] as TDate, p.[Name] as Payee, b.Memo as TMemo,
--    s.Memo as SMemo, s.Amount as SAmount,
--    ct.[Text] as Category, sc.[Text] as Subcategory,
--    cl.[Text] as Class, scl.[Text] as Subclass
--from BankTransaction b
-- join Account a on a.AccountId = b.AccountId
-- left join Payee p on p.PayeeId = b.PayeeId
-- left join Subtransaction s on s.TransactionId = b.TransactionId
-- left join Account xa on xa.AccountId = s.XferAccountId
-- left join Category ct on ct.CategoryId = s.CategoryId
-- left join Subcategory sc on sc.SubcategoryId = s.SubcategoryId
-- left join Class cl on cl.ClassId = s.ClassId
-- left join Subclass scl on scl.SubclassId = s.SubclassId
--where b.AccountId = @accountId and p.[Name] = 'Judi'

exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'Chase (Amazon)'
GO
exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'PenFed'
GO
exec spAddScheduleTransaction 'Monthly', 'Credit Card - Chase (Southwest)', 'Pend Oreille PUD'
GO
exec spAddScheduleTransaction 'Monthly', 'Credit Card - Chase (Southwest)', 'Verizon Wireless'
GO
exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'Judi'
GO
exec spAddScheduleTransaction 'Monthly', 'Credit Card - Chase (Southwest)', 'Molina Healthcare'
GO
exec spAddScheduleTransaction 'Monthly', 'Credit Card - Chase (Southwest)', 'Netflix'
GO
exec spAddScheduleTransaction 'Monthly', 'Credit Card - Chase (Southwest)', 'iFiber'
GO
exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'PenFed Auto'
GO
exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'Friends of KSPS'
GO
exec spAddScheduleTransaction 'Monthly', 'Checking - US Bank', 'Chase (Southwest)'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'LastPass'
GO
exec spAddScheduleTransaction 'Twice a year', 'Checking - US Bank', 'Pend Oreille County'
GO
exec spAddScheduleTransaction 'Monthly', 'PayPal', 'Schedules Direct'
GO
exec spAddScheduleTransaction 'Yearly', 'PayPal', 'Conde Nast'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'Amica Insurance'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'AARP'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'CBS All Access'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'Google Play'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Amazon)', 'AmazonPrime Membership'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'Markel Insurance'
GO
exec spAddScheduleTransaction 'Yearly', 'Credit Card - Chase (Southwest)', 'Costco'
GO
exec spAddScheduleTransaction 'Every other year', 'Credit Card - Chase (Southwest)', 'CityServiceValcon'
GO

select * from ScheduleTransaction
select * from ScheduleSubtransaction

/*
truncate table ScheduleTransaction
truncate table ScheduleSubtransaction
*/

--SELECT 'Weekly', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Weekly'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Weekly'))
--SELECT 'Every other week', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every other week'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every other week'))
--SELECT 'Twice a month', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Twice a month'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Twice a month'))
--SELECT 'Every four weeks', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every four weeks'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every four weeks'))
--SELECT 'Monthly', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Monthly'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Monthly'))
--SELECT 'Every other month', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every other month'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every other month'))
--SELECT 'Every three months', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every three months'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every three months'))
--SELECT 'Every four months', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every four months'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every four months'))
--SELECT 'Twice a year', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Twice a year'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Twice a year'))
--SELECT 'Yearly', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Yearly'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Yearly'))
--SELECT 'Every other Year', GETDATE(), DATENAME(weekday, GETDATE()), DBO.fnGetNextScheduleDate(GETDATE(),'Every other Year'), DATENAME(weekday, DBO.fnGetNextScheduleDate(GETDATE(),'Every other Year'))

