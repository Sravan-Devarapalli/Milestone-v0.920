-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 8-18-2008
-- Updated by:	
-- Update date: 
-- Description:	Removes the users membership
-- =============================================
CREATE PROCEDURE [dbo].[MembershipDelete]
(
	@UserId   UNIQUEIDENTIFIER
)
AS
	SET NOCOUNT ON

	DELETE FROM dbo.aspnet_UsersInRoles
	 WHERE UserId = @UserId

	DELETE FROM dbo.aspnet_Membership
	 WHERE UserId = @UserId

