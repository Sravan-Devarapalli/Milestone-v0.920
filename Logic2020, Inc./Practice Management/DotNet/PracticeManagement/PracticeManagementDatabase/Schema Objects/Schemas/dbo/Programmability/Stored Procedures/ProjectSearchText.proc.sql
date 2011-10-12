-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 11-07-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 12-04-2008
-- Updated by:	Anton Kolensikov
-- Update date: 08-07-2009
-- Description:	Retrives the list of projects by the specified conditions.
-- =============================================
CREATE PROCEDURE [dbo].[ProjectSearchText]
(
	@Looked              NVARCHAR(255),
	@PersonId			 INT
)
AS
	SET NOCOUNT ON

	DECLARE @PersonRole NVARCHAR(50)
	SELECT @PersonRole = uir.RoleName
	FROM v_UsersInRoles AS uir
    WHERE uir.PersonId = @PersonId and uir.RoleName = 'Administrator' 

	IF @PersonRole = 'Administrator' 
		SET @PersonId = null

	DECLARE @SearchText NVARCHAR(257)
	SET @SearchText = '%' + ISNULL(@Looked, '') + '%'


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
	INNER JOIN ProjectManagers AS projManagers ON projManagers.ProjectId = proj.ProjectId
	LEFT JOIN dbo.Commission C ON C.ProjectId = proj.ProjectId AND C.CommissionType = 1
	WHERE projManagers.ProjectManagerId = @PersonId OR C.PersonId = @PersonId -- Adding Salesperson - Project clients into the list.

	INSERT INTO @OwnerProjectGroupList (GroupId) 
	SELECT GroupId FROM  dbo.ProjectGroup
	WHERE ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc)


	-- Search for a project with milestone(s)
	;WITH FoundProjects AS (
	SELECT m.ClientId,
	       m.ProjectId,
	       m.MilestoneId,
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
					ELSE 0 END AS HasAttachments
	  FROM dbo.v_Milestone AS m
	       INNER JOIN ProjectManagers AS projManagers ON projManagers.ProjectId = m.ProjectId
	       INNER JOIN dbo.Commission AS c ON m.ProjectId = c.ProjectId 
	       INNER JOIN dbo.ProjectStatus AS s ON m.ProjectStatusId = s.ProjectStatusId
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = m.ProjectId) A
	 WHERE (
			m.ProjectName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.Description LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	       )
		   AND (@PersonId is NULL OR m.ClientId IN (select * from @ClientsList) OR  m.ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc))
		   AND (@PersonId is NULL OR m.GroupId IN (select * from @ProjectGroupsList) OR  m.GroupId IN (SELECT opG.GroupId FROM @OwnerProjectGroupList AS opG))
		   AND (@PersonId is NULL OR projManagers.ProjectManagerId = @PersonId OR c.PersonId = @PersonId OR projManagers.ProjectManagerId in (select * from @ProjectOwnerList))   
	UNION ALL
	SELECT p.ClientId,
	       p.ProjectId,
	       NULL AS MilestoneId,
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
					ELSE 0 END AS HasAttachments
	  FROM dbo.v_Project AS p
	  INNER JOIN ProjectManagers AS projManagers ON projManagers.ProjectId = p.ProjectId
	    INNER JOIN dbo.Commission AS c ON  p.ProjectId = c.ProjectId 
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
	 WHERE NOT EXISTS (SELECT 1 FROM dbo.Milestone AS m WHERE m.ProjectId = p.ProjectId)
	   AND (   
			p.Name LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	       )
	   AND (@PersonId is NULL OR p.ClientId in (SELECT * FROM @ClientsList) OR  p.ClientId IN (SELECT opc.ClientId FROM @OwnerProjectClientList AS opc))
	   AND (@PersonId is NULL OR p.GroupId in (SELECT * FROM @ProjectGroupsList) OR  P.GroupId IN (SELECT opG.GroupId FROM @OwnerProjectGroupList AS opG))
	   AND (@PersonId is NULL OR p.PracticeId in (SELECT * FROM @PracticesList))	
	   AND (@PersonId is NULL OR projManagers.ProjectManagerId = @PersonId  OR c.PersonId = @PersonId OR projManagers.ProjectManagerId in (SELECT * FROM @ProjectOwnerList))   
	)
	SELECT DISTINCT *
	FROM FoundProjects
	WHERE ProjectId <> @DefaultProjectId
	ORDER BY ProjectName, Description
