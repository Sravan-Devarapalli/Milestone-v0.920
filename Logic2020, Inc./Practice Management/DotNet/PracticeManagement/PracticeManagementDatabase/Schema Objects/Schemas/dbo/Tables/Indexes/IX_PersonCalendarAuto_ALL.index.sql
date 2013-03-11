CREATE NONCLUSTERED INDEX IX_PersonCalendarAuto_ALL ON [dbo].[PersonCalendarAuto] ([DayOff],companyDayOff,TimeoffHours)
INCLUDE ([Date],[PersonId])WITH (SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF) ON [PRIMARY]
