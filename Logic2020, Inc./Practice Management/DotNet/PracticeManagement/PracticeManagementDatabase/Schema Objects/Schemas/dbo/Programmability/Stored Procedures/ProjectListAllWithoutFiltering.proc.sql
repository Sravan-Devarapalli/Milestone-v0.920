CREATE PROCEDURE [dbo].[ProjectListAllWithoutFiltering]
AS

	DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	SELECT DISTINCT p.ClientId,
		   p.ProjectId,
		   p.Discount,
		   p.Terms,
		   p.Name,
		   p.PracticeManagerId,
		   p.PracticeId,
		   p.DivisionId,
		   p.DivisionName,
		   p.OfferingId,
		   p.OfferingName,
		   p.RevenueTypeId,
		   p.RevenueName,
		   p.ChannelId,
		   p.ChannelName,
		   p.SubChannel,
		   p.StartDate,
		   p.EndDate,
		   p.ClientName,
		   p.PracticeName,
		   p.ProjectStatusId,
		   p.ProjectStatusName,
		   p.ProjectNumber,
		   p.IsHouseAccount,
	       p.BuyerName,
           p.OpportunityId,
           p.GroupId,
	       p.ClientIsChargeable,
	       p.ProjectIsChargeable,
		   p.ProjectManagersIdFirstNameLastName,
		   p.ExecutiveInChargeId AS DirectorId,
		   p.DirectorLastName,
		   ISNULL(p.DirectorPreferredFirstName,p.DirectorFirstName) AS DirectorFirstName,
		   pg.Name AS GroupName,
		   1 AS InUse,
		   p.SalesPersonId,
		   cp.LastName+', ' +ISNULL(cp.PreferredFirstName,cp.FirstName) AS 'SalespersonName',
		   	p.BusinessTypeId,
			p.PricingListId,
			PL.Name AS PricingListName,
			BG.BusinessGroupId,
			BG.Name AS BusinessGroupName,
		   	CASE WHEN p.IsSeniorManagerUnassigned = 1 THEN -1 ELSE  sm.PersonId  END AS 'SeniorManagerId',
			CASE WHEN p.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE  sm.LastName+', ' +ISNULL(sm.PreferredFirstName,sm.FirstName) END AS 'SeniorManagerName',
		   re.PersonId AS 'ReviewerId',
		   re.LastName+', ' +ISNULL(re.PreferredFirstName,re.FirstName) AS 'ReviewerName',
		   p.PONumber,
		   p.IsClientTimeEntryRequired,
		   P.ProjectManagerId AS ProjectOwnerId,
		   Powner.LastName AS [ProjectOwnerLastName],
		   ISNULL(Powner.PreferredFirstName,Powner.FirstName) AS [ProjectOwnerFirstName],
		   dbo.GetProjectCapabilitiesNames(P.ProjectId) AS ProjectCapabilities,
		   PrevProject.ProjectId AS PreviousProjectId,
			PrevProject.ProjectNumber AS PreviousProjectNumber,
			p.OutsourceId
	FROM v_Project p
	LEFT JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	LEFT JOIN dbo.Person cp ON cp.PersonId = p.SalesPersonId
    LEFT JOIN Person as sm on sm.PersonId = p.EngagementManagerId
	LEFT JOIN Person as re on re.PersonId = p.ReviewerId
	LEFT JOIN dbo.BusinessGroup AS BG ON PG.BusinessGroupId=BG.BusinessGroupId
	LEFT JOIN dbo.PricingList AS PL ON P.PricingListId=PL.PricingListId 
	LEFT JOIN dbo.Person AS Powner ON Powner.PersonId = P.ProjectManagerId
	LEFT JOIN dbo.Project AS PrevProject ON p.PreviousProjectNumber=PrevProject.ProjectNumber
	WHERE P.ProjectId <> @DefaultProjectId
	AND P.IsAllowedToShow = 1

