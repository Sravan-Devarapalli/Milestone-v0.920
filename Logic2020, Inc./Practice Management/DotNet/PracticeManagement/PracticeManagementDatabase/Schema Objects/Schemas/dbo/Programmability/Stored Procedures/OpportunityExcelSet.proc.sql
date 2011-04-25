




CREATE PROCEDURE [dbo].[OpportunityExcelSet]
AS 
    BEGIN

        SELECT	ROW_NUMBER() OVER (ORDER BY opt.OpportunityId) AS 'Review Order', opt.OpportunityId,
				optstat.[Name] AS 'Status', opt.Priority, opt.ProjectedStartDate AS 'Projected Start Date',
				client.[Name] AS 'Client name', prgroup.[Name] AS 'Client Group', opt.[Name] AS 'Opportunity name', 
				pers.FirstName + ' ' + pers.LastName AS 'Salesperson', opt.BuyerName AS 'Buyer Name',
				pract.[Name] AS 'Practice', opt.OpportunityNumber AS 'Opportunity Number', rev.[Name] AS 'Revenue type',
				proj.[ProjectNumber] as 'Attached Project',  dbo.GetOpportunityHistory(opt.OpportunityId) AS 'History'
        FROM dbo.Opportunity AS opt 
			INNER JOIN dbo.RevenueType AS rev ON rev.RevenueTypeId = opt.RevenueType
			INNER JOIN dbo.OpportunityStatus AS optstat ON opt.OpportunityStatusId = optstat.OpportunityStatusId
			INNER JOIN dbo.Client AS client ON opt.ClientId = client.ClientId
			LEFT OUTER JOIN dbo.Person AS pers ON opt.SalespersonId = pers.PersonId
			INNER JOIN dbo.Practice AS pract ON opt.PracticeId = pract.PracticeId
			LEFT OUTER JOIN dbo.Project AS proj ON opt.ProjectId = proj.ProjectId		
			LEFT OUTER JOIN dbo.ProjectGroup AS prgroup ON opt.ClientId = prgroup.ClientId AND proj.GroupId = prgroup.GroupId



    END

