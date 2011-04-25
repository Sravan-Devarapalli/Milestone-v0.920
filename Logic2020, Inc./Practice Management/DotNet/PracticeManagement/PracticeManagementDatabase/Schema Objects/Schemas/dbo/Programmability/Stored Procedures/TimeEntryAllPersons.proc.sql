-- =============================================
-- Author:		Nikita G.
-- Create date: 2009-12-25
-- Description:	Retrieves all persons that have entered TE at least once
-- =============================================
CREATE  PROCEDURE dbo.TimeEntryAllPersons
	@EntryStartDate DATETIME = NULL,
	@EntryEndDate DATETIME = NULL
AS
BEGIN
	SET NOCOUNT ON;
	
    SELECT  DISTINCT objp.PersonId,
            objp.FirstName AS 'ObjectFirstName',
            objp.LastName AS 'ObjectLastName'
    FROM    TimeEntries AS te
            INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = te.MilestonePersonId
            INNER JOIN dbo.MilestonePersonEntry AS mpe ON mp.MilestonePersonId = mpe.MilestonePersonId
            INNER JOIN dbo.Person AS objp ON objp.PersonId = mp.PersonId
	WHERE (te.MilestoneDate BETWEEN @EntryStartDate AND @EntryEndDate) OR @EntryStartDate IS NULL OR @EntryEndDate IS NULL
END

