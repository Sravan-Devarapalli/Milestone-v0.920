-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2009-11-10
-- Description:	Moves just the end of the given milestone
-- =============================================
CREATE PROCEDURE MilestoneMoveEnd 
	@MilestoneId            INT,
	@MilestonePersonId		INT,
	@ShiftDays              INT
AS
BEGIN
	SET NOCOUNT ON;

	-- Update milestone end date
	UPDATE dbo.Milestone
	   SET ProjectedDeliveryDate = DATEADD(dd, @ShiftDays, ProjectedDeliveryDate)
	 WHERE MilestoneId = @MilestoneId
	 
	-- Save updated value to the variable
	DECLARE @NewMilestoneEndDate DATETIME
	
	SELECT @NewMilestoneEndDate = ProjectedDeliveryDate
	FROM dbo.Milestone
	WHERE MilestoneId = @MilestoneId
	
	-- Set milestone person entry end date to that value
	UPDATE mpe
	   SET EndDate = @NewMilestoneEndDate
	  FROM dbo.MilestonePersonEntry AS mpe
	       INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.Milestone AS sh ON mp.MilestoneId = sh.MilestoneId
	 WHERE sh.MilestoneId = @MilestoneId AND mpe.MilestonePersonId = @MilestonePersonId
END

