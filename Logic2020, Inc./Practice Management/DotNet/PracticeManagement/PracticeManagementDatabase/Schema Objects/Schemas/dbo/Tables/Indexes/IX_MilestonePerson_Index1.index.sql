CREATE NONCLUSTERED INDEX IX_MilestonePerson_Index1 ON [dbo].[MilestonePerson] 
(
	[MilestoneId] ASC
	
)INCLUDE ([PersonId] ,
	[MilestonePersonId] ) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]

