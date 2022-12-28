CREATE PROCEDURE [dbo].[spReferencesRead]
AS
    SELECT DISTINCT 
        Reference 
    FROM BankTransaction 
    WHERE NOT(Reference = '' OR Reference is NULL) 
    ORDER BY Reference

RETURN 0
