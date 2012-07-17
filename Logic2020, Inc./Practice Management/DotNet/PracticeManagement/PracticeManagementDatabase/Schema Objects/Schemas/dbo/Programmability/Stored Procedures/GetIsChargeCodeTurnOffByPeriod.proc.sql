CREATE PROCEDURE [dbo].[GetIsChargeCodeTurnOffByPeriod]
(
	@PersonId INT,
	@ClientId INT,
	@ProjectGroupId INT,
	@ProjectId INT,
	@TimeTypeId INT,
	@StartDate DATETIME,
	@EndDate DATETIME
)
AS
BEGIN 
	DECLARE @False BIT,@True BIT,@FutureDate DATETIME
	SELECT @False = CONVERT(BIT,0) ,@True = CONVERT(BIT,1),@FutureDate = dbo.GetFutureDate()

	SELECT D.ChargeCodeDate
		,CASE WHEN CCH.ChargeCodeId IS NULL THEN @False ELSE @True END AS [IsChargeCodeOff]
	FROM (SELECT DATE AS ChargeCodeDate FROM dbo.Calendar WHERE DATE BETWEEN @StartDate and @EndDate) D 
	LEFT JOIN dbo.ChargeCode CC ON CC.ClientId = @ClientId AND 	CC.ProjectGroupId = @ProjectGroupId AND CC.ProjectId = @ProjectId AND CC.TimeTypeId = @TimeTypeId
	LEFT JOIN dbo.ChargeCodeTurnOffHistory CCH ON CC.Id = CCH.ChargeCodeId AND D.ChargeCodeDate BETWEEN CCH.StartDate AND ISNULL(CCH.EndDate,@FutureDate)

END

