CREATE NONCLUSTERED INDEX IX_PersonCalendarAuto_PersonId_Date1
ON [dbo].[PersonCalendarAuto] ([PersonId] ASC,[Date] ASC)
INCLUDE ([DayOff],[TimeOffHours],[CompanyDayOff])


