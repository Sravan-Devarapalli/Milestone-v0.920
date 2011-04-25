-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 10-14-2008
-- Updated by:	
-- Update date:	
-- Description:	Prepares the data for logging the user's activity.
-- =============================================
CREATE PROCEDURE [dbo].[SessionLogUnprepare]
AS
	SET NOCOUNT ON


	DELETE
		FROM dbo.SessionLogData
		WHERE SessionID = @@SPID


