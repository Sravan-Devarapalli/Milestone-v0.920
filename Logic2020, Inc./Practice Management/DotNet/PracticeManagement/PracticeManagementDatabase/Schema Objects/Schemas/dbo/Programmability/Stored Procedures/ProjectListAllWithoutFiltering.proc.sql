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
		   1 AS InUse,
		   c.PersonId AS 'SalespersonId',
		   cp.LastName+' , ' +cp.FirstName AS 'SalespersonName',
		   	p.BusinessTypeId,
			p.PricingListId,
			PL.Name AS PricingListName,
			BG.BusinessGroupId,
			BG.Name AS BusinessGroupName,
		   sm.PersonId AS 'SeniorManagerId',
		   sm.LastName+' , ' +sm.FirstName AS 'SeniorManagerName',
		   re.PersonId AS 'ReviewerId',
		   re.LastName+' , ' +re.FirstName AS 'ReviewerName'
	FROM v_Project p
	LEFT JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	LEFT JOIN dbo.Commission c ON c.ProjectId = p.ProjectId AND c.CommissionType = 1
	LEFT JOIN dbo.Person cp ON cp.PersonId = c.PersonId
    LEFT JOIN Person as sm on sm.PersonId = p.SeniorManagerId
	LEFT JOIN Person as re on re.PersonId = p.ReviewerId
	LEFT JOIN dbo.BusinessGroup AS BG ON PG.BusinessGroupId=BG.BusinessGroupId
	LEFT JOIN dbo.PricingList AS PL ON P.PricingListId=PL.PricingListId 
	WHERE P.ProjectId <> @DefaultProjectId
	AND P.IsAllowedToShow = 1

GO

