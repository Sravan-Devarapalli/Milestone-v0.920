
CREATE PROCEDURE dbo.ProjectUpdate
(
	@ProjectId          INT,
	@ClientId           INT,
	@Discount           DECIMAL(18,2),
	@Terms              INT,
	@Name               NVARCHAR(100),
	@PracticeId         INT,
	@ProjectStatusId    INT,
	@BuyerName          NVARCHAR(100),
	@UserLogin          NVARCHAR(255),
	@GroupId			INT,
	@IsChargeable		BIT,
	@DirectorId			INT,
	@ProjectManagerIdsList	NVARCHAR(MAX)
)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRAN  T1;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	UPDATE dbo.Project
	   SET ClientId			= @ClientId,
	       Discount			= @Discount,
	       Terms			= @Terms,
	       NAME				= @Name,
	       PracticeId		= @PracticeId,
	       ProjectStatusId	= @ProjectStatusId,
	       BuyerName		= @BuyerName,
	       GroupId			= @GroupId,
	       IsChargeable		= @IsChargeable,
		   DirectorId		= @DirectorId
	 WHERE ProjectId = @ProjectId

	        DELETE pm
			FROM ProjectManagers pm
			LEFT JOIN [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
			ON pm.ProjectId = @ProjectId AND pm.ProjectManagerId = p.ResultId 
			WHERE p.ResultId IS NULL and pm.ProjectId = @ProjectId

			INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
			SELECT @ProjectId ,p.ResultId
			FROM [dbo].ConvertStringListIntoTable(@ProjectManagerIdsList) AS p 
			LEFT JOIN ProjectManagers pm
			ON p.ResultId = pm.ProjectManagerId AND pm.ProjectId=@ProjectId
			WHERE pm.ProjectManagerId IS NULL

	-- End logging session
	EXEC dbo.SessionLogUnprepare

	COMMIT TRAN T1;	

END
