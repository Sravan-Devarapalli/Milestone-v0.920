CREATE PROCEDURE [dbo].[GetProjectFeedbackByProjectId]
(
	@ProjectId		INT
)
AS
BEGIN

	SELECT PF.FeedbackId,
		   PF.PersonId,
		   P.FirstName,
		   P.LastName,
		   P.TitleId,
		   T.Title,
		   PF.ReviewPeriodStartDate AS ReviewStartDate,
		   PF.ReviewPeriodEndDate AS ReviewEndDate,
		   PF.DueDate,
		   PFS.FeedbackStatusId,
		   PFS.Name AS FeedbackStatus,
		   C.FirstName+' '+C.LastName AS CompletionCertificateBy,
		   PF.CompletionCertificateDate,
		   PF.IsCanceled,
		   PF.CancelationReason,
		   ISNULL(PF.IsGap,0) IsGap
	FROM dbo.ProjectFeedback PF
	INNER JOIN dbo.ProjectFeedbackStatus PFS ON PFS.FeedbackStatusId = PF.FeedbackStatusId
	INNER JOIN dbo.Person P ON P.PersonId = PF.PersonId
	INNER JOIN dbo.Title T ON T.TitleId = P.TitleId
	LEFT JOIN dbo.Person C ON C.PersonId = PF.CompletionCertificateBy
	WHERE PF.ProjectId = @ProjectId
		  AND PF.ReviewPeriodEndDate >= '20140701'
	ORDER BY P.LastName,P.FirstName,PF.ReviewPeriodStartDate

END

