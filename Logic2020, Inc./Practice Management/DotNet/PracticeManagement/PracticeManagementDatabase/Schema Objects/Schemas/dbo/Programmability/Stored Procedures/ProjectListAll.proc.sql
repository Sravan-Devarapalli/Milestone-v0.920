-- =============================================
-- Updated by:	Anatoliy Lokshin
-- Update date:	7-17-2008
-- Updated by:	Anton Kramarenko
-- Update date: 02-20-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 07-16-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 08-06-2009
-- Description:	Lists the projects those fall into the specified conditions.
-- =============================================
-- Arguments:
--     @ClientId - determines an ID of the Client to select the projects associated with
--     @ShowProjected - determines do include the projected projects into the result
--     @ShowCompleted - determines do include the completed projects into the result
--     @ShowActive - determines do include active projects into the result
--     @ShowExperimental - determines do include experimental projects into the result
--     @SalespersonId - determines an ID of the salesperson to filter the list for
--     @PracticeManagerId - determines an ID of the practice manager to filter the list for
--     @PracticeId - determines an ID of the practice to filter the list for
--     @ProjectGroupId - determines an ID of the project group to filter the list for
-- =============================================
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
		   p.ProjectManagerId,
		   p.ProjectManagerFirstName,
		   p.ProjectManagerLastName,
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


