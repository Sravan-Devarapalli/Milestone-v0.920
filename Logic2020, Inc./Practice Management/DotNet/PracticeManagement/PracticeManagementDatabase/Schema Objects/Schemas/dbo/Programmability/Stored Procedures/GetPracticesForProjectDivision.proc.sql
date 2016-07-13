CREATE PROCEDURE [dbo].[GetPracticesForProjectDivision]
(
	@DivisionId	INT
)
AS
BEGIN
	SELECT 
		P.PracticeId,
		P.Name as PracticeAreaName
	FROM dbo.Practice P
			INNER JOIN dbo.ProjectDivisionPracticeMapping DP ON DP.PracticeId=p.PracticeId
	WHERE DP.ProjectDivisionId=@DivisionId and P.IsActive=1
	ORDER BY P.Name
END
