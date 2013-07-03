CREATE PROCEDURE [dbo].[GetProjectAttributionValues]
	(
		@ProjectId INT
	)
AS
BEGIN
	;WITH ProjectAttributionDetail
	AS
	(
	    SELECT	AT.AttributionTypeId,--sales/delivery
				ART.AttributionRecordId,--person/practice
				A.StartDate,
				A.EndDate,
				A.TargetId,
				ISNULL(P.LastName+', '+P.FirstName,Pr.Name) AS TargetName,
				A.Percentage,
				A.AttributionId,
				pay.TitleId,
				title.Title,
				RANK() OVER (PARTITION BY pay.Person,A.StartDate,A.EndDate ORDER BY pay.StartDate DESC) AS [Rank]
		FROM dbo.Attribution AS A
		INNER JOIN dbo.AttributionTypes AS AT ON A.AttributionTypeId = AT.AttributionTypeId
		INNER JOIN dbo.AttributionRecordTypes AS ART ON ART.AttributionRecordId = A.AttributionRecordTypeId
		LEFT JOIN dbo.Person AS P ON A.AttributionRecordTypeId = 1 AND P.PersonId = A.TargetId
		LEFT JOIN dbo.Practice AS Pr ON A.AttributionRecordTypeId = 2 AND Pr.PracticeId= A.TargetId
		LEFT JOIN dbo.Pay AS pay ON A.AttributionRecordTypeId = 1 AND pay.Person = P.PersonId AND ((pay.EndDate IS NULL OR A.StartDate <= pay.EndDate) AND pay.StartDate <= A.EndDate)
		LEFT JOIN dbo.Title AS title ON A.AttributionRecordTypeId = 1 AND title.TitleId = pay.TitleId
		WHERE A.ProjectId = @ProjectId
	)
	SELECT	AttributionId,
			AttributionTypeId,
			AttributionRecordId,
			StartDate,
			EndDate,
			TargetId,
			TargetName,
			Percentage,
			TitleId,
			Title
	FROM ProjectAttributionDetail Pro WHERE [Rank] = 1
END
