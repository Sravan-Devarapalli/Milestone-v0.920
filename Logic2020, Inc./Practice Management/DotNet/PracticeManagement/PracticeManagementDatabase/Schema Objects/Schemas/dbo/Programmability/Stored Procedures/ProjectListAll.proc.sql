CREATE PROCEDURE  [dbo].[ProjectListAll]
(
	@ShowProjected         BIT = 0,
	@ShowCompleted         BIT = 0,
    @ShowActive            BIT = 0,
	@ShowExperimental      BIT = 0,
	@ShowProposed		   BIT = 0
)
AS
BEGIN
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
		   p.ExecutiveInChargeId AS DirectorId,
		   p.DirectorLastName,
		   p.DirectorFirstName,
		   pg.Name AS GroupName,
		   1 InUse
	  FROM dbo.v_Project AS p
	  INNER JOIN dbo.ProjectGroup AS pg 
	  ON p.GroupId = pg.GroupId
	 WHERE  (
				(@ShowProjected = 1 AND p.ProjectStatusId = 2)
            OR	(@ShowActive = 1 AND p.ProjectStatusId = 3)
            OR	(@ShowCompleted = 1 AND p.ProjectStatusId = 4)
			OR	(@ShowProposed = 1 AND p.ProjectStatusId = 7) --proposed
	        OR	(@ShowExperimental = 1 AND (p.ProjectStatusId = 5 OR p.ProjectStatusId = 1))
			)
    
	ORDER BY p.Name
END

