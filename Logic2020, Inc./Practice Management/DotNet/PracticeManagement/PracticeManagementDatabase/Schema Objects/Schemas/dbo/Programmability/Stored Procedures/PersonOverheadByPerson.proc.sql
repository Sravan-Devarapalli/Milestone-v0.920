-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 7-22-2008
-- Description:	Lists a person's overhead
-- =============================================
CREATE PROCEDURE [dbo].[PersonOverheadByPerson]
(
	@PersonId   INT
)
AS
	SET NOCOUNT ON

	DECLARE @today DATETIME, @FutureDate DATETIME
	SELECT @today = dbo.Today(),
			@FutureDate = dbo.GetFutureDate()

	SELECT o.PersonId,
	       o.Description,
	       o.Rate,
	       o.HoursToCollect,
	       o.StartDate,
	       o.EndDate,
	       o.IsPercentage,
	       o.OverheadRateTypeId,
	       o.OverheadRateTypeName,
	       o.BillRateMultiplier
	  FROM dbo.v_PersonOverhead AS o
	 WHERE o.PersonId = @PersonId
	   AND o.StartDate <= @today AND ISNULL(o.EndDate, @FutureDate) > @today

