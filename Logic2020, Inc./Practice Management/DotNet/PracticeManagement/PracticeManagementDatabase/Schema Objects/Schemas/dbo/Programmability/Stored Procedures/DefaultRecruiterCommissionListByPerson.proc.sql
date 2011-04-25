-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects a list of the default recruiter commissions for the specified person
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionListByPerson]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

	SELECT h.DefaultRecruiterCommissionHeaderId, h.PersonId, h.StartDate,
	       CASE h.EndDate WHEN dbo.GetFutureDate() THEN CAST(NULL AS DATETIME) ELSE h.EndDate END AS EndDate,
	       dbo.MakeDefaultRecruiterCommissionText(h.defaultrecruitercommissionheaderid) AS TextLine
	  FROM dbo.DefaultRecruiterCommissionHeader as h
	 WHERE h.PersonId = @PersonId
	ORDER BY h.StartDate

