CREATE NONCLUSTERED INDEX [IX_Pay_Index2] ON [dbo].[Pay] 
(
	[Person] ASC,
	[EndDate] ASC,
	[StartDate] ASC,
	[Timescale] ASC,
	[BonusHoursToCollect] ASC,
	[BonusAmount] ASC,
	[VacationDays] ASC
)
INCLUDE ( [Amount]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
