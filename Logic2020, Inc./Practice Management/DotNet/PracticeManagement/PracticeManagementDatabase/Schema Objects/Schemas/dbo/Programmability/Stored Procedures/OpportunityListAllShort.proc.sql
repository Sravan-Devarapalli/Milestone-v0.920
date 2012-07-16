CREATE PROCEDURE [dbo].[OpportunityListAllShort]
(
	@ActiveOnly      BIT
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @FutureDate DATETIME 
	SET @FutureDate = dbo.GetFutureDate()

	;WITH CTE
	AS
	(
	SELECT ROW_NUMBER() OVER(PARTITION BY O.ClientName + isnull(O.BuyerName, '') 
							ORDER BY CASE OP.sortOrder WHEN 0 THEN 1000 ELSE OP.sortOrder END,
							YEAR(ISNULL(O.ProjectedStartDate,@FutureDate)),
							MONTH(ISNULL(O.ProjectedStartDate,@FutureDate)),
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
		   p.LastName as [OwnerLastName],
		   p.FirstName as [OwnerFirstName],
		   o.EstimatedRevenue,
		   o.BuyerName,
		   O.ProjectedStartDate
	FROM dbo.v_Opportunity AS o
	LEFT JOIN dbo.Person p ON o.OwnerId = p.PersonId
	INNER JOIN dbo.OpportunityPriorities AS op ON op.Id = o.PriorityId
	WHERE o.OpportunityStatusId = 1 OR @ActiveOnly = 0
	)
	SELECT 
			B.*
		FROM CTE A
		JOIN CTE B
		ON (A.ClientName =B.ClientName AND ISNULL(A.BuyerName, '')  = ISNULL(B.BuyerName, '') 
			AND A.RowNumber=1 AND A.PrioritySortOrder!=0 AND B.PrioritySortOrder != 0 ) 
			OR (A.OpportunityId = B.OpportunityId AND A.PrioritySortOrder=0)
		ORDER BY A.PrioritySortOrder,
				YEAR(ISNULL(A.ProjectedStartDate,@FutureDate)),
				MONTH(ISNULL(A.ProjectedStartDate,@FutureDate)),
				A.SalespersonLastName,
				B.ClientName,
				ISNULL(B.BuyerName, ''),
				B.PrioritySortOrder,
				B.SalespersonLastName

END
