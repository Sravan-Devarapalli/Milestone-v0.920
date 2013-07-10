CREATE PROCEDURE [dbo].[ProjectAttributionReport]
	(
	@StartDate	DATETIME,
	@EndDate	DATETIME
	)
AS
BEGIN
	SELECT	ART.Name AS RecordType,
			AT.Name AS AttributionType,
			PS.Name AS ProjectStatus,
			P.ProjectNumber,
			C.Name AS Account,
			BG.Name AS BusinessGroup,
			PG.Name AS BusinessUnit,
			P.Name AS ProjectName,
			BT.Name AS NewBusinessOrExtension,
		   CASE WHEN ART.IsRangeType = 1 THEN person.LastName+', '+person.FirstName ELSE practice.Name END AS Name,
		   CASE WHEN ART.IsRangeType = 1 THEN title.Title ELSE '' END AS Title,
		   ISNULL(A.StartDate,P.StartDate) AS StartDate,
		   ISNULL(A.EndDate,P.EndDate) AS EndDate,
		   A.Percentage AS CommissionPercentage,
		   P.BusinessTypeId AS NewOrExtension
	FROM dbo.Attribution AS A
	INNER JOIN dbo.Project AS P ON P.ProjectId = A.ProjectId 
	INNER JOIN dbo.AttributionTypes AS AT ON AT.AttributionTypeId = A.AttributionTypeId
	INNER JOIN dbo.AttributionRecordTypes AS ART ON ART.AttributionRecordId = A.AttributionRecordTypeId
	INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = P.ProjectStatusId
	INNER JOIN dbo.Client AS C ON C.ClientId = P.ClientId
	INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = P.GroupId
	INNER JOIN dbo.BusinessGroup AS BG ON BG.BusinessGroupId = PG.BusinessGroupId
	LEFT JOIN dbo.BusinessType AS BT ON BT.BusinessTypeId = P.BusinessTypeId 
	LEFT JOIN dbo.Person AS person ON person.PersonId = A.TargetId
	LEFT JOIN dbo.Title AS title ON title.TitleId = person.TitleId
	LEFT JOIN dbo.Practice AS practice ON practice.PracticeId = A.TargetId
	WHERE ((@StartDate <= A.EndDate AND A.StartDate <= @EndDate) OR (ART.IsRangeType = 0 AND @StartDate<=P.EndDate AND P.StartDate <= @EndDate)) AND P.ProjectStatusId IN (2,3,4)
END

