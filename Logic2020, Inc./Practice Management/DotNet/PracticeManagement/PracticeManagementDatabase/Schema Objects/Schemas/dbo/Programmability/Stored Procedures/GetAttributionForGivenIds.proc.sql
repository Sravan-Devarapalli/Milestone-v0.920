CREATE PROCEDURE [dbo].[GetAttributionForGivenIds]
(
	@AttributionIds NVARCHAR(MAX) 
)
AS
BEGIN

	SELECT  P.ProjectNumber,
			P.Name AS ProjectName,
			A.*
	FROM dbo.Attribution A
	INNER JOIN dbo.Project P ON P.ProjectId = A.ProjectId 
	WHERE A.AttributionId IN (SELECT ResultId	FROM [dbo].[ConvertStringListIntoTable](@AttributionIds))

END

