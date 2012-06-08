-- =============================================
-- Author:		Srinivas.M
-- Create date: 06-06-2012
-- Updated By:	
-- Updated Date: 
-- Description:	Returns the Opportunity list after filetered.
-- =============================================
CREATE PROCEDURE [dbo].[OpportunityListWithMinimumDetails]
(
	@ClientId INT,
	@Link	BIT
)
AS
BEGIN
	
	SELECT O.OpportunityId,
			O.Name AS OpportunityName,
			O.OpportunityNumber,
			C.Name AS ClientName
	FROM dbo.Opportunity O
	INNER JOIN dbo.Client C ON O.ClientId =C.ClientId
	WHERE O.OpportunityStatusId = 1
		AND (@Link IS NULL
			OR (@Link IS NOT NULL AND @Link = 0 AND O.ProjectId IS NULL)
			OR (@Link IS NOT NULL AND @Link = 1 AND O.ProjectId IS NOT NULL)
			)
		AND ((@ClientId IS NOT NULL AND O.ClientId = @ClientId)
			OR @ClientId IS NULL
			)
	ORDER BY O.OpportunityNumber

END
