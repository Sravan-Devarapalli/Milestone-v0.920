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

	declare @PersonRole varchar(50)
	select @PersonRole = uir.RoleName
	from v_UsersInRoles as uir
    where uir.PersonId = @PersonId and uir.RoleName = 'Administrator' 

	if @PersonRole = 'Administrator' 
		set @PersonId = null

	DECLARE @SearchText NVARCHAR(257)
	SET @SearchText = '%' + ISNULL(@Looked, '') + '%'


	-- Convert client ids from string to table
	declare @ClientsList table (Id int)
	insert into @ClientsList
	select TargetId FROM dbo.PersonClientPermissions(@PersonId)

	-- Convert practice ids from string to table
	declare @PracticesList table (Id int)
	insert into @PracticesList
	select TargetId FROM dbo.PersonPracticePermissions(@PersonId)

	-- Convert project group ids from string to table
	declare @ProjectGroupsList table (Id int)
	insert into @ProjectGroupsList
	select TargetId FROM dbo.PersonProjectGroupPermissions(@PersonId)

	-- Convert project group ids from string to table
	declare @ProjectOwnerList table (Id int)
	insert into @ProjectOwnerList
	select TargetId FROM dbo.PersonProjectOwnerPermissions(@PersonId)

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	-- Search for a project with milestone(s)
	;with FoundProjects as (
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
	       INNER JOIN dbo.ProjectStatus AS s ON m.ProjectStatusId = s.ProjectStatusId
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = m.ProjectId) A
	 WHERE (
			m.ProjectName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.Description LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR m.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	       )
		   AND (@PersonId is NULL OR m.ClientId in (select * from @ClientsList))
		   AND (@PersonId is NULL OR m.GroupId in (select * from @ProjectGroupsList))
		   AND (@PersonId is NULL OR m.ProjectManagerId in (select * from @ProjectOwnerList))   
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
	  OUTER APPLY (SELECT TOP 1 ProjectId FROM ProjectAttachment as pa WHERE pa.ProjectId = p.ProjectId) A
	 WHERE NOT EXISTS (SELECT 1 FROM dbo.Milestone AS m WHERE m.ProjectId = p.ProjectId)
	   AND (   
			p.Name LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.ProjectNumber LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.ClientName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	        OR p.BuyerName LIKE @SearchText COLLATE SQL_Latin1_General_CP1_CI_AS
	       )
	   AND (@PersonId is NULL OR p.ClientId in (select * from @ClientsList))
	   AND (@PersonId is NULL OR p.GroupId in (select * from @ProjectGroupsList))
	   AND (@PersonId is NULL OR p.PracticeId in (select * from @PracticesList))	
	   AND (@PersonId is NULL OR p.ProjectManagerId in (select * from @ProjectOwnerList))   
	)
	select distinct *
	from FoundProjects
	WHERE ProjectId <> @DefaultProjectId
	ORDER BY ProjectName, Description

