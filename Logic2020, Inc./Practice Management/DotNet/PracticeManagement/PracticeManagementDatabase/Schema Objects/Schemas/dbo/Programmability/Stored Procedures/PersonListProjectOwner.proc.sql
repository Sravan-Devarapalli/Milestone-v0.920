CREATE PROCEDURE dbo.PersonListProjectOwner
(
	@EndDate           DATETIME,
	@IncludeInactive   BIT,
	@PersonId INT = NULL
)
AS
	SET NOCOUNT ON

	-- Table variable to store list of salespersons user is allowed to see	
    DECLARE @SalespersonPermissions TABLE ( PersonId INT NULL )
	-- Populate is with the data from the permissions table
	--		TargetType = 3 means that we are looking for the salespersons in the permissions table
    INSERT  INTO @SalespersonPermissions
            ( PersonId
	      )
            SELECT  prm.TargetId
            FROM    v_permissions AS prm
            WHERE   prm.PersonId = @PersonId
                    AND prm.PermissionTypeId = 4
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
    DECLARE @NullsNumber INT
    SELECT  @NullsNumber = COUNT(*)
    FROM    @SalespersonPermissions AS sp
    WHERE   sp.PersonId IS NULL 

    IF @NullsNumber > 0 
        SET @PersonId = NULL

	SELECT  DISTINCT 
			pers.PersonId ,
			pers.PTODaysPerAnnum ,
	        pers.HireDate ,
	        pers.TerminationDate,
	        pers.TelephoneNumber,
	        pers.DefaultPractice,
	        'Unknown' AS 'PracticeName',
	        pers.Alias ,
	        pers.FirstName ,
	        pers.LastName ,
	        pers.PersonStatusId ,
	        'Unknown' AS 'PersonStatusName',
	        pers.EmployeeNumber ,
	        pers.SeniorityId,
	        'Unknown' AS 'SeniorityName',
	        pers.ManagerId,
	        'Unknown' AS 'ManagerFirstName',	-- just stubs 
	        'Unknown' AS 'ManagerLastName'	-- just stubs
	FROM dbo.Project AS proj
	INNER JOIN dbo.Person AS pers ON proj.ProjectManagerId = pers.PersonId
	WHERE --(@EndDate BETWEEN proj.StartDate AND proj.EndDate) AND 
			(@IncludeInactive = 1 OR pers.PersonStatusId != 4)
            AND ( @PersonId IS NULL
                  OR pers.PersonId IN (
                  SELECT    *
                  FROM      @SalespersonPermissions )
                )
	order by pers.lastname, pers.firstname

