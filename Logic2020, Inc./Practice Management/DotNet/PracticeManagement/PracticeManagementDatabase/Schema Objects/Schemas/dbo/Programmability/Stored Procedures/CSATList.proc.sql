CREATE PROCEDURE [dbo].[CSATList]
(
	@ProjectId		INT = NULL
)
AS
BEGIN

	SELECT cs.CSATId,
			cs.Comments,
			cs.CompletionDate,
			cs.ReviewStartDate,
			cs.ReviewEndDate,
			cs.ProjectId,
			cs.ReferralScore,
			cs.ReviewerId,
			re.FirstName + ', '+ re.LastName AS [Reviewer]
	FROM dbo.ProjectCSAT cs
	INNER JOIN dbo.Person re on cs.ReviewerId = re.PersonId
	WHERE @ProjectId IS NULL OR cs.ProjectId = @ProjectId

END
