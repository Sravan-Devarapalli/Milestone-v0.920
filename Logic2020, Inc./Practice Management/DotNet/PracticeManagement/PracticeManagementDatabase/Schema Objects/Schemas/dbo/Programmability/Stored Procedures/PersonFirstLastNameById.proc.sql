-- =============================================
-- Updated by : Sainath.CH
-- Update Date: 03-29-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonFirstLastNameById]
	@PersonId int
AS
BEGIN
	DECLARE @NOW DATETIME 
	SELECT @NOW = dbo.GettingPMTime(GETUTCDATE())

	SELECT	P.FirstName,
			P.LastName,
			P.IsStrawman,
			TS.Name AS Timescale
	FROM dbo.Person P
	LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate,dbo.GetFutureDate()) 
	LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
	WHERE PersonId = @PersonId
END
