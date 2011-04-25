-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-23-2008
-- Updated by:  Anatoliy Lokshin
-- Update date: 11-07-2008
-- Description:	Moves the specified milestone and optionally future milestones forward and backward.
-- =============================================
CREATE PROCEDURE [dbo].[MilestoneMove]
(
	@MilestoneId            INT,
	@ShiftDays              INT,
	@MoveFutureMilestones   BIT
)
AS
	SET NOCOUNT ON

	IF @MoveFutureMilestones = 1
	BEGIN
		UPDATE mpe
		   SET StartDate = DATEADD(dd, @ShiftDays, mpe.StartDate),
		       EndDate = DATEADD(dd, @ShiftDays, mpe.EndDate)
		  FROM dbo.MilestonePersonEntry AS mpe
		       INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
		       INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
		       INNER JOIN dbo.Milestone AS sh ON m.ProjectId = sh.ProjectId AND m.StartDate > sh.StartDate
		 WHERE sh.MilestoneId = @MilestoneId

		UPDATE m
		   SET StartDate = DATEADD(dd, @ShiftDays, m.StartDate),
		       ProjectedDeliveryDate = DATEADD(dd, @ShiftDays, m.ProjectedDeliveryDate)
		  FROM dbo.Milestone AS m
		       INNER JOIN dbo.Milestone AS sh ON m.ProjectId = sh.ProjectId AND m.StartDate > sh.StartDate
		 WHERE sh.MilestoneId = @MilestoneId
	END

	UPDATE mpe
	   SET StartDate = DATEADD(dd, @ShiftDays, mpe.StartDate),
	       EndDate = DATEADD(dd, @ShiftDays, mpe.EndDate)
	  FROM dbo.MilestonePersonEntry AS mpe
	       INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.Milestone AS sh ON mp.MilestoneId = sh.MilestoneId
	 WHERE sh.MilestoneId = @MilestoneId

	UPDATE dbo.Milestone
	   SET StartDate = DATEADD(dd, @ShiftDays, StartDate),
	       ProjectedDeliveryDate = DATEADD(dd, @ShiftDays, ProjectedDeliveryDate)
	 WHERE MilestoneId = @MilestoneId

