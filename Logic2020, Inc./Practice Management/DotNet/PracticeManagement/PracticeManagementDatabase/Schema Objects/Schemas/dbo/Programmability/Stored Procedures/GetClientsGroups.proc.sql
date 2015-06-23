CREATE PROCEDURE dbo.GetClientsGroups
	@CommaSeperatedClientList	NVARCHAR(3000),
	@ProjectId	INT = NULL,
	@PersonId	INT = NULL
AS
BEGIN
	SET NOCOUNT ON;

	DECLARE @GroupPermissions TABLE(
		GroupId INT NULL
	)
	
	DECLARE @CilentList TABLE(ClientId INT NULL)
	
	INSERT INTO @CilentList
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@CommaSeperatedClientList)

	-- Populate is with the data from the permissions table
	--		TargetType = 1 means that we are looking for the clients in the permissions table
	INSERT INTO @GroupPermissions (GroupId) 
	SELECT prm.TargetId 
	FROM dbo.v_Permissions AS prm 
	WHERE prm.PersonId = @PersonId AND prm.PermissionTypeID = 2
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @GroupPermissions AS sp WHERE sp.GroupId IS NULL 	
	IF @NullsNumber > 0 SET @PersonId = NULL

	IF @ProjectId IS NOT NULL
	BEGIN
		SELECT
				pg.GroupId
				, pg.ClientId
				, pg.Name
				, 1 InUse
				,pg.Active
			FROM dbo.ProjectGroup pg
				INNER JOIN dbo.Project p ON pg.GroupId = p.GroupId				
			WHERE p.ProjectId = @ProjectId 
					AND (@PersonId IS NULL OR pg.GroupId IN (SELECT GroupId FROM @GroupPermissions))
	END
	ELSE
	BEGIN
	  ;WITH GroupIdsWithInUse 
	  AS 
	  (

	  -- Here Rank() is not necessary to achieve this functionality but when compared with other equivalent queries  it is performing better.
	   SELECT GroupId,InUse
	   FROM (
				SELECT RANK() OVER(PARTITION BY  pro.GroupId ORDER BY pro.ProjectId) AS RANKnO , 1 InUse,pro.GroupId,pro.ClientId
				FROM dbo.Project AS pro 
				INNER JOIN @CilentList AS clients 
				ON clients.ClientId = pro.ClientId  
				UNION 
				SELECT RANK() OVER(PARTITION BY op.GroupId ORDER BY op.OpportunityId) AS RANKnO , 1 InUse,op.GroupId,op.ClientId
				FROM dbo.Opportunity AS op 
				INNER JOIN @CilentList AS clients
				ON clients.ClientId = op.ClientId  
				UNION
				SELECT RANK() OVER(PARTITION BY CC.ProjectGroupId ORDER BY CC.Id) AS RANKnO , 1 InUse,CC.ProjectGroupId GroupId,CC.ClientId
				FROM dbo.ChargeCode CC 
				INNER JOIN @CilentList AS clients ON clients.ClientId = cc.ClientId 
				INNER JOIN dbo.TimeEntry TE ON TE.ChargeCodeId = CC.Id 
		    ) AS G
		Where G.RANKnO = 1

	  )

			SELECT
				pg.GroupId
				, pg.ClientId
				, pg.Name
				,ISNULL(GrInUse.InUse,0) AS InUse
				,pg.Active
			FROM dbo.ProjectGroup pg
			INNER JOIN @CilentList AS clients ON pg.ClientId =  clients.ClientId
			LEFT JOIN GroupIdsWithInUse AS GrInUse ON GrInUse.GroupId = pg.GroupId 
			WHERE       (@PersonId IS NULL 
					     OR pg.GroupId IN (SELECT GroupId FROM @GroupPermissions)
						 OR pg.GroupId IN (SELECT pro.GroupId FROM Project AS pro
										INNER JOIN dbo.ProjectAccess AS projManagers ON projManagers.ProjectId = pro.ProjectId
										WHERE projManagers.ProjectAccessId = @PersonId )
						 OR pg.GroupId IN (SELECT pro.GroupId FROM Project AS pro
										WHERE pro.ProjectManagerId = @PersonId OR pro.SalesPersonId = @PersonId)
						 )
	END


END

