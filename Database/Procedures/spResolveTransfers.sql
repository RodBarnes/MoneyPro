CREATE PROCEDURE [dbo].[spResolveTransfers]
AS
	/*
	 * These could be made into output parameters if needed.
	 */
	DEClARE
		@step1 INT,
		@step2 INT,
		@step3 INT

	/* Step #1:
	 * Update all subtransactions which have a related Category.Text in brackets and which matches an existing Account.Name,
	 * by using the Subtransaction.Amount and its parent Transaction.Date to find the matching transaction/subtransaction under the
	 * matching Account. Upon finding the match, updated the original subtransaction: set the XferSubtransactionId, XferTransactionId,
	 * and XferAccountId values to those of the matched subtransaction/transaction/account.
	 */
	--SELECT s.SubtransactionId, b.TransactionId, a.AccountId, a.[Name] AS Acct, s.Amount, b.[Date], c.[Text] as Category, xc.[Text] as XferCategory,
	--xa.[Name] AS XferAcct, xs.Amount AS XferAmount, xb.[Date] AS XferDate,
	--xs.SubtransactionId as XferSubtransactionId, xb.TransactionId as XferTransactionId, xa.AccountId as XferAccountId
	UPDATE s SET s.XferSubtransactionId = xs.SubtransactionId, s.XferTransactionId = xb.TransactionId, s.XferAccountId = xa.AccountId
	FROM Subtransaction s
	JOIN Category c ON c.CategoryId = s.CategoryId
	JOIN BankTransaction b ON b.TransactionId = s.TransactionId
	JOIN Account a ON a.AccountId = b.AccountId 
	JOIN Account xa ON xa.[Name] = SUBSTRING(c.[Text], 2, LEN(c.[Text]) - 2)
	JOIN BankTransaction xb ON xb.AccountId = xa.AccountId and xb.[Date] = b.[Date]
	JOIN Subtransaction xs ON xs.TransactionId = xb.TransactionId and xs.Amount = (s.Amount * -1)
	JOIN Category xc on xc.CategoryId = xs.CategoryId
	WHERE s.XferSubtransactionId = 0
	 AND SUBSTRING(c.[Text], 1, 1) = '[' AND SUBSTRING(c.[Text], LEN(c.[Text]),1) = ']'
	 AND SUBSTRING(xc.[Text], 1, 1) = '[' AND SUBSTRING(xc.[Text], LEN(xc.[Text]),1) = ']'

	SET @step1 = @@ROWCOUNT

	 -- If the first step upadated any rows, then need to proceed with the other two steps
	 IF @step1 > 0
	 BEGIN
		/* Step #2
		 * Delete all categories which are related to any Subtransaction which shows an XferSubtransactionId > 0.
		 */
		--SELECT DISTINCT c.CategoryId, c.[Text], SUBSTRING(c.[Text],2,LEN(c.[Text])-2) as TrimText,
		--s.SubtransactionId, s.XferSubtransactionId, s.XferTransactionId, s.XferAccountId
		DELETE Category
		FROM Category c
		JOIN Subtransaction s ON s.CategoryId = c.CategoryId 
		WHERE s.XferSubtransactionId > 0 AND SUBSTRING(c.[Text], 1, 1) = '[' AND SUBSTRING(c.[Text], LEN(c.[Text]),1) = ']'

		SET @step2 = @@ROWCOUNT;

		/* Step #3
		 * Update all subtransactions which show an XferSubtransactionId > 0: set its Category to the "Transfer" Category
		 * and set its SubcategoryId = null
		 */
		--SELECT s.XferSubtransactionId, s.SubtransactionId, s.CategoryId, c.[Text], s.SubcategoryId
		UPDATE s SET s.CategoryId = 1, s.SubcategoryId = 0
		FROM Subtransaction s
		LEFT JOIN Category c ON c.CategoryId = s.CategoryId
		WHERE s.XferSubtransactionId > 0 AND c.[Text] IS NULL

		SET @step3 = @@ROWCOUNT;

	END;


RETURN 0;