CREATE PROCEDURE [dbo].[CSATCopyFromExistingCSAT]
(
	@CopyProjectCSATId		INT, 
	@ReviewStartDate DATETIME,
	@ReviewEndDate DATETIME,
	@CompletionDate DATETIME,
	@ReviewerId		INT,
	@ReferralScore	INT,
	@UserLogin      NVARCHAR(255)
)
AS
BEGIN
	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	INSERT INTO ProjectCSAT(ProjectId,ReviewStartDate,ReviewEndDate,CompletionDate,ReferralScore,ReviewerId,Comments)
	SELECT cs.ProjectId,@ReviewStartDate,@ReviewEndDate,@CompletionDate,@ReferralScore,@ReviewerId,cs.Comments
	FROM dbo.ProjectCSAT cs
	WHERE cs.CSATId = @CopyProjectCSATId

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
