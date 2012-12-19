CREATE PROCEDURE [dbo].[GetPersonsByTodayHireDate]	
AS
BEGIN
	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))

	--unlock out
	UPDATE m
	SET m.IsLockedOut = 0,
	    m.Password = ''
	FROM   dbo.aspnet_Users AS u
	INNER JOIN dbo.aspnet_Applications AS a ON u.ApplicationId = a.ApplicationId
	INNER JOIN  dbo.aspnet_Membership AS m ON u.UserId = m.UserId
	INNER JOIN Person as P ON u.LoweredUserName = LOWER(P.Alias) 
	WHERE P.PersonStatusId IN (1,5) AND HireDate = (@Today - 1) AND P.IsWelcomeEmailSent = 0 AND a.LoweredApplicationName =LOWER('PracticeManagement')

	SELECT P.PersonId,
	       p.FirstName,
		   p.Alias
	FROM  Person as P 
	WHERE P.PersonStatusId IN (1,5) AND HireDate = (@Today - 1) AND P.IsWelcomeEmailSent = 0

END

