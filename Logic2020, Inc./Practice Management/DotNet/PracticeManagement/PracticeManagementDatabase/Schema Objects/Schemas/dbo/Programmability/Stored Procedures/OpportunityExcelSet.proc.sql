CREATE PROCEDURE [dbo].[OpportunityExcelSet]
AS 
BEGIN
SELECT O.OpportunityNumber AS 'Opportunity'
	 , O.ClientName AS 'Account'
	 , O.GroupName AS 'Business Unit'
	 , O.BuyerName AS 'Buyer'
	 , O.Name AS 'OpportunityName'
	 , O.DisplayName AS 'Sales Stage'
	 , O.OpportunityStatusName AS 'Status'
	 , CONVERT(NVARCHAR(10),CONVERT(DATE,O.CloseDate)) AS 'Close Date'
	 , CONVERT(NVARCHAR(10),CONVERT(DATE,O.ProjectedStartDate)) AS 'Project Start Date'
	 , CONVERT(NVARCHAR(10),CONVERT(DATE,O.ProjectedEndDate)) AS 'Project End Date'
	 , '$'+CONVERT(NVARCHAR(50), CONVERT(money,O.EstimatedRevenue),1)  AS 'Estimated Revenue'
	 , O.SalespersonLastName + ',' + O.SalespersonFirstName AS 'Salesperson'
	 , p.LastName + ',' + p.FirstName AS 'Owner'
	 , O.PracticeName AS 'PracticeArea'	
	 , CASE WHEN proj.ProjectId IS NULL 
	        THEN
			   'NO'
		    ELSE
			   'YES'
	   END AS 'Linked to Project?'
	 , proj.ProjectNumber AS 'Project Number'
FROM
	dbo.v_Opportunity O
	INNER JOIN dbo.Person p
		ON o.OwnerId = p.PersonId
	LEFT JOIN dbo.Project proj
		ON proj.ProjectId = o.ProjectId
WHERE
	O.OpportunityStatusId = 1
END


