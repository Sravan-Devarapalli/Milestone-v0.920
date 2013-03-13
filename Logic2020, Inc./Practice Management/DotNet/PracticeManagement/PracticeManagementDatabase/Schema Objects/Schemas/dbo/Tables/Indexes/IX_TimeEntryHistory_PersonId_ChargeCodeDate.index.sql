CREATE NONCLUSTERED INDEX IX_TimeEntryHistory_PersonId_ChargeCodeDate
ON [dbo].[TimeEntryHistory] ([PersonId],ChargeCodeDate)
