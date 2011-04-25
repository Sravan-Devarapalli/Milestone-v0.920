-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-22-2008
-- Updated by:	
-- Update date: 
-- Description:	Lock out User.
-- =============================================
CREATE PROCEDURE [dbo].[aspnet_Membership_LockUser]
(
    @ApplicationName	nvarchar(256),
    @UserName			nvarchar(256),
    @LastLockoutDate	datetime
)
AS
    UPDATE m
    SET IsLockedOut = 1,        
        LastLockoutDate = @LastLockoutDate
    FROM    dbo.aspnet_Users AS u, dbo.aspnet_Applications AS a, dbo.aspnet_Membership AS m
    WHERE   LoweredUserName = LOWER(@UserName) AND
            u.ApplicationId = a.ApplicationId  AND
            LOWER(@ApplicationName) = a.LoweredApplicationName AND
            u.UserId = m.UserId

