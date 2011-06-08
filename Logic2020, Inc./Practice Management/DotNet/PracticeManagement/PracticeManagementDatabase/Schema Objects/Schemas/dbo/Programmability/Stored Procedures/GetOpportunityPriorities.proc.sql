CREATE PROCEDURE [dbo].[GetOpportunityPriorities]
@IsInserted BIT
AS
BEGIN
	SET NOCOUNT ON
	SELECT  	OP.Id,
			OP.Priority,
			OP.Description,
			CASE 
			WHEN EXISTS(SELECT TOP 1 o.PriorityId FROM  v_Opportunity AS o WHERE o.PriorityId = op.Id)
				THEN CAST(1 AS BIT)
			ELSE CAST(0 AS BIT)
		END AS 'InUse'
	FROM dbo.OpportunityPriorities AS OP
	WHERE  op.IsInserted = @IsInserted
	ORDER BY OP.sortOrder
END

GO
