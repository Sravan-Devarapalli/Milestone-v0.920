﻿-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-05-2008
-- Updated By:	Srinivas.M
-- Updated Date: 2012-06-05
-- Description:	List projects with their details
-- =============================================
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
		   p.IsChargeable AS 'ProjectIsChargeable',
		   dbo.GetProjectManagerList(p.ProjectId) AS ProjectManagersIdFirstNameLastName,
		   p.DirectorId,
		   d.LastName as 'DirectorLastName',
		   d.FirstName as 'DirectorFirstName',
		   p.Description,
		   p.CanCreateCustomWorkTypes,
		   p.IsAllowedToShow,
		   p.IsInternal,
		   c.IsInternal AS 'ClientIsInternal',
		   p.IsNoteRequired,
		   p.ProjectOwnerId,
		   p.SowBudget
	  FROM dbo.Project AS p
		   INNER JOIN dbo.Practice AS r ON p.PracticeId = r.PracticeId
		   INNER JOIN dbo.Client AS c ON p.ClientId = c.ClientId
		   INNER JOIN dbo.ProjectStatus AS s ON p.ProjectStatusId = s.ProjectStatusId
		   LEFT JOIN Person as d on d.PersonId = p.DirectorId


