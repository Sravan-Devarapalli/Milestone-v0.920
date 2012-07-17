-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 9-02-2008
-- Updated by:	
-- Update date:	
-- Description:	Selects the default recruiter commission for the specified person and date
-- =============================================
CREATE PROCEDURE [dbo].[DefaultRecruiterCommissionGetByPersonDate]
(
	@PersonId   INT,
	@Date       DATETIME
)
AS
BEGIN
	SET NOCOUNT ON
	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

	SELECT h.DefaultRecruiterCommissionHeaderId, h.PersonId, h.StartDate,
	       CASE h.EndDate WHEN @FutureDate THEN CAST(NULL AS DATETIME) ELSE h.EndDate END AS EndDate,
	       dbo.MakeDefaultRecruiterCommissionText(h.defaultrecruitercommissionheaderid) AS TextLine
	  FROM dbo.DefaultRecruiterCommissionHeader as h
	 WHERE h.PersonId = @PersonId AND @Date >= h.StartDate AND @Date < h.EndDate
END
