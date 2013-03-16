CREATE NONCLUSTERED INDEX [IX_Calendar_year_daysinyear] ON [dbo].[Calendar] 
(
	[Year] ASC,
	daysinyear ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]



