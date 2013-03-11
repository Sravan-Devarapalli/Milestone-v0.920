CREATE NONCLUSTERED INDEX IX_TimeEntry_ChargeCodeDateWithOtherColoumns ON [dbo].[TimeEntry] 
(
[ChargeCodeDate]
)
INCLUDE ([TimeEntryId],[PersonId],[ChargeCodeId]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


