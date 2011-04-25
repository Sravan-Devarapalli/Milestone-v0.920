-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-13-2008
-- Updated by:	
-- Update date:	
-- Description:	Deletes the billing info for the project.
-- =============================================
CREATE PROCEDURE [dbo].[ProjectBillingInfoDelete]
(
	@ProjectId         INT
)
AS
	SET NOCOUNT ON

	DELETE
	  FROM dbo.ProjectBillingInfoNote
	 WHERE ProjectId = @ProjectId

	DELETE
	  FROM dbo.ProjectBillingInfo
	 WHERE ProjectId = @ProjectId

