CREATE PROCEDURE [dbo].[CSATInsert]
(
	@ProjectId		INT, 
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

	INSERT INTO ProjectCSAT(ProjectId,ReviewStartDate,ReviewEndDate,CompletionDate,ReferralScore,ReviewerId,Comments)
	VALUES (@ProjectId,@ReviewStartDate,@ReviewEndDate,@CompletionDate,@ReferralScore,@ReviewerId,@Comments)

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END
