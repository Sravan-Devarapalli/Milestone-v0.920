CREATE PROCEDURE dbo.UpdatePersonSeniorityPracticeFromCurrentPay
AS
BEGIN
	DECLARE @Today DATETIME
	SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETDATE())))

	UPDATE P
	SET P.SeniorityId = Pa.SeniorityId,
		P.DefaultPractice = Pa.PracticeId
	FROM dbo.Person P
	JOIN dbo.Pay Pa
	ON P.PersonId = Pa.Person AND 
		pa.StartDate <= @Today AND ISNULL(EndDate,dbo.GetFutureDate()) > @Today

	UPDATE dbo.Pay
	SET IsActivePay = CASE WHEN StartDate <= @Today AND  EndDate > @Today
							   THEN 1 ELSE 0 END
END
