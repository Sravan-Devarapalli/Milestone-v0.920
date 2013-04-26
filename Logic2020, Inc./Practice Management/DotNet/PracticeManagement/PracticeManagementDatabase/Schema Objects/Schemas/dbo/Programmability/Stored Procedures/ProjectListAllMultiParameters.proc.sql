---------------------------
-- Updated By: Sainathc
-- Updated Date: 2012-06-05
---------------------------

CREATE PROCEDURE [dbo].[ProjectListAllMultiParameters]
	@ClientIds			VARCHAR(8000) = NULL,
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowInactive		BIT = 0,
	@SalespersonIds		VARCHAR(8000) = NULL,
	@ProjectOwnerIds	VARCHAR(8000) = NULL,
	@PracticeIds		VARCHAR(8000) = NULL,
	@ProjectGroupIds	VARCHAR(8000) = NULL,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@ExcludeInternalPractices BIT = 0,
	@UserLogin			NVARCHAR(255)
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
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting
	
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
			AND R.LoweredRoleName IN ('administrator','client director','business unit manager','salesperson','practice area manager','senior leadership')			
	END
	
	SELECT  P.ClientId,
			P.ProjectId,
			P.Discount,
			P.Terms,
			P.Name,
			pr.PracticeManagerId,
			P.PracticeId,
			P.StartDate,
			P.EndDate,
			Clnt.Name AS ClientName,
			pr.Name AS PracticeName,
			P.ProjectStatusId,
			s.Name AS ProjectStatusName,
			P.ProjectNumber,
			P.BuyerName,
			P.OpportunityId,
			P.GroupId,
			p.BusinessTypeId,
			p.PricingListId,
			PL.Name AS PricingListName,
			BG.BusinessGroupId,
			BG.Name AS BusinessGroupName,
			PG.Name AS GroupName,
			Clnt.IsChargeable AS [ClientIsChargeable],
	        P.IsChargeable AS [ProjectIsChargeable],
		   c.PersonId AS 'SalespersonId',
		   sperson.LastName+', ' +sperson.FirstName AS 'SalespersonName' ,
		   c.CommissionType,
		   P.DirectorId,
		   d.LastName AS 'DirectorLastName',
		   d.FirstName AS 'DirectorFirstName',
		   CASE WHEN A.ProjectId IS NOT NULL THEN 1 
					ELSE 0 END AS HasAttachments,
		   M.MilestoneId,
		   M.Description AS MilestoneName,
		   P.ProjectOwnerId,
		   Powner.LastName AS [ProjectOwnerLastName],
		   Powner.FirstName AS [ProjectOwnerFirstName],
		   dbo.GetProjectManagerList(P.ProjectId) AS ProjectManagersIdFirstNameLastName,
		   P.SowBudget
	FROM	dbo.Project AS P
	INNER JOIN dbo.Practice pr ON pr.PracticeId = P.PracticeId
	INNER JOIN dbo.Client AS Clnt ON P.ClientId = Clnt.ClientId
	INNER JOIN dbo.ProjectStatus AS s ON P.ProjectStatusId = s.ProjectStatusId
	LEFT JOIN dbo.Person as d on d.PersonId = P.DirectorId
	LEFT JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
	LEFT JOIN dbo.Commission AS c on c.ProjectId = P.ProjectId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = P.GroupId
	LEFT JOIN dbo.BusinessGroup AS BG ON PG.BusinessGroupId=BG.BusinessGroupId
	LEFT JOIN dbo.PricingList AS PL ON P.PricingListId=PL.PricingListId 
	LEFT JOIN dbo.Person AS sperson ON sperson.PersonId = c.PersonId
	LEFT JOIN dbo.Person AS Powner ON Powner.PersonId = P.ProjectOwnerId
	OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = P.ProjectId) A
	WHERE	    (c.CommissionType IS NULL OR c.CommissionType = 1)
		    AND ( (P.EndDate >= @StartDate AND P.StartDate <= @EndDate) OR (P.StartDate IS NULL AND P.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR P.ClientId IN (SELECT * from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR P.GroupId IN (SELECT * FROM @ProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR P.PracticeId IN (SELECT * FROM @PracticesList) OR P.PracticeId IS NULL )
			AND ( @ProjectOwnerIds IS NULL 
					OR EXISTS (SELECT 1 FROM dbo.ProjectManagers AS projManagers
								JOIN @ProjectOwnersList POL ON POL.Id = projManagers.ProjectManagerId
									WHERE projManagers.ProjectId = P.ProjectId
							  )
					OR Powner.PersonId IN ( SELECT ID FROM @ProjectOwnersList)
			    )
			AND (    @SalespersonIds IS NULL 
				  OR c.PersonId IN (SELECT * FROM @SalespersonsList)
			)
			AND (    ( @ShowProjected = 1 AND P.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND P.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND P.ProjectStatusId = 4 )
				  OR ( @showInternal = 1 AND P.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimental = 1 AND P.ProjectStatusId = 5 )
				  OR ( @ShowInactive = 1 AND P.ProjectStatusId = 1 ) -- Inactive
			)
			AND  (ISNULL(pr.IsCompanyInternal, 0) = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)
			AND P.ProjectId <> @DefaultProjectId
			AND (@UserHasHighRoleThanProjectLead IS NULL
					OR @UserHasHighRoleThanProjectLead > 0
					OR (@UserHasHighRoleThanProjectLead = 0
						AND EXISTS (SELECT 1 FROM dbo.ProjectManagers projManagers2
									LEFT JOIN dbo.Commission Css ON Css.ProjectId = projManagers2.ProjectId AND Css.CommissionType = 1
									WHERE projManagers2.ProjectId = P.ProjectId
											AND (projManagers2.ProjectManagerId = @PersonId
												OR Css.PersonId = @PersonId
												)
									)
						)
				)
			AND P.IsAllowedToShow = 1
	ORDER BY CASE P.ProjectStatusId
			   WHEN 2 THEN P.StartDate
			   ELSE P.EndDate
			 END

