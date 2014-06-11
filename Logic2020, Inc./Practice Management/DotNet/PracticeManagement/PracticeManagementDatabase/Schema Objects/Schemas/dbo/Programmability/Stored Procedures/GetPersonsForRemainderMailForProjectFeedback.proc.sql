CREATE PROCEDURE [dbo].[GetPersonsForRemainderMailForProjectFeedback]
AS
BEGIN

	DECLARE @Today		DATETIME,
			@SendAfter	DATETIME='20140630'
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))


	SELECT	P.PersonId,
			P.FirstName,
			P.LastName,
			P.Alias,
			T.TitleId,
			T.Title,
			PF.ReviewPeriodStartDate,
			PF.ReviewPeriodEndDate,
			PF.DueDate,
			Pro.ProjectId,
			Pro.ProjectNumber,
			Pro.Name AS ProjectName,
			dbo.GetProjectManagersAliasesList(PF.ProjectId) AS ToAddressList,
			director.Alias AS DirectorAlias,
			owner.Alias AS ProjectOwnerAlias,
			seniorManager.Alias AS SeniorManagerAlias
	FROM dbo.ProjectFeedback PF
	INNER JOIN dbo.Person P ON P.PersonId = PF.PersonId 
	INNER JOIN dbo.Project Pro ON Pro.ProjectId = PF.ProjectId
	LEFT JOIN dbo.Person director ON director.PersonId = Pro.DirectorId
	LEFT JOIN dbo.Person owner ON owner.PersonId = Pro.ProjectOwnerId
	LEFT JOIN dbo.Person seniorManager ON seniorManager.PersonId = Pro.SeniorManagerId
	LEFT JOIN dbo.Title T ON T.TitleId = P.TitleId
	WHERE CONVERT(NVARCHAR(10), @Today, 101) > CONVERT(NVARCHAR(10), @SendAfter, 101)
	AND CONVERT(NVARCHAR(10), @Today, 101) > CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 101)
	AND PF.IsCanceled = 0 AND PF.FeedbackStatusId = 2 --Not Completed Status
	AND DATEPART(WEEKDAY,PF.ReviewPeriodEndDate) = DATEPART(WEEKDAY,@Today)

END
