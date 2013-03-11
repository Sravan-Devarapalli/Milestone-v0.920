CREATE NONCLUSTERED INDEX IX_Milestone_Index1 ON [dbo].[Milestone] 
(
	[ProjectId] ASC,
	[MilestoneId] ASC,
	[IsHourlyAmount] ASC,
	[StartDate] ASC,
	[ProjectedDeliveryDate] ASC
)
INCLUDE ( [Amount]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

