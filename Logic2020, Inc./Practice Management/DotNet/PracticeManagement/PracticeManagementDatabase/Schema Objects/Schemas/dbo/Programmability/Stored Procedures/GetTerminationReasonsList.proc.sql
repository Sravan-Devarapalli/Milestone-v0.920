CREATE PROCEDURE [dbo].[GetTerminationReasonsList]
AS
BEGIN
	SET NOCOUNT ON;
	
	SELECT TR.TerminationReasonId, TR.TerminationReason
	FROM dbo.TerminationReasons TR
	ORDER BY TR.TerminationReason

END
