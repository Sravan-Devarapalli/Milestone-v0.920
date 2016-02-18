CREATE PROCEDURE [dbo].[GetPracticesForDivision]
	@DivisionId	INT
AS
BEGIN
	SELECT 
		P.PracticeId,
		P.Name as PracticeAreaName
	FROM dbo.Practice P
			INNER JOIN dbo.DivisionPracticeArea DP ON DP.PracticeId=p.PracticeId
	WHERE DP.DivisionId=@DivisionId and P.IsActive=1
	ORDER BY P.Name
END

