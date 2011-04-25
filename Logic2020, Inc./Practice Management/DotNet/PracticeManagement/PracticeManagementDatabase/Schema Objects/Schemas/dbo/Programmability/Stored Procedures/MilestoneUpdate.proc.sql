-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-22-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	11-27-2008
-- Description:	Updates a Milestone
-- =============================================
CREATE PROCEDURE dbo.MilestoneUpdate
(
	@MilestoneId              INT,
	@ProjectId                INT,
	@Description              NVARCHAR(255),
	@Amount                   DECIMAL(18,2),
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@ActualDeliveryDate       DATETIME,
	@IsHourlyAmount           BIT,
	@UserLogin                NVARCHAR(255),
	@ConsultantsCanAdjust	  BIT,
	@IsChargeable			  BIT
)
AS
	SET NOCOUNT ON

	DECLARE @StartDateShift INT
	DECLARE @DurationShift INT

	-- Previous values
	SELECT @StartDateShift = DATEDIFF(dd, m.StartDate, @StartDate),
	       @DurationShift =
	           DATEDIFF(dd, @StartDate, @ProjectedDeliveryDate) - DATEDIFF(dd, m.StartDate, m.ProjectedDeliveryDate)
	  FROM dbo.Milestone AS m
	 WHERE MilestoneId = @MilestoneId

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	-- Change the milestone
	UPDATE dbo.Milestone
	   SET ProjectId = @ProjectId,
	       Description = @Description,
	       Amount = @Amount,
	       StartDate = @StartDate,
	       ProjectedDeliveryDate = @ProjectedDeliveryDate,
	       ActualDeliveryDate = @ActualDeliveryDate,
	       IsHourlyAmount = @IsHourlyAmount,
	       IsChargeable = @IsChargeable,
	       ConsultantsCanAdjust = @ConsultantsCanAdjust
	 WHERE MilestoneId = @MilestoneId

	-- Determine if milestone start date was changed
	IF @StartDateShift <> 0
	BEGIN
		-- Move milestone-person entries
		UPDATE mpe
		   SET StartDate = DATEADD(dd, @StartDateShift, mpe.StartDate),
			   EndDate = DATEADD(dd, @StartDateShift, mpe.EndDate)
		  FROM dbo.MilestonePersonEntry AS mpe
			   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
		 WHERE mp.MilestoneId = @MilestoneId
	END
/*
	-- Determine if milestone duration was reduced
	IF @DurationShift < 0
	BEGIN
		
		-- Remove entries became out the milestone period
		DELETE mpe
		  FROM dbo.MilestonePersonEntry AS mpe
			   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
			   INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
		 WHERE m.MilestoneId = @MilestoneId
		   AND mpe.StartDate >= m.ProjectedDeliveryDate

		-- Move last milestone-person entry for each assignment
		UPDATE mpe
		   SET EndDate = m.ProjectedDeliveryDate
		  FROM dbo.MilestonePersonEntry AS mpe
			   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
			   INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
		 WHERE mp.MilestoneId = @MilestoneId
		   AND mpe.EndDate > m.ProjectedDeliveryDate
	END
*/
	-- Determine if milestone duration was increased
	IF @DurationShift > 0
	BEGIN
		-- Move last milestone-person entry for each assignment
		UPDATE mpe
		   SET EndDate = DATEADD(dd, @DurationShift, mpe.EndDate)
		  FROM dbo.MilestonePersonEntry AS mpe
			   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
		 WHERE mp.MilestoneId = @MilestoneId
		   AND mpe.EndDate =
				   (SELECT MAX(EndDate) FROM dbo.MilestonePersonEntry AS e WHERE e.MilestonePersonId = mpe.MilestonePersonId)
	END

	-- End logging session
	EXEC dbo.SessionLogUnprepare

