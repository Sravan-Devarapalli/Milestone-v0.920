CREATE PROCEDURE dbo.PersonListSalesperson
    (
      @IncludeInactive BIT ,
      @PersonId INT = NULL
    )
AS 
BEGIN
    SET NOCOUNT ON;
	/*
		Showing Sales Person of the Projects, Where the Logged in User is Project Manager OR Sales Person of 
		the Project(applicable only for Project Lead role Person) as part of #2941.
	*/

	DECLARE @UserHasHighRoleThanProjectLead INT = NULL
	--Adding Project Lead as per #2941.
	IF @PersonId IS NOT NULL AND EXISTS ( SELECT 1
				FROM aspnet_Users U
				JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
				JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
				JOIN Person P ON P.Alias = U.UserName
				WHERE P.PersonId = @PersonId AND R.LoweredRoleName = 'project lead')
	BEGIN
		SET @UserHasHighRoleThanProjectLead = 0
		
		SELECT @UserHasHighRoleThanProjectLead = COUNT(*)
		FROM aspnet_Users U
		JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
		JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
		JOIN Person P ON P.Alias = U.UserName
		WHERE P.PersonId = @PersonId
			AND R.LoweredRoleName IN ('administrator','client director','practice area manager','senior leadership')		
	END

	IF @UserHasHighRoleThanProjectLead = 0--Adding Project Lead as per #2941.
	BEGIN

		SELECT DISTINCT SP.PersonId ,
				SP.FirstName ,
				SP.LastName ,
				SP.PTODaysPerAnnum ,
				SP.HireDate ,
				SP.TerminationDate ,
				SP.Alias ,
				SP.DefaultPractice ,
				SP.PracticeName ,
				SP.PersonStatusId ,
				SP.PersonStatusName ,
				SP.EmployeeNumber ,
				SP.SeniorityId ,
				SP.SeniorityName ,
				SP.ManagerId ,
				SP.ManagerAlias ,
				SP.ManagerFirstName ,
				SP.ManagerLastName ,
				SP.PracticeOwnedId ,
				SP.PracticeOwnedName,
				SP.TelephoneNumber
		FROM Project P
		LEFT JOIN ProjectManagers PM ON PM.ProjectId = P.ProjectId
		LEFT JOIN Commission C ON C.ProjectId = P.ProjectId AND C.CommissionType = 1
		JOIN v_Person SP ON SP.PersonId = C.PersonId
		WHERE (PM.ProjectManagerId = @PersonId OR C.PersonId = @PersonId) --Logged in User should be Project Manager or Sales Person of the Project.
			AND (SP.PersonStatusId = 1 /* Active person only */
					OR @IncludeInactive = 1)

	END
	ELSE
	BEGIN

	   DECLARE @Now DATETIME
		SET @Now = dbo.Today()

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
						AND prm.PermissionTypeId = 3
                    
		-- Table variable to store list of SalesPersons that owner is allowed to see	
		DECLARE @OwnerProjectSalesPersonList TABLE(
			PersonId INT NULL -- As per #2890
		)
	
		-- If there are nulls in permission table, it means that eveything is allowed to be seen,
		--		so set @PersonId to NULL which will extract all records from the table
		DECLARE @NullsNumber INT
		SELECT  @NullsNumber = COUNT(*)
		FROM    @SalespersonPermissions AS sp
		WHERE   sp.PersonId IS NULL 

		IF @NullsNumber > 0 
		BEGIN
			SET @PersonId = NULL
		END
		ELSE
		BEGIN
			INSERT INTO @OwnerProjectSalesPersonList (PersonId) 
			SELECT DISTINCT vppc.PersonId
			FROM v_PersonProjectCommission AS vppc 
			INNER JOIN  dbo.Project AS proj  ON proj.ProjectId = vppc.ProjectId
			INNER JOIN  dbo.ProjectManagers AS projManagers  ON proj.ProjectId = projManagers.ProjectId
			WHERE projManagers.ProjectManagerId = @PersonId or (vppc.PersonId = @PersonId AND vppc.CommissionType = 1)
		END

			;WITH    Salespersons
					  AS ( SELECT   p.PersonId ,
									p.FirstName ,
									p.LastName ,
									p.PTODaysPerAnnum ,
									p.HireDate ,
									p.TerminationDate ,
									p.Alias ,
									p.DefaultPractice ,
									p.PracticeName ,
									p.PersonStatusId ,
									p.PersonStatusName ,
									p.EmployeeNumber ,
									p.SeniorityId ,
									p.SeniorityName ,
									p.ManagerId ,
									p.ManagerAlias ,
									p.ManagerFirstName ,
									p.ManagerLastName ,
									p.PracticeOwnedId ,
									p.PracticeOwnedName,
									p.TelephoneNumber
						   FROM     dbo.v_Person AS p
						   WHERE    ( p.PersonStatusId = 1 /* Active person only */
									  OR @IncludeInactive = 1
									)
									AND ( @PersonId IS NULL
										  OR p.PersonId IN (
										  SELECT sp.PersonId
										  FROM   @SalespersonPermissions AS sp)
										  OR p.PersonId IN (
										  SELECT    opsp.PersonId
										  FROM      @OwnerProjectSalesPersonList AS opsp )
										)
									AND EXISTS ( SELECT 1
												 FROM   dbo.DefaultCommission AS c
												 WHERE  p.PersonId = c.PersonId
														AND [type] = 1
														AND ( @IncludeInactive = 1
															  OR ( @Now >= c.StartDate
																  AND @Now < c.EndDate
																 )
															) )
						   UNION ALL
						   SELECT   p.PersonId ,
									p.FirstName ,
									p.LastName ,
									p.PTODaysPerAnnum ,
									p.HireDate ,
									p.TerminationDate ,
									p.Alias ,
									p.DefaultPractice ,
									p.PracticeName ,
									p.PersonStatusId ,
									p.PersonStatusName ,
									p.EmployeeNumber ,
									p.SeniorityId ,
									p.SeniorityName ,
									p.ManagerId ,
									p.ManagerAlias ,
									p.ManagerFirstName ,
									p.ManagerLastName ,
									p.PracticeOwnedId ,
									p.PracticeOwnedName,
									p.TelephoneNumber
						   FROM     dbo.v_Person AS p
									INNER JOIN dbo.aspnet_Users AS u ON p.Alias = u.UserName
									INNER JOIN dbo.aspnet_UsersInRoles AS ur ON u.UserId = ur.UserId
									INNER JOIN dbo.aspnet_Roles AS r ON ur.RoleId = r.RoleId
						   WHERE    r.RoleName = 'Salesperson' AND 
									(p.PersonStatusId = 1 OR p.PersonStatusId = 3)
									AND ( @PersonId IS NULL
										  OR p.PersonId IN (
										  SELECT sp.PersonId
										  FROM   @SalespersonPermissions AS sp)
										  OR p.PersonId IN (
										  SELECT    opsp.PersonId
										  FROM      @OwnerProjectSalesPersonList AS opsp )
										)
						 )
			SELECT  DISTINCT *
			FROM    Salespersons AS s
			ORDER BY s.LastName ,
					s.FirstName
	END
END
