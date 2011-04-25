
CREATE VIEW [dbo].[v_Opportunity]
AS
	SELECT o.OpportunityId,
	       o.Name,	       
	       o.ClientId,
	       o.SalespersonId,
	       o.OpportunityStatusId,
	       o.Priority,
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
           o.ProjectId,
           o.OpportunityIndex,
           o.RevenueType,
		   o.EstimatedRevenue,
		   o.LastUpdated as 'LastUpdate',
		   o.GroupId,
		   g.[Name] as 'GroupName',
	       c.Name AS ClientName,
	       c.DefaultDiscount AS Discount,
	       c.DefaultTerms AS Terms,
	       p.FirstName AS SalespersonFirstName,
	       p.LastName AS SalespersonLastName,
		   p.PersonStatusId AS SalespersonStatusId,
	       s.Name AS OpportunityStatusName,
	       r.Name AS PracticeName,
		   own.PersonId AS 'OwnerId',
		   prowner.PersonId AS 'PracticeManagerId',
		   own.PersonStatusId AS 'OwnerStatusId'
	  FROM dbo.Opportunity AS o
	       INNER JOIN dbo.Client AS c ON o.ClientId = c.ClientId
	       LEFT JOIN dbo.Person AS p ON o.SalespersonId = p.PersonId
		   LEFT JOIN dbo.Person AS own ON o.OwnerID = own.PersonId
	       INNER JOIN dbo.OpportunityStatus AS s ON o.OpportunityStatusId = s.OpportunityStatusId
	       INNER JOIN dbo.Practice AS r ON o.PracticeId = r.PracticeId	
		   Left Join dbo.ProjectGroup as g on o.GroupId = g.GroupId
		   Left join dbo.Person as prowner on prowner.PersonId = r.PracticeManagerId
