CREATE PROCEDURE dbo.PracticeGetById
(
	@PracticeId		   INT = NULL
)
AS
	SET NOCOUNT ON	
	
	SELECT 
		p.PracticeId, 
		p.Name,
		p.IsActive,
		p.IsCompanyInternal,
		CASE 
			WHEN EXISTS(SELECT TOP 1 proj.PracticeId FROM dbo.Project proj WHERE proj.PracticeId = p.PracticeId)
				THEN CAST(1 AS BIT)
			WHEN EXISTS(SELECT TOP 1 pers.DefaultPractice FROM dbo.Person pers WHERE pers.DefaultPractice = p.PracticeId)
				THEN CAST(1 AS BIT)
			WHEN EXISTS(SELECT TOP 1 op.PracticeId FROM dbo.Opportunity op WHERE op.PracticeId = p.PracticeId)
				THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT)
		END AS 'InUse',
		pers.FirstName,
		pers.LastName,
		pers.PersonId,
		pers.PersonStatusId,
		stat.[Name] AS 'PersonStatusName'		
	  FROM dbo.Practice AS p
	  LEFT JOIN dbo.Person AS pers ON p.PracticeManagerId = pers.PersonId
	  INNER JOIN dbo.PersonStatus AS stat ON pers.PersonStatusId = stat.PersonStatusId
	  WHERE @PracticeId IS NULL OR p.PracticeId = @PracticeId
	ORDER BY p.Name
