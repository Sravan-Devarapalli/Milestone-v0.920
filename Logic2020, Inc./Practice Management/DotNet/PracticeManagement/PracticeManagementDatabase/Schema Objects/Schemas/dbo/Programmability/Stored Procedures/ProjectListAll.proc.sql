
CREATE PROCEDURE [dbo].[ProjectListAll]
(
	@ClientId              INT = NULL, 
	@ShowProjected         BIT = 0,
	@ShowCompleted         BIT = 0,
    @ShowActive            BIT = 0,
	@ShowExperimental      BIT = 0,
	@SalespersonId         INT = NULL,
	@PracticeManagerId     INT = NULL,
	@PracticeId            INT = NULL,
	@ProjectGroupId        INT = NULL
)
AS
	SET NOCOUNT ON;

	SELECT p.ClientId,
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
	       p.ClientIsChargeable,
	       p.ProjectIsChargeable,
		   p.ProjectManagersIdFirstNameLastName,
		   p.DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   pg.Name AS GroupName,
		   1 InUse
	  FROM dbo.v_Project AS p
	  INNER JOIN dbo.ProjectGroup AS pg 
	  ON p.GroupId = pg.GroupId
	 WHERE (p.ClientId = @ClientId OR @ClientId IS NULL)
	   AND (p.GroupId = @ProjectGroupId OR @ProjectGroupId IS NULL)
	   AND (p.PracticeId = @PracticeId OR @PracticeId IS NULL)
	   AND (   (@ShowProjected = 1 AND p.ProjectStatusId = 2)
            OR (@ShowActive = 1 AND p.ProjectStatusId = 3)
            OR (@ShowCompleted = 1 AND p.ProjectStatusId = 4)
	        OR (@ShowExperimental = 1 AND (p.ProjectStatusId = 5 OR p.ProjectStatusId = 1)))
       AND (   (@SalespersonId IS NULL AND @PracticeManagerId IS NULL)
	        OR EXISTS (SELECT 1
	                      FROM dbo.v_PersonProjectCommission AS c
	                     WHERE c.ProjectId = p.ProjectId AND c.PersonId = @SalespersonId AND c.CommissionType = 1
	                   UNION ALL
	                   SELECT 1
	                     FROM dbo.v_PersonProjectCommission AS c
	                    WHERE c.ProjectId = p.ProjectId AND c.PersonId = @PracticeManagerId AND c.CommissionType = 2 
	                   )
	       )
	ORDER BY p.Name


