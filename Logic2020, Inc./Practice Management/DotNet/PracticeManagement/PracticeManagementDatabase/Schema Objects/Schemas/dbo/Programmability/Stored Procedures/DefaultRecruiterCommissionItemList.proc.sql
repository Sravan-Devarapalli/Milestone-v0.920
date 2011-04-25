-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects a list of the default recruiter commission items for the specified commission
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionItemList]
(
	@DefaultRecruiterCommissionHeaderId   INT
)
AS
	SET NOCOUNT ON

	SELECT i.DefaultRecruiterCommissionHeaderId, i.HoursToCollect, i.Amount
	  FROM dbo.DefaultRecruiterCommissionItem AS i
	 WHERE i.DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId
	ORDER BY i.HoursToCollect

