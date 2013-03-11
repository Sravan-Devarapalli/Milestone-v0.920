CREATE NONCLUSTERED INDEX IX_TimeEntryHours_Index1 ON [dbo].[TimeEntryHours] 
(
	[TimeEntryId] ASC
)
INCLUDE ( [ActualHours],
[IsChargeable]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
