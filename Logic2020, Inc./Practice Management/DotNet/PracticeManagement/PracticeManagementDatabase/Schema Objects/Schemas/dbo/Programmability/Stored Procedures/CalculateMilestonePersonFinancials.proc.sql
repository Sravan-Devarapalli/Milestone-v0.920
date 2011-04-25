-- =============================================
-- Author:		Nikita Goncharenko
-- Create date: 2010-08-04
-- Description:	
-- =============================================
CREATE PROCEDURE dbo.CalculateMilestonePersonFinancials 
	-- Add the parameters for the stored procedure here
	@MilestonePersonId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	declare @milestoneId int
	declare @projectDiscount decimal(18, 2)
	declare @personId int
	declare @grossHourlyBillRate  decimal(18, 2)
	declare @loadedHourlyPayRate  decimal(18, 2)

	select top 1 @milestoneId = mp.MilestoneId, @projectDiscount = mp.Discount, @personId = mp.PersonId
	from v_MilestonePerson as mp
	where mp.MilestonePersonId = @MilestonePersonId

	;WITH FinancialsRetro AS 
	(
	SELECT f.ProjectId,
		   f.PersonId,
		   f.MilestoneId,
		   f.Date, 
		   f.EntryStartDate,
		   f.PersonMilestoneDailyAmount,
		   f.PersonDiscountDailyAmount,
		   CASE WHEN ISNULL(f.PersonHoursPerDay,0) = 0 THEN 0
				ELSE 
					ISNULL(
							( ((f.PersonMilestoneDailyAmount-f.PersonDiscountDailyAmount)/f.PersonHoursPerDay)-
								(ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+f.BonusRate+f.VacationRate + ISNULL(f.RecruitingCommissionRate,0))
							)* (SELECT SUM(c.FractionOfMargin) 
							  FROM dbo.Commission AS  c 
							  WHERE c.ProjectId = f.ProjectId 
									AND c.CommissionType = 1
								)/100,0
							)
				END SCPH
			/*
		   GrossMargin-Semi = Revenue-SCogs
		   SCPH = GrossMargin-Semi*SC%
		   */,
		   (ISNULL(f.PayRate, 0) + ISNULL(f.OverheadRate, 0)+ISNULL(f.BonusRate,0)+ISNULL(f.VacationRate,0)
		   	+ISNULL(f.RecruitingCommissionRate,0)) SLHR,
		   ISNULL(f.PayRate,0) PayRate,
		   f.MLFOverheadRate,
		   f.PersonHoursPerDay,
		   f.PracticeManagementCommissionSub,
		   f.PracticeManagementCommissionOwn ,
		   f.PracticeManagerId,
		   f.Discount
	FROM v_FinancialsRetrospective f
	WHERE f.MilestoneId = @MilestoneId AND f.PersonId = @PersonId     
	) ,
	MilestonePersonBasicFinancials AS (
	SELECT f.EntryStartDate,
		   ISNULL(SUM(f.PersonMilestoneDailyAmount - f.PersonDiscountDailyAmount),0) AS RevenueNet,
		   ISNULL(SUM((CASE WHEN f.SLHR+f.SCPH >=  f.PayRate +f.MLFOverheadRate 
					 THEN f.SLHR+f.SCPH ELSE f.PayRate + f.MLFOverheadRate END)*ISNULL(f.PersonHoursPerDay, 0)),0) Cogs,
		   ISNULL(SUM(f.PersonHoursPerDay), 0) AS BilledHours       
	  FROM FinancialsRetro AS f
	  LEFT JOIN dbo.v_MilestonePersonVacations AS vac ON f.MilestoneId = vac.MilestoneId AND f.PersonId = vac.PersonId AND vac.StartDate = f.EntryStartDate
	 WHERE f.MilestoneId = @MilestoneId AND f.PersonId = @PersonId
	GROUP BY f.EntryStartDate
	)
	select
		@grossHourlyBillRate = 
			SUM(
				case 
				when BilledHours > 0 
					then RevenueNet / BilledHours
					else 0
				end
			),
		@loadedHourlyPayRate = 
			SUM(
				case 
				when BilledHours > 0 
					then Cogs / BilledHours
					else 0
				end
			)
	from MilestonePersonBasicFinancials
	
    -- Insert statements for procedure here
	SELECT 
		dbo.GetMilestonePersonHoursInPeriod(@MilestonePersonId) as HoursInPeriod, 
		@projectDiscount as ProjectDiscount,
		@grossHourlyBillRate as GrossHourlyBillRate,
		@loadedHourlyPayRate LoadedHourlyPayRate
END

