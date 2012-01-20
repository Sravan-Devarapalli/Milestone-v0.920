CREATE PROCEDURE [dbo].[GetIsHourlyRevenueByPeriod]
(
	@PersonId INT,
	@ProjectId INT,
	@StartDate DATETIME,
	@EndDate DATETIME
)
AS
BEGIN 

	SELECT D.ChargeCodeDate
		,CONVERT(BIT,(CASE WHEN SUM(CASE WHEN ISNULL(IsHourlyAmount,1) = 1  THEN 1 ELSE 0 END) > 0 THEN 1 ELSE 0 END)) AS 'IsHourlyRevenue'
	FROM (select date AS ChargeCodeDate from dbo.Calendar where DATE BETWEEN @StartDate and @EndDate) D 
	LEFT JOIN [dbo].[Milestone] M  ON D.ChargeCodeDate BETWEEN M.StartDate AND M.ProjectedDeliveryDate  AND M.ProjectId = @ProjectId
	LEFT JOIN dbo.MilestonePerson MP ON M.MilestoneId = MP.MilestoneId AND MP.PersonId =  @PersonId
	GROUP BY D.ChargeCodeDate
		

END

