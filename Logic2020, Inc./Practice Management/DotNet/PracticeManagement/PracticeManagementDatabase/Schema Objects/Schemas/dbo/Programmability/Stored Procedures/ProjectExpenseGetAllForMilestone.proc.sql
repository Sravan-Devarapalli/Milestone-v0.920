-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-04-20
-- =============================================
CREATE PROCEDURE dbo.ProjectExpenseGetAllForMilestone 
	@MilestoneId int
AS
BEGIN
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	SELECT *
	from v_ProjectExpenses 
	where MilestoneId = @MilestoneId
END

