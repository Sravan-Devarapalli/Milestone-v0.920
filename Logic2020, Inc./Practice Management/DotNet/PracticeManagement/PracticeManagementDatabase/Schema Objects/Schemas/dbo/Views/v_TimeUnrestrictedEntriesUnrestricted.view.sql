CREATE VIEW [dbo].[v_TimeUnrestrictedEntriesUnrestricted]
AS
SELECT  DISTINCT   te.TimeEntryId, te.EntryDate, te.ModifiedDate, te.MilestonePersonId, te.ActualHours, te.ForecastedHours, te.TimeTypeId, te.ModifiedBy, te.Note, 
                      te.IsReviewed, te.IsChargeable, te.MilestoneDate, m.Description AS 'MilestoneName', mpe.StartDate, mpe.EndDate, 
                      mpe.HoursPerDay, m.MilestoneId
FROM         dbo.TimeEntries AS te INNER JOIN
                      dbo.MilestonePerson AS mp ON mp.MilestonePersonId = te.MilestonePersonId LEFT JOIN
                      dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId INNER JOIN
                      dbo.Milestone AS m ON m.MilestoneId = mp.MilestoneId 
