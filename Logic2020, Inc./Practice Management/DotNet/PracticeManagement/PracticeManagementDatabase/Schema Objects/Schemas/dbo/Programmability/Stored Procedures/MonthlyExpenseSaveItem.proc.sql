-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 7-24-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	8-20-2008
-- Description:	Saves a month expense item
-- =============================================
CREATE PROCEDURE [dbo].[MonthlyExpenseSaveItem]
(
	@Name                NVARCHAR(50),
	@ExpenseCategoryId   INT,
	@ExpenseBasisId      INT,
	@WeekPaidOptionId    INT,
	@Year                INT,
	@Month               INT,
	@Amount              DECIMAL(18, 3),
	@OLD_Name            NVARCHAR(50)
)
AS
	SET NOCOUNT ON
	SET XACT_ABORT ON
	DECLARE @ErrorMessage NVARCHAR(2048)
	BEGIN TRAN

	IF NOT EXISTS (SELECT 1 FROM dbo.ExpenseCategory AS ec WHERE ec.ExpenseCategoryId = @ExpenseCategoryId)
	BEGIN
		ROLLBACK
		SELECT  @ErrorMessage = [dbo].[GetErrorMessage](70012)
		RAISERROR (@ErrorMessage, 16, 1)
		RETURN
	END

	UPDATE dbo.MonthlyExpense
	   SET ExpenseCategoryId = @ExpenseCategoryId,
	       ExpenseBasisId = @ExpenseBasisId,
	       WeekPaidOptionId = @WeekPaidOptionId,
	       Name = @Name
	 WHERE Name = @OLD_Name

	IF EXISTS(SELECT 1
	            FROM dbo.MonthlyExpense AS me
	           WHERE Name = @Name AND [Year] = @Year AND [Month] = @Month)
	BEGIN
		UPDATE dbo.MonthlyExpense
		   SET Amount = @Amount
		 WHERE Name = @OLD_Name AND [Year] = @Year AND [Month] = @Month
	END
	ELSE
	BEGIN
		INSERT INTO dbo.MonthlyExpense
		            (ExpenseCategoryId, ExpenseBasisId, WeekPaidOptionId, Name, [Year], [Month], Amount)
		     VALUES (@ExpenseCategoryId, @ExpenseBasisId, @WeekPaidOptionId, @Name, @Year, @Month, @Amount)
	END

	COMMIT TRAN

