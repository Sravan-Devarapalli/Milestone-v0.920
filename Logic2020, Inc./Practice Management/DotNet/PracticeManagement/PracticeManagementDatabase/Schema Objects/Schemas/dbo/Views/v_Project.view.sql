-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-05-2008
-- Updated by:	Anton Kramarenko
-- Update date: 02-20-2009
-- Updated by:	Anton Kolesnikov
-- Update date: 07-16-2009
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
		   m.PersonId AS 'ProjectManagerId',
		   m.FirstName AS 'ProjectManagerFirstName',
		   m.LastName AS 'ProjectManagerLastName',
		   p.DirectorId,
		   d.LastName as 'DirectorLastName',
		   d.FirstName as 'DirectorFirstName'
	  FROM dbo.Project AS p
		   INNER JOIN dbo.Practice AS r ON p.PracticeId = r.PracticeId
		   INNER JOIN dbo.Client AS c ON p.ClientId = c.ClientId
		   INNER JOIN dbo.ProjectStatus AS s ON p.ProjectStatusId = s.ProjectStatusId
		   INNER JOIN dbo.Person AS m ON p.ProjectManagerId = m.PersonId
		   LEFT JOIN Person as d on d.PersonId = p.DirectorId


