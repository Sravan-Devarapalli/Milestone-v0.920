CREATE PROCEDURE [dbo].[GetTerminationReasonsList]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT TR.TerminationReasonId
			, TR.TerminationReason
			, TR.IsW2SalaryRule
			, TR.IsW2HourlyRule
			, TR.Is1099Rule
			, TR.IsContingentRule
			, TR.IsVisible
	FROM dbo.TerminationReasons TR
	ORDER BY TR.TerminationReason

END
