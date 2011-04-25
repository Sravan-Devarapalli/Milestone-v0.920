-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects the default recruiter commission by the specified ID
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionGetById]
(
	@DefaultRecruiterCommissionHeaderId   INT
)
AS
	SET NOCOUNT ON

	SELECT h.DefaultRecruiterCommissionHeaderId, h.PersonId, h.StartDate,
	       CASE h.EndDate WHEN dbo.GetFutureDate() THEN CAST(NULL AS DATETIME) ELSE h.EndDate END AS EndDate,
	       dbo.MakeDefaultRecruiterCommissionText(h.defaultrecruitercommissionheaderid) AS TextLine
	  FROM dbo.DefaultRecruiterCommissionHeader as h
	 WHERE h.DefaultRecruiterCommissionHeaderId = @DefaultRecruiterCommissionHeaderId

