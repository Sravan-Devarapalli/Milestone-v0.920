CREATE PROCEDURE [dbo].[CanMoveFutureMilestones]
(
	@MilestoneId            INT,
	@ShiftDays              INT 
)
AS
	SET NOCOUNT ON

	IF EXISTS (SELECT 1 FROM Milestone fm
				JOIN Milestone cm ON fm.ProjectId = cm.ProjectId 
				AND fm.StartDate>=cm.StartDate 
				AND fm.MilestoneId<>cm.MilestoneId and cm.MilestoneId = @MilestoneId
				)
	 AND @ShiftDays <0
	 AND EXISTS(SELECT 1 FROM ProjectExpense PE 
				JOIN Project P ON P.ProjectId = PE.ProjectId
				WHERE PE.EndDate> DATEADD(DD,@ShiftDays,P.EndDate)
				)
	BEGIN
		SELECT 'False'
	END 
	ELSE
	BEGIN
		SELECT 'True'
	END
