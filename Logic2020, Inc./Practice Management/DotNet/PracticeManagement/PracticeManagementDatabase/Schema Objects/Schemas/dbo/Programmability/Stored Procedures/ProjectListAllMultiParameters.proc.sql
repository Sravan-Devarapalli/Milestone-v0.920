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

	-- Convert client ids from string to table
	declare @ClientsList table (Id int)
	insert into @ClientsList
	select * FROM dbo.ConvertStringListIntoTable(@ClientIds)

	-- Convert practice ids from string to table
	declare @PracticesList table (Id int)
	insert into @PracticesList
	select * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	-- Convert project group ids from string to table
	declare @ProjectGroupsList table (Id int)
	insert into @ProjectGroupsList
	select * FROM dbo.ConvertStringListIntoTable(@ProjectGroupIds)

	-- Convert project owner ids from string to table
	declare @ProjectOwnersList table (Id int)
	insert into @ProjectOwnersList
	select * FROM dbo.ConvertStringListIntoTable(@ProjectOwnerIds)

	-- Convert salesperson ids from string to table
	declare @SalespersonsList table (Id int)
	insert into @SalespersonsList
	select * FROM dbo.ConvertStringListIntoTable(@SalespersonIds)
	union all
	-- All persons with Role = Salesperson
	select PersonId from v_UsersInRoles where RoleName = 'Salesperson'
	union all
	-- All persons that have commission
	SELECT c.PersonId FROM dbo.DefaultCommission AS c
    WHERE [type] = 1 AND dbo.IsDateRangeWithingTimeInterval(c.StartDate, c.EndDate, @StartDate, @EndDate) = 1
	union all
	-- All persons that have project commission
	SELECT com.PersonId FROM dbo.Commission AS com
    WHERE com.CommissionType = 1
	/*union all
	-- All active persons
	select PersonId from Person where PersonStatusId = 1*/
	
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
			AND R.LoweredRoleName IN ('administrator','client director','practice area manager','senior leadership')		
	END
	
	SELECT  p.ClientId,
			p.ProjectId,
			p.Discount,
			p.Terms,
			p.Name,
			p.PracticeManagerId,
			p.PracticeId,
			p.StartDate,
			p.EndDate,
			p.ClientName,
			p.PracticeName,
			p.ProjectStatusId,
			p.ProjectStatusName,
			p.ProjectNumber,
			p.BuyerName,
			p.OpportunityId,
			p.GroupId,
			PG.Name GroupName,
	       p.ClientIsChargeable,
	       p.ProjectIsChargeable,
		   p.ProjectManagersIdFirstNameLastName,
		   c.PersonId as 'SalespersonId',
		   person.LastName+' , ' +person.FirstName AS 'SalespersonName' ,
		   c.CommissionType,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   CASE WHEN A.ProjectId IS NOT NULL THEN 1 
					ELSE 0 END AS HasAttachments,
		   M.MilestoneId,
		   M.Description MilestoneName
	FROM	dbo.v_Project AS p
	JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	
	LEFT JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
	LEFT JOIN dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	LEFT JOIN Person AS person ON person.PersonId = c.PersonId
	OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
	WHERE	    (c.CommissionType is NULL OR c.CommissionType = 1)
		    AND (dbo.IsDateRangeWithingTimeInterval(p.StartDate, p.EndDate, @StartDate, @EndDate) = 1 OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR p.ClientId IN (select * from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT * from @ProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR p.PracticeId IN (SELECT * FROM @PracticesList) OR p.PracticeId IS NULL )
			AND ( @ProjectOwnerIds IS NULL 
					OR EXISTS (SELECT 1 FROM dbo.ProjectManagers AS projManagers
								JOIN @ProjectOwnersList POL ON POL.Id = projManagers.ProjectManagerId
									WHERE projManagers.ProjectId = p.ProjectId
							  )
			    )
			AND (    @SalespersonIds IS NULL 
				  OR c.PersonId IN (SELECT * FROM @SalespersonsList)
				  OR c.PersonId is null
			)
			AND (    ( @ShowProjected = 1 AND p.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND p.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND p.ProjectStatusId = 4 )
				  OR ( @showInternal = 1 AND p.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimental = 1 AND p.ProjectStatusId = 5 )
				  OR ( @ShowInactive = 1 AND p.ProjectStatusId = 1 ) -- Inactive
			)
			AND  (ISNULL(pr.IsCompanyInternal, 0) = 0 AND @ExcludeInternalPractices  = 1 OR @ExcludeInternalPractices = 0)
			AND P.ProjectId <> @DefaultProjectId
			AND (@UserHasHighRoleThanProjectLead IS NULL
					OR @UserHasHighRoleThanProjectLead > 0
					OR (@UserHasHighRoleThanProjectLead = 0
						AND EXISTS (SELECT 1 FROM dbo.ProjectManagers projManagers2
									LEFT JOIN Commission Css ON Css.ProjectId = projManagers2.ProjectId AND Css.CommissionType = 1
									WHERE projManagers2.ProjectId = p.ProjectId
											AND (projManagers2.ProjectManagerId = @PersonId
												OR Css.PersonId = @PersonId
												)
									)
						)
				)
	ORDER BY CASE p.ProjectStatusId
			   WHEN 2 THEN p.StartDate
			   ELSE p.EndDate
			 END

