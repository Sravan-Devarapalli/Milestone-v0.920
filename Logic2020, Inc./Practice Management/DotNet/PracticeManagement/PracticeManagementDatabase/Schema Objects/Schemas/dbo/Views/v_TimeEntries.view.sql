
/*INNER JOIN dbo.Client AS clnt ON proj.ClientId = clnt.ClientId*/
CREATE VIEW dbo.v_TimeEntries
AS
SELECT  DISTINCT   te.TimeEntryId, te.EntryDate, te.ModifiedDate, te.MilestonePersonId, te.ActualHours, te.ForecastedHours, te.TimeTypeId, tt.Name AS 'TimeTypeName', te.ModifiedBy, te.Note, 
                      te.IsReviewed, te.IsChargeable, te.MilestoneDate, modp.FirstName AS 'ModifiedByFirstName', modp.LastName AS 'ModifiedByLastName', objp.PersonId, 
                      objp.FirstName AS 'ObjectFirstName', objp.LastName AS 'ObjectLastName', m.Description AS 'MilestoneName', mpe.StartDate, mpe.EndDate, 
                      mpe.HoursPerDay, m.MilestoneId, proj.ProjectId, proj.Name AS 'ProjectName', proj.ProjectNumber, te.IsCorrect, c.ClientId, c.[Name] AS 'ClientName', proj.IsChargeable as 'IsProjectChargeable'
FROM         dbo.TimeEntries AS te INNER JOIN
                      dbo.Person AS modp ON modp.PersonId = te.ModifiedBy INNER JOIN
                      dbo.MilestonePerson AS mp ON mp.MilestonePersonId = te.MilestonePersonId INNER JOIN
                      dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId INNER JOIN
                      dbo.Person AS objp ON objp.PersonId = mp.PersonId INNER JOIN
                      dbo.Milestone AS m ON m.MilestoneId = mp.MilestoneId INNER JOIN
                      dbo.Project AS proj ON proj.ProjectId = m.ProjectId INNER JOIN
                      dbo.TimeType AS tt ON tt.TimeTypeId = te.TimeTypeId INNER JOIN
                      dbo.Client AS c ON proj.ClientId = c.ClientId
WHERE te.MilestoneDate BETWEEN mpe.StartDate AND ISNULL(mpe.EndDate, m.ProjectedDeliveryDate)

