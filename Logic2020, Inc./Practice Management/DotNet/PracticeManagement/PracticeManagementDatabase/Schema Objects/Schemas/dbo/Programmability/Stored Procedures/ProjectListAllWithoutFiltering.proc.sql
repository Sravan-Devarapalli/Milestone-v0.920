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
		   cp.LastName+' , ' +cp.FirstName AS 'SalespersonName'
	FROM v_Project p
	LEFT JOIN dbo.ProjectGroup AS pg ON p.GroupId = pg.GroupId
	LEFT JOIN dbo.Commission c ON c.ProjectId = p.ProjectId AND c.CommissionType = 1
	LEFT JOIN dbo.Person cp ON cp.PersonId = c.PersonId
	WHERE P.ProjectId <> @DefaultProjectId
	AND P.IsAllowedToShow = 1

GO

