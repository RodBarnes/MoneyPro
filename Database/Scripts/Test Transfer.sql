USE MoneyPro

-- List of subtransactions using Category which hasn't been converted to Transfer
select s.SubtransactionId, s.Amount, b.[Date], c.[Text] as Category
from Subtransaction s
 join BankTransaction b on b.TransactionId = s.TransactionId
 left join Category c on c.CategoryId = s.CategoryId
where SUBSTRING(c.[Text], 2, LEN(c.[Text]) - 2) in ('Emergency Fund','Credit Card - USAA')
--where SUBSTRING(c.[Text], 1, 1) = '[' AND SUBSTRING(c.[Text], LEN(c.[Text]),1) = ']'


-- List all subtransactions that have had transfers resolved
select s.SubtransactionId, s.Amount, b.[Date], b.Memo, a.[Name] as Account,
sx.SubtransactionId, sx.Amount, b.[Date], b.memo, ax.[Name] as XferAccount
from Subtransaction s
 join BankTransaction b on b.TransactionId = s.TransactionId
 join Account a on a.AccountId = b.AccountId
 join Subtransaction sx on sx.SubtransactionId = s.XferSubtransactionId
 join BankTransaction bx on bx.TransactionId = s.XferTransactionId
 join Account ax on ax.AccountId = s.XferAccountId
where s.XferAccountId > 0
