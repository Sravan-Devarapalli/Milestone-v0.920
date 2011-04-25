CREATE PROCEDURE dbo.CategoryItemBudgetSave
(
	@ItemId		INT,
	@CategoryTypeId	INT,
	@MonthStartDate	DATETIME,
	@Amount			DECIMAL(18,2)
)
AS
BEGIN
	IF EXISTS (SELECT 1 FROM dbo.CategoryItemBudget
				WHERE ItemId = @ItemId AND CategoryTypeId = @CategoryTypeId
						AND MonthStartDate = @MonthStartDate)
	BEGIN
	 UPDATE dbo.CategoryItemBudget
	 SET Amount = @Amount
	 WHERE ItemId = @ItemId 
			AND CategoryTypeId = @CategoryTypeId
			AND MonthStartDate = @MonthStartDate
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[CategoryItemBudget]
           ([ItemId]
           ,[CategoryTypeId]
           ,[MonthStartDate]
           ,[Amount])
     VALUES
           (@ItemId
           ,@CategoryTypeId
           ,@MonthStartDate
           ,@Amount)
	END
END
