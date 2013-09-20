﻿CREATE PROCEDURE dbo.PersonListProjectOwner
(
	@PersonId INT = NULL
)
AS
	SET NOCOUNT ON
	
	/*
		Showing Project Owner of the Projects, Where the Logged in User is Project Manager OR Sales Person of 
		the Project(applicable only for Project Lead role Person) as part of #2941.

		If @PersonId != null	
	    1.Checks whether person have 'only' project lead role.
			If Yes:Lists the Projectmanagers of the projects where the @PersonId is assigned as ProjectOwner or ProjectManager or ProjectSalesPerson
			Else : Lists the the persons  on whom(project managers) he has permissions(which appears in Permissions tab of Persondetail page.) or
			      projectmanagers of the projects where the person is assigned as ProjectOwner or ProjectManager or ProjectSalesPerson
	Otherwise
		1.Lists all the persons who projectmanagers of the projects where the person is assigned as ProjectOwner or ProjectManager or ProjectSalesPerson  
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
			AND R.LoweredRoleName IN ('administrator','client director','business unit manager','salesperson','practice area manager','senior leadership')		
	END

	IF @UserHasHighRoleThanProjectLead = 0--Adding Project Lead as per #2941.
	BEGIN
		
		SELECT  DISTINCT 
				pers.PersonId ,
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
		FROM dbo.Project Proj
		INNER JOIN dbo.ProjectManagers PM ON PM.ProjectId = Proj.ProjectId
		INNER JOIN dbo.Person Pers ON PM.ProjectManagerId = Pers.PersonId
		WHERE (PM.ProjectManagerId = @PersonId OR Proj.SalesPersonId = @PersonId OR proj.projectOwnerId = @PersonId )
			

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
		INNER JOIN dbo.Person AS pers ON ( projManagers.ProjectManagerId = pers.PersonId OR  proj.projectownerId = pers.PersonId )
		WHERE ( @PersonId IS NULL
					  OR pers.PersonId IN (
					  			SELECT    *
					  			FROM      @ProjectManagersPermissions 
							       )
					   OR pers.PersonId = @PersonId
					   OR proj.SalesPersonId = @PersonId
					)
		order by pers.lastname, pers.firstname
	END

