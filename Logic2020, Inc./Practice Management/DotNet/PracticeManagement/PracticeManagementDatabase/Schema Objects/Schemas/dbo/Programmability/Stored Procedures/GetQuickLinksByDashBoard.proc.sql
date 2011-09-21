CREATE PROCEDURE [dbo].[GetQuickLinksByDashBoard]
(
 @dashBoardType INT 
)
AS
BEGIN

	SELECT Q.DashBoardTypeId,
		   Q.Id,
		   Q.LinkName,
		   Q.VirtualPath
	FROM QuickLinks AS Q 
	WHERE  Q.DashBoardTypeId = @dashBoardType

END
