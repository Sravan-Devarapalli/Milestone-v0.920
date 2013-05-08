CREATE PROCEDURE [dbo].[CSATInsert]
(
	@ProjectCSATId			INT OUTPUT,
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

	DECLARE @InsertingTime DATETIME,
	        @ModifiedBy    INT
	SELECT @InsertingTime=dbo.InsertingTime(),@ModifiedBy=PersonId FROM Person WHERE Alias=@UserLogin;

	INSERT INTO ProjectCSAT(ProjectId,ReviewStartDate,ReviewEndDate,CompletionDate,ReferralScore,ReviewerId,Comments,CreatedDate,ModifiedDate,ModifiedBy)
	VALUES (@ProjectId,@ReviewStartDate,@ReviewEndDate,@CompletionDate,@ReferralScore,@ReviewerId,@Comments,@InsertingTime,@InsertingTime,@ModifiedBy)

	SET @ProjectCSATId = SCOPE_IDENTITY()

	-- End logging session
	EXEC dbo.SessionLogUnprepare

END

