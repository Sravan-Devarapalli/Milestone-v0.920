CREATE PROCEDURE dbo.ProjectGroupListAll
	@ClientId	INT = NULL,
	@ProjectId	INT = NULL,
	@PersonId	INT = NULL
AS
	SET NOCOUNT ON;

	DECLARE @GroupPermissions TABLE(
		PersonId INT NULL
	)

	-- Populate is with the data from the permissions table
	--		TargetType = 1 means that we are looking for the clients in the permissions table
	INSERT INTO @GroupPermissions (
		PersonId
	) SELECT prm.TargetId FROM dbo.v_Permissions AS prm WHERE prm.PersonId = @PersonId AND prm.PermissionTypeID = 2
	
	-- If there are nulls in permission table, it means that eveything is allowed to be seen,
	--		so set @PersonId to NULL which will extract all records from the table
	DECLARE @NullsNumber INT
	SELECT @NullsNumber = COUNT(*) FROM @GroupPermissions AS sp WHERE sp.PersonId IS NULL 	
	IF @NullsNumber > 0 SET @PersonId = NULL

	IF @ProjectId IS NOT NULL
		SELECT
				pg.GroupId
				, pg.ClientId
				,c.Name AS ClientName
				, pg.Code
				, pg.Name
				, 1 InUse
				,pg.Active,
				pg.BusinessGroupId
			FROM ProjectGroup pg
				INNER JOIN Project p ON pg.GroupId = p.GroupId				
				INNER JOIN Client c on c.ClientId = pg.ClientId
			WHERE p.ProjectId = @ProjectId 
					AND (@PersonId IS NULL OR pg.GroupId IN (SELECT * FROM @GroupPermissions))
			ORDER BY pg.Name
	ELSE
	IF @ClientId IS NOT NULL 
			SELECT
				GroupId
				,pg.ClientId
				,c.Name AS ClientName
				, pg.Code
				, pg.Name
				,dbo.IsProjectGroupInUse(pg.GroupId,pg.Code) AS InUse
				,Active
				,BusinessGroupId
			FROM ProjectGroup PG
			INNER JOIN Client c on c.ClientId = PG.ClientId
			WHERE ((pg.ClientId = @ClientId))
					AND (@PersonId IS NULL 
					     OR GroupId IN (SELECT * FROM @GroupPermissions)
						 OR GroupId IN (SELECT pro.GroupId FROM Project AS pro
										INNER JOIN dbo.ProjectAccess AS projManagers ON projManagers.ProjectId = pro.ProjectId
										WHERE projManagers.ProjectAccessId = @PersonId )
						 OR GroupId IN (SELECT pro.GroupId FROM Project AS pro
										WHERE pro.ProjectManagerId = @PersonId )
						 OR GroupId IN (SELECT proj.GroupId 
										FROM Project AS proj
										WHERE proj.SalesPersonId = @PersonId)
						 )
			ORDER BY Name

	ELSE 
			SELECT
				  pg.ClientId
				, cl.[Name] AS 'ClientName'
				, pg.GroupId
				,pg.Code
				, pg.[Name]
				, dbo.IsProjectGroupInUse(pg.GroupId,pg.Code) AS InUse
				,pg.Active
				,pg.BusinessGroupId
			FROM ProjectGroup AS pg
			INNER JOIN dbo.Client AS cl ON pg.ClientId = cl.ClientId
			WHERE(@PersonId IS NULL OR pg.GroupId IN (SELECT * FROM @GroupPermissions))
			ORDER BY ClientName,pg.[Name]
			


