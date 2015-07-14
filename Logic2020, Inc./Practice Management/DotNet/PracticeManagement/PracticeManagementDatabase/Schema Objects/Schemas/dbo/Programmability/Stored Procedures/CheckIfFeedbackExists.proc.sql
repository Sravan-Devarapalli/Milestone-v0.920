CREATE PROCEDURE [dbo].[CheckIfFeedbackExists]
(
	@MilestonePersonId INT=NULL,
	@MilestoneId	   INT=NULL,
	@StartDate		   DATETIME=NULL,
	@EndDate	       DATETIME=NULL
)
AS
BEGIN
    DECLARE @ProjectIdLocal			INT=NULL,
			@StartDateLocal			DATETIME,
			@EndDateLocal			DATETIME,
			@PersonStartDateLocal	DATETIME,
			@PersonEndDateLocal		DATETIME
	
	SELECT @ProjectIdLocal= ProjectId, @StartDateLocal= StartDate, @EndDateLocal= ProjectedDeliveryDate FROM dbo.Milestone WHERE @MilestoneId IS NOT NULL AND MilestoneId = @MilestoneId
	
	IF @MilestonePersonId IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE MilestonePersonId = @MilestonePersonId AND FeedbackStatusId = 1 AND ReviewPeriodStartDate <= @EndDate AND @StartDate <= ReviewPeriodEndDate)
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END
	END
   
    IF @MilestoneId IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE ProjectId = @ProjectIdLocal AND FeedbackStatusId = 1 AND ReviewPeriodStartDate <= @EndDateLocal AND @StartDateLocal <= ReviewPeriodEndDate)
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END
	END

END
