CREATE NONCLUSTERED INDEX [IX_MilestonePersonEntry_index1] ON [dbo].[MilestonePersonEntry] 
(
	[MilestonePersonId] ASC,
	[StartDate] ASC,
	[EndDate] ASC,
	[HoursPerDay] ASC
)WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

