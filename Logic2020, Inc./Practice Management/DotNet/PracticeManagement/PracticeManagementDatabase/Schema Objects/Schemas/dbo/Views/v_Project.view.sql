CREATE VIEW dbo.v_Project
AS
	SELECT p.ClientId,
		   c.IsMarginColorInfoEnabled,
		   p.ProjectId,
		   p.Discount,
		   p.Terms,
		   p.Name,
		   r.PracticeManagerId,
		   p.PracticeId,
		   p.StartDate,
		   p.EndDate,
		   c.Name AS ClientName,
		   c.IsChargeable AS 'ClientIsChargeable',
		   r.Name AS PracticeName,
		   p.ProjectStatusId,
		   s.Name AS ProjectStatusName,
		   p.ProjectNumber,
		   p.BuyerName,
		   p.OpportunityId,
		   p.GroupId,
		   p.PricingListId,
		   p.BusinessTypeId,
		   p.IsChargeable AS 'ProjectIsChargeable',
		   dbo.GetProjectManagerList(p.ProjectId) AS ProjectManagersIdFirstNameLastName,
		   p.ExecutiveInChargeId,
		   d.LastName as 'DirectorLastName',
		   d.FirstName as 'DirectorFirstName',
		   d.PreferredFirstName as 'DirectorPreferredFirstName',
		   p.Description,
		   p.CanCreateCustomWorkTypes,
		   p.IsAllowedToShow,
		   p.IsInternal,
		   c.IsInternal AS 'ClientIsInternal',
		   p.IsNoteRequired,
		   p.ProjectManagerId,
		   p.SowBudget,
		   p.POAmount,
		   c.IsNoteRequired AS [ClientIsNoteRequired],
		   c.IsHouseAccount,
		   p.IsAdministrative,
		   [dbo].[GetProjectCapabilities](p.ProjectId) AS ProjectCapabilityIds,
			p.ReviewerId,
			p.EngagementManagerId,
			p.IsSeniorManagerUnassigned,
			p.PONumber,
			p.SalesPersonId,
			p.DivisionId,
			pd.DivisionName,
			p.ChannelId,
			ch.ChannelName,
			p.SubChannel,
			p.RevenueTypeId,
			re.RevenueName,
			p.OfferingId,
			o.Name as OfferingName,
			p.IsClientTimeEntryRequired,
			p.PreviousProjectNumber,
			p.OutsourceId
	  FROM dbo.Project AS p
		   INNER JOIN dbo.Practice AS r ON p.PracticeId = r.PracticeId
		   INNER JOIN dbo.Client AS c ON p.ClientId = c.ClientId
		   LEFT JOIN dbo.ProjectDivision AS pd ON pd.DivisionId=p.DivisionId
		   INNER JOIN dbo.ProjectStatus AS s ON p.ProjectStatusId = s.ProjectStatusId
		   LEFT JOIN Person as d on d.PersonId = p.ExecutiveInChargeId
		   LEFT JOIN dbo.Channel ch on ch.ChannelId=p.ChannelId
		   LEFT JOIN dbo.Revenue re ON re.RevenueTypeId=p.RevenueTypeId
		   LEFT JOIN dbo.Offering o ON o.OfferingId=p.OfferingId
		   


