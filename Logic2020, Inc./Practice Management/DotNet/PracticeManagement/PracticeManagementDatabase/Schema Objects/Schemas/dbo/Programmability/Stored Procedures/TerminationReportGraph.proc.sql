CREATE PROCEDURE [dbo].[TerminationReportGraph]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME,
	@TimeScaleIds	XML = null,
	@SeniorityIds	XML = null,
	@TerminationReasonIds XML = NULL,
	@PracticeIds	XML = null,
	@ExcludeInternalPractices	BIT
)
AS
BEGIN
			SELECT	0 AS [ActivePersonsAtTheBeginning],
					0 AS [NewHiredInTheRange] ,
					0 AS [TerminationsInTheRange],
					@StartDate AS StartDate,
					@EndDate AS EndDate
END
