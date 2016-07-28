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

	 --Adjust Expenses in Milestone level, but not in Project level

	 UPDATE dbo.ProjectExpense 
	 SET StartDate = CASE WHEN StartDate <= @StartDate THEN @StartDate ELSE StartDate END,
	     EndDate = CASE WHEN EndDate <= @ProjectedDeliveryDate THEN EndDate ELSE @ProjectedDeliveryDate END
	 WHERE StartDate <= @ProjectedDeliveryDate AND @StartDate <= EndDate
		   AND ProjectId = @ProjectId AND MilestoneId=@MilestoneId
		   
	 UPDATE dbo.ProjectExpense 
	 SET StartDate = @StartDate,
		 EndDate = @ProjectedDeliveryDate
	 WHERE (StartDate > @ProjectedDeliveryDate OR @StartDate > EndDate)
			AND ProjectId = @ProjectId AND MilestoneId=@MilestoneId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
 END

