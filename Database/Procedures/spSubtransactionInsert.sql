CREATE PROCEDURE [dbo].[spSubtransactionInsert]
	@transactionId INT,
    @amount MONEY,
    @memo NVARCHAR(250),
    @category NVARCHAR(50),
    @subcategory NVARCHAR(50),
    @class NVARCHAR(50),
    @subclass NVARCHAR(50),
    @xferSubtransactionId INT = 0,
    @xferTransactionId INT = 0,
    @xferAccount NVARCHAR(50) = NULL
AS
    DECLARE @categoryId INT, 
            @subcategoryId INT, 
            @classId INT, 
            @subclassId INT, 
            @xferAccountId INT
    DECLARE @categories TABLE (Id INT)
    DECLARE @subcategories TABLE (Id INT)
    DECLARE @classes TABLE (Id INT)
    DECLARE @subclasses TABLE (Id INT)

    IF @category = '' OR @category IS NULL
    BEGIN
        SET @categoryId = 0
    END
    ELSE
    BEGIN
        SELECT @categoryId = CategoryId FROM Category WHERE [Text] = @category
        IF @categoryId IS NULL
        BEGIN
            INSERT INTO Category ([Text]) OUTPUT Inserted.CategoryId INTO @categories(Id) VALUES (@category)
            SELECT @categoryId = Id FROM @categories
        END
    END

    IF @subcategory = '' OR @subcategory IS NULL
    BEGIN
        SET @subcategoryId = 0
    END
    ELSE
    BEGIN
        SELECT @subcategoryId = SubcategoryId FROM Subcategory WHERE [Text] = @subcategory
        IF @subcategoryId IS NULL
        BEGIN
            INSERT INTO Subcategory ([Text]) OUTPUT Inserted.SubcategoryId INTO @subcategories(Id) VALUES (@subcategory)
            SELECT @subcategoryId = Id FROM @subcategories
        END
    END

    IF @class = '' OR @class IS NULL
    BEGIN
        SET @classId = 0
    END
    ELSE
    BEGIN
        SELECT @classId = ClassId FROM Class WHERE [Text] = @class
        IF @classId IS NULL
        BEGIN
            INSERT INTO Class ([Text]) OUTPUT Inserted.ClassId INTO @classes(Id) VALUES (@class)
            SELECT @classId = Id FROM @classes
        END
    END

    IF @subclass = '' OR @subclass IS NULL
    BEGIN
        SET @subclassId = 0
    END
    ELSE
    BEGIN
        SELECT @subclassId = SubclassId FROM Subclass WHERE [Text] = @subclass
        IF @subclassId IS NULL
        BEGIN
            INSERT INTO Subclass ([Text]) OUTPUT Inserted.SubclassId INTO @subclasses(Id) VALUES (@subclass)
            SELECT @subclassId = Id FROM @subclasses
        END
    END

    IF @xferAccount = '' OR @xferAccount IS NULL
    BEGIN
        SET @xferAccountId = 0
    END
    ELSE
    BEGIN
        SELECT @xferAccountId = AccountId FROM Account WHERE [Name] = @xferAccount
    END

    INSERT INTO Subtransaction(
        TransactionId, 
        Amount, 
        Memo, 
        CategoryId, 
        SubcategoryId, 
        ClassId, 
        SubclassId, 
        XferSubtransactionId, 
        XferTransactionId, 
        XferAccountId
    )
    OUTPUT Inserted.SubtransactionId 
    VALUES(
        @transactionId, 
        @amount, 
        @memo, 
        @categoryId, 
        @subcategoryId, 
        @classId, 
        @subclassId, 
        @xferSubtransactionId, 
        @xferTransactionId, 
        @xferAccountId
    )

RETURN 0
