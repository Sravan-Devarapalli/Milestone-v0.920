-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-23
-- =============================================
CREATE PROCEDURE ExpensesClone
	@OldMilestoneId INT, 
	@NewMilestoneId INT
AS
BEGIN
	SET NOCOUNT ON;

	INSERT INTO [dbo].[ProjectExpense]
			   ([Name]
			   ,[Amount]
			   ,[Reimbursement]
			   ,[MilestoneId])
	 select Name, Amount, Reimbursement, @NewMilestoneId
	 from ProjectExpense
	 where MilestoneId = @OldMilestoneId
END

