CREATE PROCEDURE [dbo].[ProjectAttributionReport]
	(
	@StartDate	DATETIME,
	@EndDate	DATETIME
	)
AS
BEGIN
	
	;WITH ProjectAttributionDetail
	AS
	(
	    SELECT	A.AttributionTypeId,--sales/delivery
				A.AttributionRecordTypeId,--person/practice
				ISNULL(A.StartDate,P.StartDate) AS StartDate,
				ISNULL(A.EndDate,P.EndDate) AS EndDate,
				A.TargetId,
				ISNULL(Per.LastName+', '+ISNULL(Per.PreferredFirstName,Per.FirstName),Pr.Name) AS TargetName,
				A.Percentage,
				A.AttributionId,
				pay.TitleId,
				title.Title,
				RANK() OVER (PARTITION BY pay.Person,A.StartDate,A.EndDate ORDER BY pay.StartDate DESC) AS [Rank],
				P.ProjectNumber,
				P.Name,
				P.BusinessTypeId,
				P.ProjectStatusId,
				P.ClientId,
				P.GroupId
		FROM dbo.Attribution AS A
		INNER JOIN dbo.Project AS P ON P.ProjectId = A.ProjectId AND  P.ProjectStatusId IN (2,3,4) AND @StartDate <= ISNULL(A.EndDate,P.EndDate) AND ISNULL(A.StartDate,P.StartDate) <= @EndDate
		LEFT JOIN dbo.Person AS Per ON A.AttributionRecordTypeId = 1 AND Per.PersonId = A.TargetId
		LEFT JOIN dbo.Practice AS Pr ON A.AttributionRecordTypeId = 2 AND Pr.PracticeId= A.TargetId
		LEFT JOIN dbo.Pay AS pay ON A.AttributionRecordTypeId = 1 AND pay.Person = Per.PersonId AND A.StartDate <= pay.EndDate-1 AND pay.StartDate <= A.EndDate
		LEFT JOIN dbo.Title AS title ON A.AttributionRecordTypeId = 1 AND title.TitleId = pay.TitleId
	)
	SELECT	ART.Name AS RecordType,
			AT.Name AS AttributionType,
			PS.Name AS ProjectStatus,
			Pro.ProjectNumber,
			C.Name AS Account,
			BG.Name AS BusinessGroup,
			PG.Name AS BusinessUnit,
			Pro.Name AS ProjectName,
			BT.Name AS NewBusinessOrExtension,
		    Pro.TargetName AS Name,
		    ISNULL(Pro.Title,'') AS Title,
			Pro.StartDate,
			Pro.EndDate,
			Pro.Percentage AS CommissionPercentage,
			Pro.BusinessTypeId AS NewOrExtension
	FROM ProjectAttributionDetail Pro 
	INNER JOIN dbo.AttributionTypes AS AT ON AT.AttributionTypeId = Pro.AttributionTypeId
	INNER JOIN dbo.AttributionRecordTypes AS ART ON ART.AttributionRecordId = Pro.AttributionRecordTypeId
	INNER JOIN dbo.ProjectStatus AS PS ON PS.ProjectStatusId = Pro.ProjectStatusId
	INNER JOIN dbo.Client AS C ON C.ClientId = Pro.ClientId
	INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = Pro.GroupId
	INNER JOIN dbo.BusinessGroup AS BG ON BG.BusinessGroupId = PG.BusinessGroupId
	LEFT JOIN dbo.BusinessType AS BT ON BT.BusinessTypeId = Pro.BusinessTypeId 
	WHERE Pro.[Rank] = 1
	ORDER BY Pro.ProjectNumber,ART.Name,AT.Name,Pro.TargetName,Pro.StartDate,Pro.Percentage

END

