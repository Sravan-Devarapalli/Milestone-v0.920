-- =============================================
-- Description:	Inserts a Project
-- Updated By:	Srinivas.M
-- Updated Date: 2012-06-05
-- =============================================
CREATE PROCEDURE dbo.ProjectInsert
(
	@ProjectId          INT OUT,
	@ClientId           INT,
	@Terms              INT,
	@Name               NVARCHAR(100),
	@PracticeId         INT,
	@ProjectStatusId    INT,
	@BuyerName          NVARCHAR(100),
	@UserLogin          NVARCHAR(255),
	@GroupId			INT,
	@ProjectManagerIdsList	NVARCHAR(MAX),
	@DirectorId			INT = NULL,
	@OpportunityId		INT = NULL,
	@Description        NVARCHAR(MAX),
	@CanCreateCustomWorkTypes BIT,
	@IsInternal			BIT,
	@IsNoteRequired     BIT = 1,
	@ProjectOwner       INT = NULL,
	@SowBudget			DECIMAL(18,2),
	@ProjectCapabilityIds NVARCHAR(MAX) = NULL,
	@PricingListId      INT = NULL,
	@BusinessTypeId		INT = NULL
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

	DECLARE @Discount DECIMAL(18,2)
	SELECT @Discount = C.DefaultDiscount
	FROM dbo.Client C
	WHERE C.ClientId = @ClientId

	-- Inserting Project
	INSERT INTO dbo.Project
	            (ClientId, Terms, Name, PracticeId,
	             ProjectStatusId, ProjectNumber, BuyerName, GroupId, DirectorId, OpportunityId, Description, CanCreateCustomWorkTypes, IsInternal, IsNoteRequired, ProjectOwnerId, SowBudget, Discount,PricingListId,BusinessTypeId)
	     VALUES (@ClientId, @Terms, @Name, @PracticeId,
	             @ProjectStatusId, @ProjectNumber, @BuyerName, @GroupId, @DirectorId, @OpportunityId, @Description, @CanCreateCustomWorkTypes, @IsInternal, @IsNoteRequired, @ProjectOwner, @SowBudget, @Discount,@PricingListId,@BusinessTypeId)
	
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

	INSERT INTO dbo.ProjectCapabilities(ProjectId,CapabilityId)
	SELECT  @ProjectId,ResultId
	FROM    dbo.ConvertStringListIntoTable(@ProjectCapabilityIds)
     
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

