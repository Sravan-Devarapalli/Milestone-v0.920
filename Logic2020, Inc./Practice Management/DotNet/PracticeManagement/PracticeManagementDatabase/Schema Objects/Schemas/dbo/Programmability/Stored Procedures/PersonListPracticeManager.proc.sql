CREATE PROCEDURE dbo.PersonListPracticeManager
(
	@ProjectId         INT,
	@EndDate           DATETIME,
	@IncludeInactive   BIT,
	@PersonId		   INT = NULL
)
AS
	SET NOCOUNT ON

	DECLARE @Now DATETIME
	SET @Now = dbo.Today()

	-- Table variable to store list of salespersons user is allowed to see	
	DECLARE @SalespersonPermissions TABLE(
		PersonId INT NULL
	)
	-- Populate is with the data from the permissions table
	--		TargetType = 4 means that we are looking for the practice manager in the permissions table
	INSERT INTO @SalespersonPermissions (
		PersonId
	) SELECT prm.TargetId FROM dbo.Permission AS prm WHERE prm.PersonId = @PersonId AND prm.TargetType = 4
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @SalespersonPermissions AS sp WHERE sp.PersonId IS NULL 
	IF @NullsNumber > 0 SET @PersonId = NULL

	IF @EndDate IS NULL
	BEGIN
		SELECT @EndDate = p.EndDate
		  FROM dbo.Project AS p
		 WHERE p.ProjectId = @ProjectId
	END

	SELECT p.PersonId,
	       p.FirstName,
	       p.LastName,
	       p.PTODaysPerAnnum,
	       p.HireDate,
	       p.TerminationDate,
	       p.Alias,
	       p.DefaultPractice,
	       p.PracticeName,
	       p.PersonStatusId,
	       p.PersonStatusName,
		   p.EmployeeNumber,
	       p.SeniorityId,
	       p.SeniorityName,
	       p.ManagerId,
	       p.ManagerAlias,
	       p.ManagerFirstName,
	       p.ManagerLastName,
	       p.PracticeOwnedId,
	       p.PracticeOwnedName,
	       p.TelephoneNumber
	  FROM dbo.v_Person AS p
	 WHERE (p.PersonStatusId = 1 /* Active person only*/ OR @IncludeInactive = 1)
		AND (
			@PersonId IS NULL 
			OR  
			p.PersonId IN (SELECT * FROM @SalespersonPermissions)
		)
	   AND EXISTS (SELECT 1
	                 FROM dbo.DefaultCommission AS c
	                WHERE p.PersonId = c.PersonId
	                  AND [type] = 2
	                  AND (   @IncludeInactive = 1
	                       OR (@Now >= c.StartDate AND @Now < c.EndDate)))
	   AND (p.TerminationDate IS NULL OR p.TerminationDate > @EndDate OR @IncludeInactive = 1)
	ORDER BY p.LastName, p.FirstName

