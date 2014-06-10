CREATE PROCEDURE [dbo].[SaveFeedbackCancelationDetails]
(
	@FeedbackId	INT,
	@StatusId	INT=2,--status 2 is not completed
	@IsCanceled BIT,
	@CancelationReason NVARCHAR(MAX)=NULL,
	@UserLogin       NVARCHAR(255)
)
AS
BEGIN
 
	DECLARE @ModifiedBy INT,
			@CurrentPMTime DATETIME
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin
	SELECT @CurrentPMTime = dbo.InsertingTime()

	EXEC SessionLogUnprepare
	EXEC SessionLogPrepare @UserLogin = @UserLogin

	UPDATE dbo.ProjectFeedback 
	SET	IsCanceled = @IsCanceled,
		CancelationReason = @CancelationReason,
		FeedbackStatusId = CASE WHEN @IsCanceled = 1 THEN 3 ELSE @StatusId END,
		CompletionCertificateBy = @ModifiedBy,
		CompletionCertificateDate = @CurrentPMTime
	WHERE FeedbackId =  @FeedbackId
		  
END
