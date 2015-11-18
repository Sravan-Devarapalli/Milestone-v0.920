CREATE NONCLUSTERED INDEX [IX_BadgeHistoryForReports_Index1] ON [dbo].[BadgeHistoryForReports]
(
	PersonId ASC
)INCLUDE ( [BadgeStartDate]) WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]


