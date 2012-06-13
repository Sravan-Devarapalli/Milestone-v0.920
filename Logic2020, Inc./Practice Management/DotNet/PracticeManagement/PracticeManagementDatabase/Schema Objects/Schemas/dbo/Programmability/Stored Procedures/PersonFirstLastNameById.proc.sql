﻿-- =============================================
-- Updated by : Sainath.CH
-- Update Date: 05-31-2012
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
			TS.Name AS Timescale,
			P.IsOffshore,
			P.PersonStatusId,
			ISNULL(P.Alias,'') AS Alias
	FROM dbo.Person P
	LEFT JOIN dbo.Pay PA ON PA.Person = P.PersonId 
							AND @NOW BETWEEN PA.StartDate  AND ISNULL(PA.EndDate-1,dbo.GetFutureDate()) 
	LEFT JOIN dbo.Timescale TS ON PA.Timescale = TS.TimescaleId
	WHERE PersonId = @PersonId
END
