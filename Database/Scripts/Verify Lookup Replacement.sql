USE MoneyPro

-- Categories in use
select x.[Text] as Category, s.CategoryId, count(s.SubtransactionId) as Total
from Subtransaction s
join Category x on x.CategoryId = s.CategoryId
group by x.[Text], s.CategoryId
order by x.[Text]
-- Category swapped
select s.CategoryId, c.[Text], count(*) as Cnt
from Subtransaction s join Category c on c.CategoryId = s.CategoryId
where s.CategoryId in (33,65) group by s.CategoryId, c.[Text]
select * from Category where CategoryId in (33,65)
select s.TransactionId, s.SubtransactionId, CategoryId from Subtransaction s where s.CategoryId in (33,65)

-- Subcategories in use
select x.[Text] as Subcategory, s.SubcategoryId, count(s.SubtransactionId) as Total
from Subtransaction s
join Subcategory x on x.SubcategoryId = s.SubcategoryId
group by x.[Text], s.SubcategoryId
order by x.[Text]
-- Subcategory swapped
select s.SubcategoryId, c.[Text], count(*) as Cnt from Subtransaction s join Subcategory c on c.SubcategoryId = s.SubcategoryId
where s.SubcategoryId in (32,38) group by s.SubcategoryId, c.[Text]
select * from Subcategory where SubcategoryId in (32,38)
select s.TransactionId, s.SubtransactionId, SubcategoryId from Subtransaction s where s.SubcategoryId in (32,38)

-- Classes in use
select x.[Text] as Class, s.ClassId, count(s.SubtransactionId) as Total
from Subtransaction s
join Class x on x.ClassId = s.ClassId
group by x.[Text], s.ClassId
order by x.[Text]
-- Classes wapped 
select s.ClassId, c.[Text], count(*) as Cnt from Subtransaction s join Class c on c.ClassId = s.ClassId
where s.ClassId in (12,14) group by s.ClassId, c.[Text]
select * from Class where ClassId in (12,14)
select s.TransactionId, s.SubtransactionId, ClassId from Subtransaction s where s.ClassId in (12,14)

-- Subclasses in use
select x.[Text] as Sublass, s.SubclassId, count(s.SubtransactionId) as Total
from Subtransaction s
join Subclass x on x.SubclassId = s.SubclassId
group by x.[Text], s.SubclassId
order by x.[Text]
-- Subclasses swapped
select s.SubclassId, c.[Text], count(*) as Cnt from Subtransaction s join Subclass c on c.SubclassId = s.SubclassId
where s.SubclassId in (9,13) group by s.SubclassId, c.[Text]
select * from Subclass where SubclassId in (9,13)
select s.TransactionId, s.SubtransactionId, SubclassId from Subtransaction s where s.SubclassId in (9,13)

-- Payees in use
select t.Payee, count(t.Payee) as Total, x.[Name]
from BankTransaction t
left join Payee x on x.[Name] = t.Payee
group by t.Payee, x.[Name]
order by t.Payee

