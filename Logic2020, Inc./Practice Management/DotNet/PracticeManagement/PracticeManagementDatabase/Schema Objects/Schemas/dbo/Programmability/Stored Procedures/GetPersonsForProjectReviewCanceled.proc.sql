CREATE PROCEDURE [dbo].[GetPersonsForProjectReviewCanceled]
(
	@PersonId		INT,
	@UserLogin       NVARCHAR(255)
)
AS
BEGIN

	DECLARE @Today DATETIME,
			@ModifiedBy INT,
			@SendAfter	DATETIME='20140630'
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
	SELECT @ModifiedBy = PersonId FROM Person WHERE Alias = @UserLogin

	--SELECT	P.PersonId,
	--		P.FirstName,
	--		P.LastName,
	--		P.Alias,
	--		T.TitleId,
	--		T.Title,
	--		PF.ReviewPeriodStartDate,
	--		PF.ReviewPeriodEndDate,
	--		PF.DueDate,
	--		Pro.ProjectId,
	--		Pro.ProjectNumber,
	--		Pro.Name AS ProjectName,
	--		dbo.GetProjectManagersAliasesList(PF.ProjectId) AS ToAddressList,
	--		director.Alias AS DirectorAlias,
	--		owner.Alias AS ProjectOwnerAlias,
	--		seniorManager.Alias AS SeniorManagerAlias
	--FROM dbo.ProjectFeedback PF
	--INNER JOIN dbo.Person P ON P.PersonId = PF.PersonId 
	--INNER JOIN dbo.Project Pro ON Pro.ProjectId = PF.ProjectId
	--LEFT JOIN dbo.Person director ON director.PersonId = Pro.ExecutiveInChargeId
	--LEFT JOIN dbo.Person owner ON owner.PersonId = Pro.ProjectManagerId
	--LEFT JOIN dbo.Person seniorManager ON seniorManager.PersonId = Pro.EngagementManagerId
	--LEFT JOIN dbo.Title T ON T.TitleId = P.TitleId
	--WHERE CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 111) <= CONVERT(NVARCHAR(10), @Today, 111)
	--			AND PF.IsCanceled = 0 AND PF.FeedbackStatusId = 2 --Not Completed Status
	--			AND P.PersonId = @PersonId
	--			AND CONVERT(NVARCHAR(10), @Today, 101) > CONVERT(NVARCHAR(10), @SendAfter, 101)
	--			AND CONVERT(NVARCHAR(10), PF.ReviewPeriodEndDate, 111) > CONVERT(NVARCHAR(10), @SendAfter, 111)
	--			AND Pro.ProjectStatusId IN (3,4)
	--Cancel feedback records for the person if the status is 'not completed' as per #3271
	
	UPDATE dbo.ProjectFeedback
	SET FeedbackStatusId = 3,
		IsCanceled = 1,
		CancelationReason = 'Termination from company,'+ CONVERT(NVARCHAR(10), @Today, 101),
		CompletionCertificateBy = @ModifiedBy,
		CompletionCertificateDate = @Today
	WHERE PersonId = @PersonId AND FeedbackStatusId = 2 --Status 2 = not completed
	AND CONVERT(NVARCHAR(10), ReviewPeriodEndDate, 111) > CONVERT(NVARCHAR(10), @SendAfter, 111)
END
