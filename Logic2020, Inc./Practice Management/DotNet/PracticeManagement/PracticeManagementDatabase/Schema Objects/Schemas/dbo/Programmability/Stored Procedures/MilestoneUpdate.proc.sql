CREATE PROCEDURE dbo.MilestoneUpdate
(
	@MilestoneId              INT,
	@ProjectId                INT,
	@Description              NVARCHAR(255),
	@Amount                   DECIMAL(18,2),
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@IsHourlyAmount           BIT,
	@UserLogin                NVARCHAR(255),
	@ConsultantsCanAdjust	  BIT,
	@IsChargeable			  BIT
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @ProjectNewStartDate	DATETIME,
			@ProjectNewEndDate		DATETIME

	-- Change the milestone
	UPDATE dbo.Milestone
	   SET ProjectId = @ProjectId,
	       Description = @Description,
	       Amount = @Amount,
	       StartDate = @StartDate,
	       ProjectedDeliveryDate = @ProjectedDeliveryDate,
	       IsHourlyAmount = @IsHourlyAmount,
	       IsChargeable = @IsChargeable,
	       ConsultantsCanAdjust = @ConsultantsCanAdjust
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

	-- End logging session
	EXEC dbo.SessionLogUnprepare
 END

