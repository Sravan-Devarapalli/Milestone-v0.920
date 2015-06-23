﻿CREATE PROCEDURE [dbo].[ProjectListAllWithoutFiltering]
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
		   p.DirectorFirstName,
		   pg.Name AS GroupName,
		   1 AS InUse,
		   p.SalesPersonId,
		   cp.LastName+', ' +cp.FirstName AS 'SalespersonName',
		   	p.BusinessTypeId,
			p.PricingListId,
			PL.Name AS PricingListName,
			BG.BusinessGroupId,
			BG.Name AS BusinessGroupName,
		   	CASE WHEN p.IsSeniorManagerUnassigned = 1 THEN -1 ELSE  sm.PersonId  END AS 'SeniorManagerId',
			CASE WHEN p.IsSeniorManagerUnassigned = 1 THEN 'Unassigned' ELSE  sm.LastName+', ' +sm.FirstName END AS 'SeniorManagerName',
		   re.PersonId AS 'ReviewerId',
		   re.LastName+', ' +re.FirstName AS 'ReviewerName',
		   p.PONumber,
		   P.ProjectManagerId AS ProjectOwnerId,
		   Powner.LastName AS [ProjectOwnerLastName],
		   Powner.FirstName AS [ProjectOwnerFirstName]
	FROM v_Project p
	LEFT JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	LEFT JOIN dbo.Person cp ON cp.PersonId = p.SalesPersonId
    LEFT JOIN Person as sm on sm.PersonId = p.EngagementManagerId
	LEFT JOIN Person as re on re.PersonId = p.ReviewerId
	LEFT JOIN dbo.BusinessGroup AS BG ON PG.BusinessGroupId=BG.BusinessGroupId
	LEFT JOIN dbo.PricingList AS PL ON P.PricingListId=PL.PricingListId 
	LEFT JOIN dbo.Person AS Powner ON Powner.PersonId = P.ProjectManagerId
	WHERE P.ProjectId <> @DefaultProjectId
	AND P.IsAllowedToShow = 1

