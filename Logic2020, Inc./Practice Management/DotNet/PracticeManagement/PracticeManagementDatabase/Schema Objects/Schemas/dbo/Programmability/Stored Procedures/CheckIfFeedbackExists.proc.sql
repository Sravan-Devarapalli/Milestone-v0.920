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
			@PersonEndDateLocal		DATETIME,
			@PersonId				INT
	
	SELECT @ProjectIdLocal= ProjectId, @StartDateLocal= StartDate, @EndDateLocal= ProjectedDeliveryDate FROM dbo.Milestone WHERE @MilestoneId IS NOT NULL AND MilestoneId = @MilestoneId

	SELECT @PersonId = MP.PersonId,@ProjectIdLocal = M.ProjectId
	FROM dbo.MilestonePerson MP
	JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
	WHERE MP.MilestonePersonId = @MilestonePersonId
	
	IF @MilestonePersonId IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE PersonId = @PersonId AND ProjectId = @ProjectIdLocal AND FeedbackStatusId = 1 AND ReviewPeriodStartDate <= @EndDate AND @StartDate <= ReviewPeriodEndDate AND ReviewPeriodEndDate >= '20140701')
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
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE ProjectId = @ProjectIdLocal AND FeedbackStatusId = 1 AND ReviewPeriodStartDate <= @EndDateLocal AND @StartDateLocal <= ReviewPeriodEndDate AND ReviewPeriodEndDate >= '20140701')
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END
	END

END
