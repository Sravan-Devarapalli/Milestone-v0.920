CREATE PROCEDURE dbo.PersonListProjectOwner
(
	@IncludeInactive   BIT,
	@PersonId INT = NULL
)
AS
	SET NOCOUNT ON
	
	/*
		Showing Project Owner of the Projects, Where the Logged in User is Project Manager OR Sales Person of 
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
		FROM Project Proj
		LEFT JOIN ProjectManagers PM ON PM.ProjectId = Proj.ProjectId
		LEFT JOIN Commission C ON C.ProjectId = Proj.ProjectId AND C.CommissionType = 1
		JOIN Person Pers ON PM.ProjectManagerId = Pers.PersonId
		WHERE (PM.ProjectManagerId = @PersonId OR C.PersonId = @PersonId)
			AND (@IncludeInactive = 1 OR pers.PersonStatusId != 4)

	END
	ELSE
	BEGIN

		-- Table variable to store list of ProjectManagers user is allowed to see	
		DECLARE @ProjectManagersPermissions TABLE ( PersonId INT NULL )
		-- Populate is with the data from the permissions table
		--		TargetType = 4 means that we are looking for the ProjectManagers in the permissions table
		INSERT  INTO @ProjectManagersPermissions
				(PersonId)
				SELECT  prm.TargetId
				FROM    v_permissions AS prm
				WHERE   prm.PersonId = @PersonId
						AND prm.PermissionTypeId = 4
	
		-- If there are nulls in permission table, it means that eveything is allowed to be seen,
		--		so set @PersonId to NULL which will extract all records from the table
		DECLARE @NullsNumber INT
		SELECT  @NullsNumber = COUNT(*)
		FROM    @ProjectManagersPermissions AS pmp
		WHERE   pmp.PersonId IS NULL 

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
		INNER JOIN dbo.ProjectManagers AS projManagers ON projManagers.ProjectId = proj.ProjectId
		INNER JOIN dbo.Person AS pers ON projManagers.ProjectManagerId = pers.PersonId
		LEFT JOIN dbo.Commission AS C ON C.ProjectId = proj.ProjectId AND C.CommissionType = 1
		WHERE (@IncludeInactive = 1 OR pers.PersonStatusId != 4)
				AND ( @PersonId IS NULL
					  OR pers.PersonId IN (
					  SELECT    *
					  FROM      @ProjectManagersPermissions )
					   OR projManagers.ProjectManagerId  = @PersonId
					   OR C.PersonId = @PersonId
					)
		order by pers.lastname, pers.firstname
	END

