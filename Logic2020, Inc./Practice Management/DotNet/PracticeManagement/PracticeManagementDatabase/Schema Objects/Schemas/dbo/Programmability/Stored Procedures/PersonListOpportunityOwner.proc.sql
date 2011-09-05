CREATE PROCEDURE [dbo].[PersonListOpportunityOwner]
(
	@IncludeInactive   BIT,
	@PersonId INT = NULL
)
AS
BEGIN
	SET NOCOUNT ON;
	
    DECLARE @PracticeManagerPermissions TABLE ( PersonId INT NULL )
	-- Populate is with the data from the permissions table
    INSERT  INTO @PracticeManagerPermissions
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
    FROM    @PracticeManagerPermissions AS sp
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
	FROM dbo.Opportunity AS op
	INNER JOIN dbo.Person AS pers ON op.OwnerId = pers.PersonId
	WHERE (@IncludeInactive = 1 OR pers.PersonStatusId != 4)
            AND ( @PersonId IS NULL
                  OR pers.PersonId IN (
                  SELECT    *
                  FROM      @PracticeManagerPermissions )
				   OR op.OwnerId = @PersonId
                )
	ORDER BY pers.lastname, pers.firstname

END
