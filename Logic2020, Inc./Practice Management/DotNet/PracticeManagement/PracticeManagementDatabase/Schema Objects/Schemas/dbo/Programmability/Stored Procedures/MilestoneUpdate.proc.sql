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
	       ActualDeliveryDate = @ActualDeliveryDate,
	       IsHourlyAmount = @IsHourlyAmount,
	       IsChargeable = @IsChargeable,
	       ConsultantsCanAdjust = @ConsultantsCanAdjust
	 WHERE MilestoneId = @MilestoneId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
 END
