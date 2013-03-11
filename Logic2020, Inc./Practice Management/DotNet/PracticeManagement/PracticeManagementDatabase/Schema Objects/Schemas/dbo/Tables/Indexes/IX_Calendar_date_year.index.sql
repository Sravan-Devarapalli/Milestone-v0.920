﻿CREATE NONCLUSTERED INDEX [IX_Calendar_date_year] ON [dbo].[Calendar] 
(
	[Date] ASC,
	[Year] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
