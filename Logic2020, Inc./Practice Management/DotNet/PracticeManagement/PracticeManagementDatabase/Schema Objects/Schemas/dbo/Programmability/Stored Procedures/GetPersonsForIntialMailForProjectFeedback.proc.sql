CREATE PROCEDURE [dbo].[GetPersonsForIntialMailForProjectFeedback]
(
	@FeedbackId	INT=NULL
)
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
	LEFT JOIN dbo.Person director ON director.PersonId = Pro.ExecutiveInChargeId
	LEFT JOIN dbo.Person owner ON owner.PersonId = Pro.ProjectManagerId
	LEFT JOIN dbo.Person seniorManager ON seniorManager.PersonId = Pro.EngagementManagerId
	LEFT JOIN dbo.Title T ON T.TitleId = P.TitleId
	WHERE (@FeedbackId IS NOT NULL AND PF.FeedbackId = @FeedbackId)
		   OR (@FeedbackId IS NULL AND CONVERT(NVARCHAR(10), PF.NextIntialMailSendDate, 111) = CONVERT(NVARCHAR(10), @Today, 111)
				AND PF.IsCanceled = 0 AND PF.FeedbackStatusId = 2) --Not Completed Status
				AND CONVERT(NVARCHAR(10), @Today, 111) > CONVERT(NVARCHAR(10), @SendAfter, 111)
				AND CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 111) > CONVERT(NVARCHAR(10), @SendAfter, 111)
				AND Pro.ProjectStatusId IN (3,4)
	ORDER BY PF.ReviewPeriodEndDate DESC			
END
