USE MoneyPro

-- Count of all Category in use
select u.[Text], u.CategoryId as ID, count(s.SubtransactionId) as Cnt
from Category u
 left join Subtransaction s on s.CategoryId = u.CategoryId
group by u.[Text], u.CategoryId
order by u.[Text]

-- Count of all Subcategory in use
select u.[Text], u.SubcategoryId as ID, count(s.SubtransactionId) as Cnt
from Subcategory u
 left join Subtransaction s on s.SubcategoryId = u.SubcategoryId
group by u.[Text], u.SubcategoryId
order by u.[Text]

-- Count of all Class in use
select u.[Text], u.ClassId as ID, count(s.SubtransactionId) as Cnt
from Class u
 left join Subtransaction s on s.ClassId = u.ClassId
group by u.[Text], u.ClassId
order by u.[Text]

-- Count of all Subclass in use
select u.[Text], u.SubclassId as ID, count(s.SubtransactionId) as Cnt
from Subclass u
 left join Subtransaction s on s.SubclassId = u.SubclassId
group by u.[Text], u.SubclassId
order by u.[Text]

-- Count of all Payee in use
select u.[Name], count(t.TransactionId) as Cnt
from Payee u
 left join BankTransaction t on t.Payee = u.[Name]
group by u.[Name]
order by u.[Name]


