CREATE PROCEDURE [dbo].[spScheduleSubtransactionInsert]
	@scheduleTransactionId INT,
    @category NVARCHAR(50),
    @subcategory NVARCHAR(50),
    @class NVARCHAR(50),
    @subclass NVARCHAR(50),
    @memo NVARCHAR(250),
    @budget NVARCHAR(50),
    @amount MONEY
AS
    DECLARE @categoryId INT, 
            @subcategoryId INT, 
            @classId INT, 
            @subclassId INT, 
            @budgetId INT
    DECLARE @categories TABLE (Id INT)
    DECLARE @subcategories TABLE (Id INT)
    DECLARE @classes TABLE (Id INT)
    DECLARE @subclasses TABLE (Id INT)
    DECLARE @budgets TABLE (Id INT)

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

    IF @budget = '' OR @budget IS NULL
    BEGIN
        SET @budgetId = 0
    END
    ELSE
    BEGIN
        INSERT INTO Budget ([Name], Amount) OUTPUT Inserted.BudgetId INTO @budgets(Id) VALUES (@budget, 0)
        SELECT @budgetId = BudgetId FROM Budget WHERE [Name] = @budget
    END

    INSERT INTO ScheduleSubtransaction(
        ScheduleTransactionId, 
        Memo, 
        CategoryId, 
        SubcategoryId, 
        ClassId, 
        SubclassId, 
        BudgetId, 
        Amount
    )
    OUTPUT Inserted.ScheduleSubtransactionId 
    VALUES(
        @scheduleTransactionId, 
        @memo, 
        @categoryId, 
        @subcategoryId, 
        @classId, 
        @subclassId, 
        @budgetId, 
        @amount
    )

RETURN 0
