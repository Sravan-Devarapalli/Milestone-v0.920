-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-3-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	9-09-2008
-- Description:	Removes persons-milestones details and association for the specified milestone and person.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonDelete]
(
	@MilestonePersonId   INT
)
AS
	SET NOCOUNT ON
	DECLARE @ErrorMessage NVARCHAR(2048)

	DECLARE @PersonId	INT,
			@StartDate  DATETIME,
			@EndDate	DATETIME,
			@ProjectId  INT

	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
	
	SELECT @PersonId = MP.PersonId, @StartDate = MIN(MPE.StartDate), @EndDate = MAX(MPE.EndDate),@ProjectId = m.ProjectId
	FROM dbo.MilestonePerson MP
	JOIN dbo.MilestonePersonEntry MPE ON MP.MilestonePersonId = MPE.MilestonePersonId
	JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	GROUP BY MP.PersonId,M.ProjectId

	IF EXISTS (SELECT TOP 1 1 FROM dbo.TimeEntries AS te WHERE te.MilestonePersonId = @MilestonePersonId)
	BEGIN
		SELECT @ErrorMessage = [dbo].[GetErrorMessage](70019)
		RAISERROR (@ErrorMessage, 16, 1)
	END
	ELSE IF EXISTS (SELECT 1 FROM dbo.ProjectFeedback WHERE PersonId = @PersonId AND @ProjectId = ProjectId AND FeedbackStatusId = 1 AND ReviewPeriodStartDate <= @EndDate AND @StartDate <= ReviewPeriodEndDate AND ReviewPeriodEndDate >= '20140701')
	BEGIN
	    RAISERROR ('This person cannot be deleted from this milestone because project feedback has been marked as completed.  The person can be deleted from the milestone if the status of the feedback is changed to ''Not Completed'' or ''Canceled''. Please navigate to the ''Project Feedback'' tab for more information to make the necessary adjustments..', 16, 1)
	END
	ELSE
	BEGIN
	    
		--DELETE 
		--FROM dbo.ProjectFeedback 
		--WHERE MilestonePersonId = @MilestonePersonId

		EXEC dbo.MilestonePersonDeleteEntries @MilestonePersonId = @MilestonePersonId
		
	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

		DELETE
		  FROM dbo.MilestonePerson
		 WHERE MilestonePersonId = @MilestonePersonId
	END

