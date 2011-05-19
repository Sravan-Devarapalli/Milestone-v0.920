CREATE PROCEDURE [dbo].[OpportunityListAllShort]
(
	@ActiveOnly      BIT
)
AS
BEGIN
	SET NOCOUNT ON
	
	;WITH CTE
	AS
	(
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') ORDER BY CASE OP.sortOrder WHEN 0 THEN 1000 ELSE OP.sortOrder END, O.SalespersonLastName) RowNumber,
		   o.OpportunityId,
		   o.Name,
		   o.Priority,
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
		   OP.sortOrder
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
			AND A.RowNumber=1 AND A.sortOrder!=0 AND B.SortOrder != 0 ) OR (A.OpportunityId = B.OpportunityId AND A.sortOrder=0)
		ORDER BY A.sortOrder,A.SalespersonLastName,B.ClientName,isnull(B.BuyerName, ''),B.sortOrder,B.SalespersonLastName

END
