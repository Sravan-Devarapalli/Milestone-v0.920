﻿CREATE PROCEDURE [dbo].[GetProjectsListWithFilters]
(
	@ClientIds			NVARCHAR(MAX) = NULL,
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowProposed		BIT = 0,
	@ShowInactive		BIT = 0,
	@SalespersonIds		NVARCHAR(MAX) = NULL,
	@ProjectOwnerIds	NVARCHAR(MAX) = NULL,
	@PracticeIds		NVARCHAR(MAX) = NULL,
	@ProjectGroupIds	NVARCHAR(MAX) = NULL,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@UserLogin			NVARCHAR(255)
)
AS
	SET NOCOUNT ON ;

	-- Convert client ids from string to TABLE
	DECLARE @ClientsList TABLE (Id INT)
	INSERT INTO @ClientsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert practice ids from string to TABLE
	DECLARE @PracticesList TABLE (Id INT)
	INSERT INTO @PracticesList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	-- Convert project group ids from string to TABLE
	DECLARE @ProjectGroupsList TABLE (Id INT)
	INSERT INTO @ProjectGroupsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIds)

	-- Convert project owner ids from string to TABLE
	DECLARE @ProjectOwnersList TABLE (Id INT)
	INSERT INTO @ProjectOwnersList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIds)

	-- Convert salesperson ids from string to TABLE
	DECLARE @SalespersonsList TABLE (Id INT)
	INSERT INTO @SalespersonsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId=[dbo].[GetDefaultPTOProject]()

	DECLARE @UserHasHighRoleThanProjectLead INT = NULL
	DECLARE @PersonId INT
	
	SELECT @PersonId = P.PersonId
	FROM Person P
	WHERE P.Alias = @UserLogin
	
	--Adding this as part of #2941.
	IF EXISTS ( SELECT 1
				FROM aspnet_Users U
				JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
				JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
				WHERE U.UserName = @UserLogin AND R.LoweredRoleName = 'project lead')
	BEGIN
		SET @UserHasHighRoleThanProjectLead = 0
		
		SELECT @UserHasHighRoleThanProjectLead = COUNT(*)
		FROM aspnet_Users U
		JOIN aspnet_UsersInRoles UR ON UR.UserId = U.UserId
		JOIN aspnet_Roles R ON R.RoleId = UR.RoleId
		WHERE U.UserName = @UserLogin
			AND R.LoweredRoleName IN ('system administrator','client director','business unit manager','salesperson','practice area manager','senior leadership','operations')			
	END

	SELECT	P.ProjectId,
			P.ProjectNumber,
			P.Name AS ProjectName,
			P.StartDate,
			P.EndDate,
			P.ClientId,
			Clnt.Name AS ClientName,
			P.PracticeId,
			pr.Name AS PracticeName,
			P.ProjectStatusId,
			s.Name AS ProjectStatusName,
			P.ProjectManagerId,
			Powner.LastName+', '+ISNULL(Powner.PreferredFirstName,Powner.FirstName) AS ProjectManagerName,
			P.ExecutiveInChargeId,
			ExcPerson.LastName+', '+ ISNULL(ExcPerson.PreferredFirstName,ExcPerson.FirstName) AS ExecutiveInCharge,
			BG.BusinessGroupId,
			BG.Name AS BusinessGroupName,
			p.GroupId AS BusinessUnitId,
			PG.Name AS BusinessUnitName,
			dbo.GetProjectCapabilitiesNames(P.ProjectId) AS ProjectCapabilities,
			[dbo].[GetProjectCapabilities](p.ProjectId) AS ProjectCapabilityIds
	FROM Project AS P
	INNER JOIN dbo.Client AS Clnt ON Clnt.ClientId=P.ClientId
	INNER JOIN dbo.Practice pr ON pr.PracticeId = P.PracticeId
	INNER JOIN dbo.ProjectStatus AS s ON P.ProjectStatusId = s.ProjectStatusId
	LEFT JOIN dbo.Person AS Powner ON Powner.PersonId = P.ProjectManagerId
	LEFT JOIN dbo.Person AS ExcPerson ON ExcPerson.PersonId = P.ExecutiveInChargeId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = P.GroupId
	LEFT JOIN dbo.BusinessGroup AS BG ON PG.BusinessGroupId=BG.BusinessGroupId
	WHERE ((@StartDate <= P.EndDate AND P.StartDate <= @EndDate) OR (P.StartDate IS NULL AND P.EndDate IS NULL))
		  AND ( @ClientIds IS NULL OR P.ClientId IN (SELECT Id FROM @ClientsList))
		  AND ( @ProjectGroupIds IS NULL OR P.GroupId IN (SELECT Id FROM @ProjectGroupsList))
		  AND ( @PracticeIds IS NULL OR P.PracticeId IN (SELECT Id FROM @PracticesList))
		  AND ( @ProjectOwnerIds IS NULL 
			     OR EXISTS (SELECT 1 FROM dbo.ProjectAccess AS projManagers
							JOIN @ProjectOwnersList POL ON POL.Id = projManagers.ProjectAccessId
							WHERE projManagers.ProjectId = P.ProjectId
						   )
			  )
		  AND ( @SalespersonIds IS NULL OR P.SalesPersonId IN (SELECT * FROM @SalespersonsList))
		  AND ( ( @ShowProjected = 1 AND P.ProjectStatusId = 2 ) --Projected
				OR ( @ShowActive = 1 AND P.ProjectStatusId = 3 )  --Active
				OR ( @ShowCompleted = 1 AND P.ProjectStatusId = 4 ) --Completed
				OR ( @showInternal = 1 AND P.ProjectStatusId = 6 ) -- Internal
				OR ( @ShowExperimental = 1 AND P.ProjectStatusId = 5 ) --Experimental
				OR ( @ShowProposed = 1 AND P.ProjectStatusId = 7 ) -- Proposed
				OR ( @ShowInactive = 1 AND P.ProjectStatusId = 1 ) -- Inactive
			  )
		 AND (@UserHasHighRoleThanProjectLead IS NULL
					OR @UserHasHighRoleThanProjectLead > 0
					OR (@UserHasHighRoleThanProjectLead = 0
						AND ( EXISTS (SELECT 1 FROM dbo.ProjectAccess projManagers2 WHERE projManagers2.ProjectId = P.ProjectId AND projManagers2.ProjectAccessId = @PersonId) OR P.SalesPersonId = @PersonId)
						)
			  )
		AND P.IsAllowedToShow = 1
		AND P.ProjectId <> @DefaultProjectId
	ORDER BY P.ProjectNumber





