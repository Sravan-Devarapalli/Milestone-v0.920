CREATE PROCEDURE [dbo].[OpportunityGetById]
(
	@OpportunityId   INT
)
AS
	SET NOCOUNT ON

	SELECT o.OpportunityId,
	       o.Name,
	       o.ClientId,
	       o.SalespersonId,
	       o.OpportunityStatusId,
	       o.Priority,
		   o.PriorityId,
	       o.ProjectedStartDate,
	       o.ProjectedEndDate,
	       o.OpportunityNumber,
	       o.Description,
	       o.PracticeId,
	       o.BuyerName,
	       o.CreateDate,
	       o.Pipeline,
	       o.Proposed,
	       o.SendOut,
	       o.ClientName,
	       o.SalespersonFirstName,
	       o.SalespersonLastName,
		   ps.Name AS SalespersonStatus,
	       o.OpportunityStatusName,
	       o.PracticeName,
	       o.ProjectId,
	       o.OpportunityIndex,
	       o.RevenueType,
		   o.OwnerId,		   
		   o.LastUpdate,
		   o.GroupId,
		   o.GroupName,
		   o.PracticeManagerId,
		   p.LastName as 'OwnerLastName',
		   p.FirstName as 'OwnerFirstName',
		   os.Name AS 'OwnerStatus',
		   o.EstimatedRevenue
		   
	 FROM dbo.v_Opportunity AS o
	 LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
	 LEFT JOIN dbo.PersonStatus ps ON ps.PersonStatusId = o.SalespersonStatusId 	
	 LEFT JOIN dbo.PersonStatus os ON os.PersonStatusId = o.OwnerStatusId
	 WHERE o.OpportunityId = @OpportunityId
