-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-21-2008
-- Updated by:	Anton Kramarenko
-- Update date: 02-20-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 07-16-2009
-- Description:	Inserts a Project record
-- Updated by:	Ravi Narsini
-- Update date:	10-26-2010 : Changes: Added default milestone logic (#2600)
-- =============================================
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
	@ProjectManagerId	INT
)
AS
	SET NOCOUNT ON

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
	       ProjectManagerId = @ProjectManagerId,
		   DirectorId		= @DirectorId
	 WHERE ProjectId = @ProjectId

	-- End logging session
	EXEC dbo.SessionLogUnprepare


