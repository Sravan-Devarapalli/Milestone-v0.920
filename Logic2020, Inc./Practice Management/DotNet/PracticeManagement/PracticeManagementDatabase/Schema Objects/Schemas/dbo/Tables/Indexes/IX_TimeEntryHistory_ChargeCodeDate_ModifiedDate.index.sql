CREATE NONCLUSTERED INDEX IX_TimeEntryHistory_ChargeCodeDate_ModifiedDate
ON [dbo].[TimeEntryHistory] ([ChargeCodeDate],[ModifiedDate])
INCLUDE ([PersonId],[OldHours],[ActualHours])
