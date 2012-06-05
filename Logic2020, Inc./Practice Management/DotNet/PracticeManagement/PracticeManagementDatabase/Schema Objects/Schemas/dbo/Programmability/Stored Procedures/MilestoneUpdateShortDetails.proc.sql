-- =============================================
-- Author:		Srinivas.M
-- Create date: 06-05-2012
-- Updated By:	
-- Updated Date: 
-- Description:	Update some details Milestone.
-- =============================================
CREATE PROCEDURE [dbo].[MilestoneUpdateShortDetails]
(
	@MilestoneId              INT,
	@Description              NVARCHAR(255),
	@UserLogin                NVARCHAR(255)
)
AS
BEGIN
	SET NOCOUNT ON;

	-- Start logging session
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

	-- Change the milestone
	UPDATE dbo.Milestone
	   SET [Description] = @Description
	 WHERE MilestoneId = @MilestoneId

	-- End logging session
	EXEC dbo.SessionLogUnprepare
END
