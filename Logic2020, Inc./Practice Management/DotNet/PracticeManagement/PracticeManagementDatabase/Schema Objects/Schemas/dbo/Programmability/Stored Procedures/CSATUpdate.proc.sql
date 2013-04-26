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

	UPDATE dbo.ProjectCSAT
	SET ReviewStartDate = @ReviewStartDate,
		ReviewEndDate = @ReviewEndDate,
		CompletionDate = @CompletionDate,
		Comments = @Comments,
		ReferralScore = @ReferralScore,
		ReviewerId = @ReviewerId
	WHERE [CSATId] = @ProjectCSATId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
