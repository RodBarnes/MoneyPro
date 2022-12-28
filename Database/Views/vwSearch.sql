CREATE VIEW dbo.vwSearch
AS
    SELECT
        t.Reference,
        t.[Date],
        p.[Name] AS Payee,
        t.Memo AS TransMemo,
        s.Memo AS SubMemo,
        s.Amount,
        cg.[Text] AS Category,
        sg.[Text] AS Subcategory,
        cs.[Text] AS Class,
        ss.[Text] AS Subclass,
        s.SubtransactionId,
        t.TransactionId,
        a.[Name] as Account
    FROM Subtransaction s
     JOIN BankTransaction t ON t.TransactionId = s.TransactionId
     JOIN Account a ON a.AccountId = t.AccountId
     LEFT JOIN Payee p ON p.PayeeId = t.PayeeId
     LEFT JOIN Category cg ON cg.CategoryId = s.CategoryId
     LEFT JOIN Subcategory sg ON sg.SubcategoryId = s.SubcategoryId
     LEFT JOIN Class cs ON cs.ClassId = s.ClassId
     LEFT JOIN Subclass ss ON ss.SubclassId = s.SubclassId
