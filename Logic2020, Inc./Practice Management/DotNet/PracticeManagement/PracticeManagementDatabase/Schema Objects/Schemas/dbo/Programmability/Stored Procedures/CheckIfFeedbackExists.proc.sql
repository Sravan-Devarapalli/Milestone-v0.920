CREATE PROCEDURE [dbo].[CheckIfFeedbackExists]
(
	@MilestonePersonId INT=NULL,
	@MilestoneId INT=NULL,
	@ProjectId	INT=NULL
)
AS
BEGIN
    DECLARE @ProjectIdLocal INT=NULL
	SELECT @ProjectIdLocal= ProjectId FROM dbo.Milestone WHERE @MilestoneId IS NOT NULL AND MilestoneId = @MilestoneId
	
	IF @MilestonePersonId IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE MilestonePersonId = @MilestonePersonId AND FeedbackStatusId = 1)
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
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE ProjectId = @ProjectIdLocal AND FeedbackStatusId = 1)
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END
	END

	IF @ProjectId IS NOT NULL
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.ProjectFeedback WHERE ProjectId = @ProjectId AND FeedbackStatusId = 1)
		BEGIN
			SELECT CONVERT(BIT,1) Result 
		END
		ELSE
		BEGIN
			SELECT CONVERT(BIT,0) Result
		END
	END

END
