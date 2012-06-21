CREATE PROCEDURE [dbo].[GetStrawmanListShortFilterWithTodayPay]
AS
BEGIN
	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATE, dbo.GettingPMTime(GETUTCDATE()))
	
	SELECT	P.PersonId,
			P.LastName,
			P.FirstName
	FROM dbo.Person P
	JOIN dbo.Pay PA ON PA.Person = P.PersonId AND @Today BETWEEN PA.StartDate AND PA.EndDate - 1
	WHERE P.IsStrawman = 1

END
