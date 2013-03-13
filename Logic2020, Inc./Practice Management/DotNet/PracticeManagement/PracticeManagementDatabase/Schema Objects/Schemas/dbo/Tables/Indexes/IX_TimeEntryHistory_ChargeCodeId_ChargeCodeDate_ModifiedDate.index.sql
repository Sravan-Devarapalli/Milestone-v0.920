CREATE NONCLUSTERED INDEX IX_TimeEntryHistory_ChargeCodeId_ChargeCodeDate_ModifiedDate
ON [dbo].[TimeEntryHistory] ([ChargeCodeId],[ChargeCodeDate],[ModifiedDate])
INCLUDE ([PersonId],[Note],[OldHours],[ActualHours],[IsChargeable])
