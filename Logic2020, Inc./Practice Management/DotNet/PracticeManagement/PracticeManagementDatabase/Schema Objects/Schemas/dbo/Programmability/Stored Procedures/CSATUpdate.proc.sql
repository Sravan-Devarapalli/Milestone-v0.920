CREATE PROCEDURE [dbo].[CSATUpdate]
(
	@ProjectCSATId		INT, 
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@CompletionDate DATETIME,
	@ReviewerId		INT,
	@ReferralScore	INT,
	@Comments		NVARCHAR(MAX),
	@UserLogin      NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	DECLARE @InsertingTime DATETIME,
	        @ModifiedBy    INT
	SELECT @InsertingTime=dbo.InsertingTime(),@ModifiedBy=PersonId FROM Person WHERE Alias=@UserLogin;

	UPDATE dbo.ProjectCSAT
	SET ReviewStartDate = @ReviewStartDate,
		ReviewEndDate = @ReviewEndDate,
		CompletionDate = @CompletionDate,
		Comments = @Comments,
		ReferralScore = @ReferralScore,
		ReviewerId = @ReviewerId,
		ModifiedDate = @InsertingTime,
		ModifiedBy =@ModifiedBy
	WHERE [CSATId] = @ProjectCSATId AND 
		  (
			ReviewStartDate != @ReviewStartDate OR 
			ReviewEndDate != @ReviewEndDate OR
			CompletionDate != @CompletionDate OR
			Comments != @Comments OR
			ReferralScore != @ReferralScore OR
			ReviewerId != @ReviewerId
		  )

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END

