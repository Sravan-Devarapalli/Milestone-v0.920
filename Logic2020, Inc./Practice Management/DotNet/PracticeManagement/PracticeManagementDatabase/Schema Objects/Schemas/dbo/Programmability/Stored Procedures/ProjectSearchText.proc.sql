CREATE PROCEDURE  [dbo].[ProjectSearchText]
(
	@Looked              NVARCHAR(255),
	@PersonId			 INT,
	@ClientIds			VARCHAR(8000) = NULL,
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowProposed		BIT=0,
	@ShowInactive		BIT = 0,
	@ShowAtRisk			BIT = 0,
	@SalespersonIds		VARCHAR(8000) = NULL,
	@ProjectOwnerIds	VARCHAR(8000) = NULL,
	@PracticeIds		VARCHAR(8000) = NULL,
	@DivisionIds		NVARCHAR(MAX) = NULL,
	@ChannelIds			NVARCHAR(MAX) = NULL,
	@RevenueTypeIds		NVARCHAR(MAX) = NULL,
	@OfferingIds		NVARCHAR(MAX) = NULL,
	@ProjectGroupIds	VARCHAR(8000) = NULL
)
AS
	SET NOCOUNT ON

	DECLARE @PersonRole BIT=0
	
	IF EXISTS(SELECT 1 FROM v_UsersInRoles AS uir
    WHERE uir.PersonId = @PersonId and (uir.RoleName = 'System Administrator' OR uir.RoleName = 'Operations'))
	BEGIN
		SET @PersonRole = 1
	END
	
	IF @PersonRole = 1 
		SET @PersonId = null

	DECLARE @SearchText NVARCHAR(257)
	SET @SearchText = '%' +  Replace(ISNULL(@Looked, ''),' ','%') + '%'

	
	/*
		Showing client of the Projects, Where the Logged in User is Project Manager OR Sales Person of 
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
			AND R.LoweredRoleName IN ('system administrator','client director','business unit manager','salesperson','practice area manager','senior leadership','operations')		
	END


		-- Convert client ids from string to table
		DECLARE @ClientsList TABLE (Id INT)
		INSERT INTO @ClientsList
		SELECT TargetId FROM dbo.PersonClientPermissions(@PersonId)

		-- Convert practice ids from string to table
		DECLARE @PracticesList TABLE (Id INT)
		INSERT INTO @PracticesList
		SELECT TargetId FROM dbo.PersonPracticePermissions(@PersonId)

		-- Convert project group ids from string to table
		DECLARE @ProjectGroupsList TABLE (Id INT)
		INSERT INTO @ProjectGroupsList
		SELECT TargetId FROM dbo.PersonProjectGroupPermissions(@PersonId)

		-- Convert project group ids from string to table
		DECLARE @ProjectOwnerList TABLE (Id INT)
		INSERT INTO @ProjectOwnerList
		SELECT TargetId FROM dbo.PersonProjectOwnerPermissions(@PersonId)

		DECLARE @DefaultProjectId INT
		SELECT @DefaultProjectId = ProjectId
		FROM dbo.DefaultMilestoneSetting

			-- Table variable to store list of Clients that owner and salesPerson is allowed to see	
		DECLARE @OwnerProjectClientList TABLE(
			ClientId INT NULL -- As per #2890
		)

	   -- Table variable to store list of groups that owner and salesPerson is allowed to see	
		DECLARE @OwnerProjectGroupList TABLE(
			GroupId INT NULL -- As per #2890
		)

		-- Populate is with the data from the Project
		INSERT INTO @OwnerProjectClientList (ClientId) 
		SELECT proj.ClientId 
		FROM dbo.Project AS proj
		INNER JOIN ProjectAccess AS projManagers ON projManagers.ProjectId = proj.ProjectId
		WHERE projManagers.ProjectAccessId = @PersonId 
				OR proj.SalesPersonId = @PersonId -- Adding Salesperson - Project clients into the list.
				OR proj.ProjectManagerId = @PersonId

		INSERT INTO @OwnerProjectGroupList (GroupId) 
		SELECT GroupId FROM  dbo.ProjectGroup
		WHERE ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc)


		-- Convert client ids from string to TABLE
	DECLARE @SelectedClientsList TABLE (Id INT)
	INSERT INTO @SelectedClientsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert practice ids from string to TABLE
	DECLARE @SelectedPracticesList TABLE (Id INT)
	INSERT INTO @SelectedPracticesList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	-- Convert division ids from string to TABLE
	DECLARE @SelectedDivisionsList TABLE (Id INT)
	INSERT INTO @SelectedDivisionsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@DivisionIds)

	-- Convert practice ids from string to TABLE
	DECLARE @SelectedChannelsList TABLE (Id INT)
	INSERT INTO @SelectedChannelsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ChannelIds)

	-- Convert practice ids from string to TABLE
	DECLARE @SelectedRevenueTypesList TABLE (Id INT)
	INSERT INTO @SelectedRevenueTypesList
	SELECT * FROM dbo.ConvertStringListIntoTable(@RevenueTypeIds)

	-- Convert practice ids from string to TABLE
	DECLARE @SelectedOfferingsList TABLE (Id INT)
	INSERT INTO @SelectedOfferingsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@OfferingIds)

	-- Convert project group ids from string to TABLE
	DECLARE @SelectedProjectGroupsList TABLE (Id INT)
	INSERT INTO @SelectedProjectGroupsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIds)

	-- Convert project owner ids from string to TABLE
	DECLARE @SelectedProjectOwnersList TABLE (Id INT)
	INSERT INTO @SelectedProjectOwnersList
	SELECT * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIds)

	-- Convert salesperson ids from string to TABLE
	DECLARE @SelectedSalespersonsList TABLE (Id INT)
	INSERT INTO @SelectedSalespersonsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)


		-- Search for a project with milestone(s)
		;WITH FoundProjects AS (
		SELECT m.ClientId,
			   m.ProjectId,
			   m.MilestoneId,
			   m.StartDate AS MilestoneStartDate,
			   m.ClientName,
			   m.ProjectName,
			   m.Description,
			   m.ProjectStartDate,
			   m.ProjectEndDate,
			   m.ProjectNumber,
			   m.ProjectStatusId,
			   s.Name AS ProjectStatusName,
			   m.GroupId,
			   CASE WHEN A.ProjectId IS NOT NULL THEN 1 
						ELSE 0 END AS HasAttachments,
			   m.ProjectManagerId AS ProjectOwnerId,
			   m.SalesPersonId,
			   m.PONumber
		  FROM dbo.v_Milestone AS m
			   INNER JOIN ProjectAccess AS projManagers ON projManagers.ProjectId = m.ProjectId AND m.IsAllowedToShow = 1
			   INNER JOIN dbo.ProjectStatus AS s ON m.ProjectStatusId = s.ProjectStatusId
			   INNER JOIN dbo.Project as p ON p.ProjectId=m.ProjectId
		  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = m.ProjectId) A
		 WHERE ( 
				@Looked IS NULL OR
				m.ProjectName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR m.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR m.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR m.Description LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR m.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR m.PONumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
			   )
			AND (@PersonId is NULL OR m.ClientId IN (select * from @ClientsList) OR  m.ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc))
			AND (@PersonId is NULL OR m.GroupId IN (select * from @ProjectGroupsList) OR  m.GroupId IN (SELECT opG.GroupId FROM @OwnerProjectGroupList AS opG))
		    AND ( @ClientIds IS NULL OR P.ClientId IN (SELECT * from @SelectedClientsList) )
			AND ( @ProjectGroupIds IS NULL OR P.GroupId IN (SELECT * FROM @SelectedProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR P.PracticeId IN (SELECT * FROM @SelectedPracticesList) OR P.PracticeId IS NULL )
		    AND ( @DivisionIds IS NULL OR P.DivisionId IN (SELECT * FROM @SelectedDivisionsList))
		    AND ( @ChannelIds IS NULL OR P.ChannelId IN (SELECT * FROM @SelectedChannelsList))
		    AND ( @RevenueTypeIds IS NULL OR P.RevenueTypeId IN (SELECT * FROM @SelectedRevenueTypesList))
		    AND ( @OfferingIds IS NULL OR P.OfferingId IN (SELECT * FROM @SelectedOfferingsList))
		    AND (@SalespersonIds IS NULL OR P.SalesPersonId IN (SELECT * FROM @SelectedSalespersonsList))
			   AND (@PersonId is NULL OR projManagers.ProjectAccessId = @PersonId OR m.SalesPersonId= @PersonId OR projManagers.ProjectAccessId in (select * from @ProjectOwnerList) 
									  OR m.ProjectManagerId = @PersonId OR m.ProjectManagerId in (select * from @ProjectOwnerList) )   
		UNION ALL
		SELECT p.ClientId,
			   p.ProjectId,
			   NULL AS MilestoneId,
			   NULL AS MilestoneStartDate,
			   p.ClientName,
			   p.Name AS ProjectName,
			   NULL AS Description,
			   NULL AS ProjectStartDate,
			   NULL AS ProjectEndDate,
			   p.ProjectNumber,
			   p.ProjectStatusId,
			   p.ProjectStatusName,
			   p.GroupId,
			   CASE WHEN A.ProjectId IS NOT NULL THEN 1 
						ELSE 0 END AS HasAttachments,
			   p.ProjectManagerId,
			   p.SalesPersonId,
			   p.PONumber
		  FROM dbo.v_Project AS p
		  INNER JOIN ProjectAccess AS projManagers ON projManagers.ProjectId = p.ProjectId AND p.IsAllowedToShow = 1
		  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
		 WHERE NOT EXISTS (SELECT 1 FROM dbo.Milestone AS m WHERE m.ProjectId = p.ProjectId)
		   AND (   
				@Looked IS NULL OR
				p.Name LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR p.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR p.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR p.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
				OR p.PONumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
			   )
			  
		  AND (@PersonId is NULL OR p.ClientId in (SELECT * FROM @ClientsList) OR  p.ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc))
		  AND (@PersonId is NULL OR p.GroupId in (SELECT * FROM @ProjectGroupsList) OR  P.GroupId IN (SELECT opG.GroupId FROM @OwnerProjectGroupList AS opG))
		  AND (@PersonId is NULL OR p.PracticeId in (SELECT * FROM @PracticesList))	
		  AND ( @ClientIds IS NULL OR P.ClientId IN (SELECT * from @SelectedClientsList) )
		  AND ( @ProjectGroupIds IS NULL OR P.GroupId IN (SELECT * FROM @SelectedProjectGroupsList) )
		  AND ( @PracticeIds IS NULL OR P.PracticeId IN (SELECT * FROM @SelectedPracticesList) OR P.PracticeId IS NULL )
		  AND ( @DivisionIds IS NULL OR P.DivisionId IN (SELECT * FROM @SelectedDivisionsList))
		  AND ( @ChannelIds IS NULL OR P.ChannelId IN (SELECT * FROM @SelectedChannelsList))
		  AND ( @RevenueTypeIds IS NULL OR P.RevenueTypeId IN (SELECT * FROM @SelectedRevenueTypesList))
		  AND ( @OfferingIds IS NULL OR P.OfferingId IN (SELECT * FROM @SelectedOfferingsList))
		  AND (@SalespersonIds IS NULL OR P.SalesPersonId IN (SELECT * FROM @SelectedSalespersonsList))
		  AND (@PersonId is NULL OR projManagers.ProjectAccessId = @PersonId  OR p.SalesPersonId = @PersonId OR projManagers.ProjectAccessId in (SELECT * FROM @ProjectOwnerList)
								  OR p.ProjectManagerId = @PersonId OR p.ProjectManagerId in (select * from @ProjectOwnerList))
		)
		SELECT DISTINCT FP.*
		FROM FoundProjects FP
		LEFT JOIN ProjectAccess PM ON PM.ProjectId = FP.ProjectId
		WHERE FP.ProjectId <> @DefaultProjectId
			AND ( @UserHasHighRoleThanProjectLead IS NULL 
					OR @UserHasHighRoleThanProjectLead <> 0
					OR (@UserHasHighRoleThanProjectLead = 0 AND (PM.ProjectAccessId = @PersonId OR FP.SalesPersonId = @PersonId OR FP.ProjectOwnerId = @PersonId ))
				)
			AND (    ( @ShowProjected = 1 AND FP.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND FP.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND FP.ProjectStatusId = 4 )
				  OR ( @showInternal = 1 AND FP.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimental = 1 AND FP.ProjectStatusId = 5 ) 
				  OR ( @ShowProposed = 1 AND FP.ProjectStatusId = 7 ) -- Proposed
				  OR ( @ShowInactive = 1 AND FP.ProjectStatusId = 1 ) -- Inactive
				  OR ( @ShowAtRisk = 1 AND FP.ProjectStatusId=8)
			)
		ORDER BY FP.ProjectName, FP.MilestoneStartDate

