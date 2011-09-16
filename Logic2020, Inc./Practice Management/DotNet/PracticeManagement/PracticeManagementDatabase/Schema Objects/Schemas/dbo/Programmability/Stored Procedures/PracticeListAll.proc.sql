CREATE PROCEDURE dbo.PracticeListAll
(
	@PersonId		   INT = NULL
)
AS
	SET NOCOUNT ON;
	
-- Table variable to store list of practices user is allowed to see	
	DECLARE @PracticePermissions TABLE(
		PracticeId INT NULL
	)
	-- Populate is with the data from the permissions table
	--		TargetType = 5 means that we are looking for the practices in the permissions table
	INSERT INTO @PracticePermissions (
		PracticeId
	) SELECT prm.TargetId FROM dbo.Permission AS prm WHERE prm.PersonId = @PersonId AND prm.TargetType = 5
	
	-- Table variable to store list of Practices that owner is allowed to see	
	DECLARE @OwnerProjectPracticeList TABLE(
		PracticeId INT NULL -- As per #2890
	)
	
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @PracticePermissions AS sp WHERE sp.PracticeId IS NULL 	
	IF @NullsNumber > 0 
	BEGIN
		SET @PersonId = NULL
	END
	ELSE
	BEGIN
		-- Populate is with the data from the v_Project 
		INSERT INTO @OwnerProjectPracticeList (PracticeId) 
		SELECT proj.PracticeId 
		FROM dbo.v_Project AS proj 
		LEFT JOIN dbo.Commission AS C ON C.ProjectId = proj.ProjectId AND C.CommissionType = 1 --as per #2914
		WHERE proj.ProjectManagerId = @PersonId OR C.PersonId = @PersonId
	END
	
	SELECT 
		p.PracticeId, 
		p.Name,
		p.IsActive,
		p.IsCompanyInternal,
		p.IsNotesRequired,
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
			p.PracticeId IN (SELECT PP.PracticeId  FROM @PracticePermissions AS PP)
			
			OR p.PracticeId IN (SELECT OPP.PracticeId FROM @OwnerProjectPracticeList AS OPP)
		)
	ORDER BY p.Name
