CREATE PROCEDURE dbo.PracticeListAll
(
	@PersonId		   INT = NULL
)
AS
	SET NOCOUNT ON
	
	-- Table variable to store list of salespersons user is allowed to see	
	DECLARE @SalespersonPermissions TABLE(
		PersonId INT NULL
	)
	-- Populate is with the data from the permissions table
	--		TargetType = 5 means that we are looking for the practices in the permissions table
	INSERT INTO @SalespersonPermissions (
		PersonId
	) SELECT prm.TargetId FROM dbo.Permission AS prm WHERE prm.PersonId = @PersonId AND prm.TargetType = 5
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @SalespersonPermissions AS sp WHERE sp.PersonId IS NULL 	
	IF @NullsNumber > 0 SET @PersonId = NULL
	
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
			WHEN EXISTS(SELECT TOP 1 pay.PracticeId FROM dbo.Pay pay WHERE pay.PracticeId = p.PracticeId)
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
	  WHERE (
			@PersonId IS NULL 
			OR  
			p.PracticeId IN (SELECT * FROM @SalespersonPermissions)
		)
	ORDER BY p.Name

