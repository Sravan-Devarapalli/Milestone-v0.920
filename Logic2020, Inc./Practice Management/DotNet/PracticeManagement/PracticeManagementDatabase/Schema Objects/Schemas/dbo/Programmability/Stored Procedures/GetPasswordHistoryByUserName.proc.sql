CREATE PROCEDURE [dbo].[GetPasswordHistoryByUserName]
(
	@UserName   NVARCHAR(256)
)
AS
BEGIN
SET NOCOUNT ON;
	DECLARE @COUNT INT 
	SELECT @COUNT = s.Value FROM Settings AS s
	WHERE s.SettingsKey='OldPasswordCheckCount' AND s.TypeId = 4

	IF(@COUNT > 0)
	BEGIN
	SET @COUNT = @COUNT -1
	END
	
	SELECT TOP(@COUNT) u.Id, u.Password ,u.PasswordSalt
	FROM UserPassWordHistory AS u
	INNER JOIN aspnet_Membership AS a 
	ON a.Email = @UserName and a.UserId =u.UserId
	order by u.Id desc
END
GO
