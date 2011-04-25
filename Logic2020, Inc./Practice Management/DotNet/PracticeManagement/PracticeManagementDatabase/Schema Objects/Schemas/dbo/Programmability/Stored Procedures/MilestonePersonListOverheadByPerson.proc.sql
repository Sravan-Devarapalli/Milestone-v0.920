-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 6-27-2008
-- Updated by:	Anatoliy Lokshin
-- Update date:	7-22-2008
-- Description:	Selects an overhead for the person on the milstone grouped by days.
-- =============================================
CREATE PROCEDURE [dbo].[MilestonePersonListOverheadByPerson]
(
	@PersonId      INT,
	@MilestoneId   INT
)
AS
	SET NOCOUNT ON

	SELECT mp.PersonId, mp.MilestoneId,
	       mp.Date AS StartDate, mp.Date AS EndDate,
	       ISNULL(SUM(HourlyRate * HoursPerDay), 0) AS Rate,
	       'Aggregated Daily' AS [Description],
	       CAST(ROUND(HoursPerDay, 0) AS INT) AS HoursToCollect,
	       CAST(0 AS BIT) AS IsPercentage,
	       NULL AS OverheadRateTypeId,
	       NULL AS OverheadRateTypeName,
	       CAST(0 AS DECIMAL) AS BillRateMultiplier
	  FROM dbo.v_MilestonePersonSchedule AS mp
	       INNER JOIN dbo.v_PersonOverheadRetrospective AS o ON mp.PersonId = o.PersonId AND mp.Date = o.Date
	 WHERE mp.PersonId = @PersonId AND mp.MilestoneId = @MilestoneId
	GROUP BY mp.PersonId, mp.MilestoneId, mp.Date, mp.HoursPerDay
	ORDER BY StartDate

