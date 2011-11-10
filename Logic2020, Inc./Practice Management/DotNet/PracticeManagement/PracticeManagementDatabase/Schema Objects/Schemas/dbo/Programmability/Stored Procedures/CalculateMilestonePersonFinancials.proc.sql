CREATE PROCEDURE dbo.CalculateMilestonePersonFinancials 
(
	@MilestonePersonId int
)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	DECLARE @milestoneId INT
	DECLARE @projectDiscount DECIMAL(18, 2)
	DECLARE @personId INT
	DECLARE @grossHourlyBillRate  DECIMAL(18, 2)
	DECLARE @loadedHourlyPayRate  DECIMAL(18, 2)

	SELECT TOP 1 @milestoneId = mp.MilestoneId, @projectDiscount = p.Discount, @personId = mp.PersonId
	FROM dbo.MilestonePerson AS mp
	INNER JOIN dbo.Milestone AS m ON mp.MilestoneId = m.MilestoneId
	INNER JOIN dbo.Project AS p ON m.ProjectId = p.ProjectId
	WHERE mp.MilestonePersonId = @MilestonePersonId

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.MilestoneId,
		   f.Date, 
		   f.EntryStartDate,
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
		   	+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount,
		   f.EntryId
	FROM v_FinancialsRetrospective f
	WHERE f.MilestoneId = @MilestoneId AND f.PersonId = @PersonId     
	) ,
	MilestonePersonBasicFinancials AS (
	SELECT f.MilestoneId ,
		   f.PersonId,
		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount),0) AS RevenueNet,
		   ISNULL(SUM((CASE WHEN f.SLHR >=  f.PayRate +f.MLFOverheadRate 
					 THEN f.SLHR ELSE f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,
		   ISNULL(SUM(f.PersonHoursPerDay), 0) AS BilledHours       
	  FROM FinancialsRetro AS f
	 WHERE f.MilestoneId = @MilestoneId AND f.PersonId = @PersonId
	GROUP BY  f.MilestoneId , f.PersonId
	)
	SELECT
		@grossHourlyBillRate = 
			SUM(
				CASE 
				WHEN BilledHours > 0 
					THEN RevenueNet / BilledHours
					ELSE 0
				END
			),
		@loadedHourlyPayRate = 
			SUM(
				CASE 
				WHEN BilledHours > 0 
					THEN Cogs / BilledHours
					ELSE 0
				END
			)
	FROM MilestonePersonBasicFinancials
	
    -- Insert statements for procedure here
	SELECT 
		dbo.GetMilestonePersonHoursInPeriod(@MilestonePersonId) as HoursInPeriod, 
		@projectDiscount AS ProjectDiscount,
		@grossHourlyBillRate AS GrossHourlyBillRate,
		@loadedHourlyPayRate LoadedHourlyPayRate
END

