CREATE STATISTICS STPay_StartDate_Person_EndDate_Timescale_BonusHoursToCollect_BonusAmount_VacationDays 
	ON [dbo].[Pay]([StartDate], [Person], [EndDate], [Timescale], [BonusHoursToCollect], [BonusAmount], [VacationDays])

