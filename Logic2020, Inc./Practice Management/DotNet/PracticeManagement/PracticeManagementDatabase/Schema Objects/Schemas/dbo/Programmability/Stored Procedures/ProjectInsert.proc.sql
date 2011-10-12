﻿-- =============================================
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
	@OpportunityId		INT = NULL
)
AS
BEGIN

	SET NOCOUNT ON;

	BEGIN TRAN  T1;

	-- Generating Project Number
	DECLARE @ProjectNumber NVARCHAR(12)
	EXEC dbo.GenerateNewProjectNumber @ProjectNumber OUTPUT ;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	-- Inserting Project
	INSERT INTO dbo.Project
	            (ClientId, Discount, Terms, Name, PracticeId,
	             ProjectStatusId, ProjectNumber, BuyerName, GroupId, IsChargeable,  DirectorId, OpportunityId)
	     VALUES (@ClientId, @Discount, @Terms, @Name, @PracticeId,
	             @ProjectStatusId, @ProjectNumber, @BuyerName, @GroupId, @IsChargeable, @DirectorId, @OpportunityId)

	-- End logging session
	EXEC dbo.SessionLogUnprepare

	SET @ProjectId = SCOPE_IDENTITY()

	INSERT INTO ProjectManagers(ProjectId,ProjectManagerId)
	SELECT  @ProjectId,ResultId
	FROM    dbo.ConvertStringListIntoTable(@ProjectManagerIdsList)
     
	COMMIT TRAN T1;
END
