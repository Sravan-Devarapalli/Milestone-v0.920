-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 28 Sept. 2009
-- Description:	Converts opportunity history rows into string
-- =============================================
CREATE FUNCTION dbo.GetOpportunityHistory 
(
	@optId int
)
RETURNS VARCHAR(5000)
AS
BEGIN
	DECLARE @history VARCHAR(5000)
	SET @history = ''

	SELECT @history = @history + [Status] + ' ' + CONVERT(VARCHAR(20), TransitionDate, 100) + ' by ' + Responsible + ', Note: ' + NoteText + '. | '
	FROM v_OpportunityHistory
	WHERE OpportunityId = @optId
	ORDER BY TransitionDate

	RETURN @history

END

