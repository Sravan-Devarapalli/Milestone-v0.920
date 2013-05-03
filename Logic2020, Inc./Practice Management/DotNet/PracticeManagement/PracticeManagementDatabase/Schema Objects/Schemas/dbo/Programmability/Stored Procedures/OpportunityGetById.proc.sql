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
		   proj.ProjectNumber,
	       o.OpportunityIndex,
	       o.RevenueType,
		   o.OwnerId,		   
		   o.LastUpdate,
		   o.GroupId,
		   o.PricingListId,
		   pl.Name AS PricingListName,
		   o.BusinessTypeId,
		   o.GroupName,
		   o.PracticeManagerId,
		   p.LastName as 'OwnerLastName',
		   p.FirstName as 'OwnerFirstName',
		   os.Name AS 'OwnerStatus',
		   o.EstimatedRevenue ,
		   o.CloseDate
		   
	 FROM dbo.v_Opportunity AS o
	 LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
	 LEFT JOIN dbo.PersonStatus ps ON ps.PersonStatusId = o.SalespersonStatusId 	
	 LEFT JOIN dbo.PersonStatus os ON os.PersonStatusId = o.OwnerStatusId
	 LEFT JOIN dbo.Project proj ON proj.ProjectId = o.ProjectId
	 LEFT JOIN dbo.PricingList pl ON pl.PricingListId = o.PricingListId
	 WHERE o.OpportunityId = @OpportunityId

