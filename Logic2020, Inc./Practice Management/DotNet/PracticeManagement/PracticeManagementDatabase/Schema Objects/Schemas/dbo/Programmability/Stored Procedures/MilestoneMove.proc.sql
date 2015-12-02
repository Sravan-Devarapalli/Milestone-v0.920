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

	BEGIN TRY
	BEGIN TRAN  Tran_MilestoneMove

	DECLARE @ProjectNewStartDate	DATETIME,
			@ProjectNewEndDate		DATETIME,
			@ProjectId	INT
    SELECT @ProjectId=ProjectId FROM dbo.Milestone WHERE MilestoneId = @MilestoneId

	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	SELECT P.ProjectId,P.Name AS ProjectName,P.ProjectNumber,mpe.BadgeStartDate,MPE.BadgeEndDate,DATEADD(dd, @ShiftDays, mpe.BadgeStartDate) AS NewBadgeStartDate, DATEADD(dd, @ShiftDays, mpe.BadgeEndDate) AS NewBadgeEndDate,
		   MP.PersonId,Per.LastName,Per.FirstName,mpe.IsBadgeException
	FROM dbo.MilestonePersonEntry AS mpe
		 INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
		 INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
		 INNER JOIN dbo.Milestone AS sh ON m.ProjectId = sh.ProjectId AND m.StartDate > sh.StartDate
		 INNER JOIN dbo.Project P ON P.ProjectId = sh.ProjectId
		 INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	WHERE sh.MilestoneId = @MilestoneId AND @MoveFutureMilestones = 1 AND mpe.IsBadgeRequired = 1 AND P.ProjectStatusId IN (2,3) --Inactive,Completed,active and projected statuses.
	UNION ALL
	SELECT P.ProjectId,P.Name AS ProjectName,P.ProjectNumber,mpe.BadgeStartDate,MPE.BadgeEndDate,DATEADD(dd, @ShiftDays, mpe.BadgeStartDate) AS NewBadgeStartDate, DATEADD(dd, @ShiftDays, mpe.BadgeEndDate) AS NewBadgeEndDate,
		   MP.PersonId,Per.LastName,Per.FirstName,mpe.IsBadgeException
	FROM dbo.MilestonePersonEntry AS mpe
	       INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.Milestone AS sh ON mp.MilestoneId = sh.MilestoneId
		   INNER JOIN dbo.Project P ON P.ProjectId = sh.ProjectId
		 INNER JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	 WHERE sh.MilestoneId = @MilestoneId AND mpe.IsBadgeRequired = 1 AND P.ProjectStatusId IN (2,3) --Inactive,Completed,active and projected statuses.

	IF @MoveFutureMilestones = 1
	BEGIN
		
		UPDATE mpe
		   SET StartDate = DATEADD(dd, @ShiftDays, mpe.StartDate),
		       EndDate = DATEADD(dd, @ShiftDays, mpe.EndDate),
			   BadgeStartDate = CASE WHEN BadgeStartDate IS NULL THEN NULL
								ELSE DATEADD(dd, @ShiftDays, mpe.BadgeStartDate) END,
			   BadgeEndDate = CASE WHEN BadgeEndDate IS NULL THEN NULL
							  ELSE DATEADD(dd, @ShiftDays, mpe.BadgeEndDate) END,
			   IsApproved = CASE WHEN IsApproved IS NULL OR BadgeStartDate IS NULL THEN NULL
							 ELSE 0 END
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
	       EndDate = DATEADD(dd, @ShiftDays, mpe.EndDate),
		   BadgeStartDate = CASE WHEN BadgeStartDate IS NULL THEN NULL
								ELSE DATEADD(dd, @ShiftDays, mpe.BadgeStartDate) END,
		   BadgeEndDate = CASE WHEN BadgeEndDate IS NULL THEN NULL
							  ELSE DATEADD(dd, @ShiftDays, mpe.BadgeEndDate) END,
		   IsApproved = CASE WHEN IsApproved IS NULL OR BadgeStartDate IS NULL THEN NULL
							 ELSE 0 END
	  FROM dbo.MilestonePersonEntry AS mpe
	       INNER JOIN dbo.MilestonePerson AS mp ON mpe.MilestonePersonId = mp.MilestonePersonId
	       INNER JOIN dbo.Milestone AS sh ON mp.MilestoneId = sh.MilestoneId
	 WHERE sh.MilestoneId = @MilestoneId

	 UPDATE dbo.Milestone
	 SET StartDate = DATEADD(dd, @ShiftDays, StartDate),
	     ProjectedDeliveryDate = DATEADD(dd, @ShiftDays, ProjectedDeliveryDate)
	 WHERE MilestoneId = @MilestoneId

	 SELECT @ProjectNewStartDate=MIN(M.StartDate),
	        @ProjectNewEndDate = MAX(M.ProjectedDeliveryDate)
	 FROM dbo.Milestone M 
	 WHERE M.ProjectId = @ProjectId
	 GROUP BY M.ProjectId 

	 UPDATE dbo.ProjectExpense 
	 SET StartDate = CASE WHEN StartDate <= @ProjectNewStartDate THEN @ProjectNewStartDate ELSE StartDate END,
	     EndDate = CASE WHEN EndDate <= @ProjectNewEndDate THEN EndDate ELSE @ProjectNewEndDate END
	 WHERE StartDate <= @ProjectNewEndDate AND @ProjectNewStartDate <= EndDate
	 AND ProjectId = @ProjectId

	 UPDATE dbo.ProjectExpense 
	 SET StartDate = @ProjectNewStartDate,
		 EndDate = @ProjectNewEndDate
	 WHERE StartDate > @ProjectNewEndDate OR @ProjectNewStartDate > EndDate
	 AND ProjectId = @ProjectId

	 IF @MilestoneId IS NOT NULL
	 BEGIN
		EXEC dbo.InsertProjectFeedbackByMilestonePersonId @MilestonePersonId = NULL,@MilestoneId = @MilestoneId
	 END

	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

	COMMIT TRAN Tran_MilestoneMove
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_MilestoneMove
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH

