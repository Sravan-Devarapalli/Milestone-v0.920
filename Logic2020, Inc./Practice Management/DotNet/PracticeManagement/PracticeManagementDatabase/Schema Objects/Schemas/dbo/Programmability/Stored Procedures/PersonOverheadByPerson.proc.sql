-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 5-28-2008
-- Updated by:	Anatoliy Lokshin
-- Update date: 7-22-2008
-- Description:	Lists a person's overhead
-- =============================================
CREATE PROCEDURE [dbo].[PersonOverheadByPerson]
(
	@PersonId   INT,
	@EffectiveDate DATETIME = null
)
AS
	SET NOCOUNT ON

	DECLARE @FutureDate DATETIME
 	SELECT @FutureDate = dbo.GetFutureDate()

    IF @EffectiveDate IS null
	BEGIN
		SELECT @EffectiveDate = dbo.Today()
	END

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
	   AND o.StartDate <= @EffectiveDate AND ISNULL(o.EndDate, @FutureDate) > @EffectiveDate

