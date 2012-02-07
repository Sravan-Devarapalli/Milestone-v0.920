-- =============================================
-- Description:	Inserts a Milestone
-- Updated by:	Ravi Narsini
-- Update date:	10-26-2010 : Changes: Added default milestone logic (#2600)
-- =============================================
CREATE PROCEDURE dbo.ProjectInsert
(
	@ProjectId          INT OUT,
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
	@ProjectManagerIdsList	NVARCHAR(MAX),
	@DirectorId			INT = NULL,
	@OpportunityId		INT = NULL,
	@Description        NVARCHAR(MAX),
	@CanCreateCustomWorkTypes BIT,
	@IsInternal			BIT 
)
AS
BEGIN

	SET NOCOUNT ON;
	
	BEGIN TRY
	BEGIN TRAN  T1;

	-- Generating Project Number
	DECLARE @ProjectNumber NVARCHAR(12) 
	EXEC dbo.GenerateNewProjectNumber @IsInternal , @ProjectNumber OUTPUT 

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	-- Inserting Project
	INSERT INTO dbo.Project
	            (ClientId, Discount, Terms, Name, PracticeId,
	             ProjectStatusId, ProjectNumber, BuyerName, GroupId, IsChargeable,  DirectorId, OpportunityId,Description,CanCreateCustomWorkTypes,IsInternal)
	     VALUES (@ClientId, @Discount, @Terms, @Name, @PracticeId,
	             @ProjectStatusId, @ProjectNumber, @BuyerName, @GroupId, @IsChargeable, @DirectorId, @OpportunityId,@Description,@CanCreateCustomWorkTypes,@IsInternal)
	
	IF(@OpportunityId IS NOT NULL)
	BEGIN
	  
	  UPDATE dbo.Opportunity 
	  SET Description = @Description
	  WHERE OpportunityId = @OpportunityId 
	 
	END

	-- End logging session
	EXEC dbo.SessionLogUnprepare

	SET @ProjectId = SCOPE_IDENTITY()
		
	IF @IsInternal = 1
	BEGIN
		INSERT INTO dbo.ProjectTimeType(ProjectId,TimeTypeId,IsAllowedToShow)
		SELECT @ProjectId, TT.TimeTypeId, 0
		FROM dbo.TimeType TT
		WHERE TT.IsDefault = 1 AND TT.Code NOT IN ('W6000')--Here 'W6000' is code for General Default work type.

	END

	INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
	SELECT  @ProjectId,ResultId
	FROM    dbo.ConvertStringListIntoTable(@ProjectManagerIdsList)
     
	COMMIT TRAN T1;
	END TRY
	BEGIN CATCH
	ROLLBACK TRAN T1;
		DECLARE	 @ERROR_STATE	tinyint
				,@ERROR_SEVERITY		tinyint
				,@ERROR_MESSAGE		    nvarchar(2000)
				,@InitialTranCount		tinyint

		SET	 @ERROR_MESSAGE		= ERROR_MESSAGE()
		SET  @ERROR_SEVERITY	= ERROR_SEVERITY()
		SET  @ERROR_STATE		= ERROR_STATE()
		RAISERROR ('%s', @ERROR_SEVERITY, @ERROR_STATE, @ERROR_MESSAGE)
	END CATCH
END

