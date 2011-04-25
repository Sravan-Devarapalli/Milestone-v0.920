-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-06-23
-- Description:	Updates LastUpdated column
-- =============================================
CREATE TRIGGER Opportunity_UpdateLastUpdatedColumn 
   ON  dbo.Opportunity 
   AFTER UPDATE
AS 
BEGIN
	SET NOCOUNT ON;

	Update Opportunity 
	set Opportunity.LastUpdated = getdate()
	From Opportunity inner join inserted on inserted.OpportunityId = Opportunity.OpportunityId
END

GO
EXECUTE sp_settriggerorder @triggername = N'[dbo].[Opportunity_UpdateLastUpdatedColumn]', @order = N'first', @stmttype = N'update';


