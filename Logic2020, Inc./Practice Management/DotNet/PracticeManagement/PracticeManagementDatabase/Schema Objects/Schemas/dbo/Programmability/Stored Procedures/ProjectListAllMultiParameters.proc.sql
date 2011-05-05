CREATE PROCEDURE [dbo].[ProjectListAllMultiParameters]
	@ClientIds			VARCHAR(250) = NULL,
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowInactive		BIT = 0,
	@SalespersonIds		VARCHAR(250) = NULL,
	@ProjectOwnerIds	VARCHAR(250) = NULL,
	@PracticeIds		VARCHAR(250) = NULL,
	@ProjectGroupIds	VARCHAR(250) = NULL,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@ExcludeInternalPractices BIT = 0
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
		   p.ProjectManagerId,
		   p.ProjectManagerFirstName,
		   p.ProjectManagerLastName,
		   c.PersonId as 'SalespersonId',
		   person.LastName+' , ' +person.FirstName AS 'SalespersonName' ,
		   c.CommissionType,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   pa.[FileName]
	FROM	dbo.v_Project AS p
	JOIN dbo.Practice pr ON pr.PracticeId = p.PracticeId
	LEFT JOIN dbo.v_PersonProjectCommission AS c on c.ProjectId = p.ProjectId
	LEFT JOIN dbo.ProjectGroup PG	ON PG.GroupId = p.GroupId
	LEFT JOIN v_Person AS person ON person.PersonId = c.PersonId
	LEFT JOIN dbo.ProjectAttachment AS pa ON p.ProjectId = pa.ProjectId
	WHERE	    (c.CommissionType is NULL OR c.CommissionType = 1)
		    AND (dbo.IsDateRangeWithingTimeInterval(p.StartDate, p.EndDate, @StartDate, @EndDate) = 1 OR (p.StartDate IS NULL AND p.EndDate IS NULL))
			AND ( @ClientIds IS NULL OR p.ClientId IN (select * from @ClientsList) )
			AND ( @ProjectGroupIds IS NULL OR p.GroupId IN (SELECT * from @ProjectGroupsList) )
			AND ( @PracticeIds IS NULL OR p.PracticeId IN (SELECT * FROM @PracticesList) OR p.PracticeId IS NULL )
			AND ( @ProjectOwnerIds IS NULL OR p.ProjectManagerId IN (SELECT * FROM @ProjectOwnersList) )
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
	ORDER BY CASE p.ProjectStatusId
			   WHEN 2 THEN p.StartDate
			   ELSE p.EndDate
			 END

