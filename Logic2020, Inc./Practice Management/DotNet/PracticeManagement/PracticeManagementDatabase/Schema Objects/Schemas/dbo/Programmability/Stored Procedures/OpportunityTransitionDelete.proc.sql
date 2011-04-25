-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-07-05
-- Description:	Removes OpportunityTransition
-- =============================================
CREATE PROCEDURE dbo.OpportunityTransitionDelete
	@OpportunityTransitionId	INT
AS
BEGIN
	SET NOCOUNT ON;

	delete 
	from OpportunityTransition
	where OpportunityTransitionId = @OpportunityTransitionId
END

