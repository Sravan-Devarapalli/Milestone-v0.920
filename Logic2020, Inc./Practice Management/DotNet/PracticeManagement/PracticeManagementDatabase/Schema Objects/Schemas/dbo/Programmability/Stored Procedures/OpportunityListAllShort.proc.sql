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
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') ORDER BY O.Priority, O.SalespersonLastName) RowNumber,
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
		   o.BuyerName
	FROM v_Opportunity AS o
	LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
	WHERE o.OpportunityStatusId = 1 OR @ActiveOnly = 0
	)
	SELECT 
			B.*
		FROM CTE A
		JOIN CTE B
		ON A.ClientName =B.ClientName AND isnull(A.BuyerName, '')  = isnull(B.BuyerName, '') AND A.RowNumber=1
		ORDER BY A.Priority,A.SalespersonLastName,B.ClientName,isnull(B.BuyerName, ''),B.Priority,B.SalespersonLastName

END
