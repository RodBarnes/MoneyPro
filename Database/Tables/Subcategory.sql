CREATE TABLE [dbo].[Subcategory] (
    [SubcategoryId] INT NOT NULL PRIMARY KEY IDENTITY,
    [Text]       NVARCHAR(50) NOT NULL,
    [Tax]        BIT        NOT NULL DEFAULT 0
)