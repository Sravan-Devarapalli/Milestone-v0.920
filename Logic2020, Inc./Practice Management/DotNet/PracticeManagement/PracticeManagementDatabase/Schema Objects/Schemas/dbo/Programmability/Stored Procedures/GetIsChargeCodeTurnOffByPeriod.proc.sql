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

	SELECT D.ChargeCodeDate
		,CASE WHEN CCH.ChargeCodeId IS NULL THEN CONVERT(BIT,0) ELSE CONVERT(BIT,1) END AS 'IsChargeCodeOff'
	FROM (select date AS ChargeCodeDate from dbo.Calendar where DATE BETWEEN @StartDate and @EndDate) D 
	LEFT JOIN dbo.ChargeCode CC ON CC.ClientId = @ClientId AND 	CC.ProjectGroupId = @ProjectGroupId AND CC.ProjectId = @ProjectId AND CC.TimeTypeId = @TimeTypeId
	LEFT JOIN dbo.ChargeCodeTurnOffHistory CCH ON CC.Id = CCH.ChargeCodeId AND D.ChargeCodeDate BETWEEN CCH.StartDate AND ISNULL(CCH.EndDate,dbo.GetFutureDate())

END

