﻿CREATE PROCEDURE [dbo].[OpportunityListAllShort]
(
	@ActiveOnly      BIT
)
AS
BEGIN
	SET NOCOUNT ON
	
	;WITH CTE
	AS
	(
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') 
							ORDER BY CASE OP.sortOrder WHEN 0 THEN 1000 ELSE OP.sortOrder END,
							YEAR(O.ProjectedStartDate),MONTH(O.ProjectedStartDate),
							O.SalespersonLastName) RowNumber,
		   o.OpportunityId,
		   o.Name,
		   o.Priority,
		   op.sortOrder PrioritySortOrder,
		   o.ClientId,
		   o.ClientName,
		   o.OpportunityIndex,
		   o.GroupId,
		   o.GroupName,
		   o.SalespersonId ,
		   o.SalespersonFirstName,
		   o.SalespersonLastName,
		   o.OwnerId,
		   p.LastName as 'OwnerLastName',
		   p.FirstName as 'OwnerFirstName',
		   o.EstimatedRevenue,
		   o.BuyerName,
		   O.ProjectedStartDate
	FROM v_Opportunity AS o
	LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
	INNER JOIN dbo.OpportunityPriorities AS op ON op.Id = o.PriorityId
	WHERE o.OpportunityStatusId = 1 OR @ActiveOnly = 0
	)
	SELECT 
			B.*
		FROM CTE A
		JOIN CTE B
		ON (A.ClientName =B.ClientName AND isnull(A.BuyerName, '')  = isnull(B.BuyerName, '') 
			AND A.RowNumber=1 AND A.PrioritySortOrder!=0 AND B.PrioritySortOrder != 0 ) 
			OR (A.OpportunityId = B.OpportunityId AND A.PrioritySortOrder=0)
		ORDER BY A.PrioritySortOrder,
		YEAR(A.ProjectedStartDate),
		MONTH(A.ProjectedStartDate),
		A.SalespersonLastName,
		B.ClientName,
		isnull(B.BuyerName, ''),B.PrioritySortOrder,B.SalespersonLastName

END
