CREATE PROCEDURE [dbo].[TimeEntriesGetByPersonsForExcel]
	@PersonIds	NVARCHAR(MAX),
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimescaleIds NVARCHAR(4000),
	@PracticeIds  NVARCHAR(MAX)
	
AS
BEGIN
    
	DECLARE @PersonList TABLE (Id INT)
	INSERT INTO @PersonList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PersonIds)
	
	DECLARE @TimescaleIdList TABLE (Id INT)
	INSERT INTO @TimescaleIdList
	SELECT * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
	
	DECLARE @PracticeIdsList TABLE (Id INT)
	INSERT INTO @PracticeIdsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	SELECT DISTINCT 
		   p.LastName +', ' +p.FirstName Name,
		   proj.ProjectNumber AS 'P#',
		   c.Name Client,
		   proj.Name AS 'Project Name',
		   m.Description AS Milestone,
		   tt.Name TimeType,
		   te.Note Note,
		   CONVERT(VARCHAR(10), te.MilestoneDate, 1)  AS 'Date',
		   te.ActualHours AS 'Hours'
	FROM dbo.Person p
	JOIN dbo.MilestonePerson mp on mp.PersonId = p.PersonId
	JOIN dbo.MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
	JOIN dbo.Milestone m on m.MilestoneId = mp.MilestoneId
	JOIN dbo.Project proj on proj.ProjectId = m.ProjectId
	JOIN dbo.Client c on proj.ClientId = c.ClientId
	JOIN dbo.TimeEntries te on te.MilestonePersonId = mp.MilestonePersonId
									AND te.MilestoneDate BETWEEN ISNULL(@StartDate, te.MilestoneDate) AND ISNULL(@EndDate, te.MilestoneDate)
	LEFT JOIN dbo.TimeType tt on tt.TimeTypeId = te.TimeTypeId
	WHERE p.PersonId IN (select Id from @PersonList)
		AND	(ISNULL(@StartDate,Mpe.StartDate) >= mpe.StartDate AND ISNULL(@EndDate,mpe.EndDate) <= mpe.EndDate
			OR ISNULL(@StartDate,Mpe.StartDate - 1) < mpe.StartDate AND ISNULL(@EndDate,mpe.StartDate) >= mpe.StartDate
			OR ISNULL(@StartDate,Mpe.EndDate) <= mpe.EndDate AND ISNULL(@EndDate,mpe.EndDate + 1) >= mpe.EndDate
			)
		AND (dbo.GetCurrentPayType(p.PersonId) IN (select Id from @TimescaleIdList)  OR dbo.GetCurrentPayType(p.PersonId) IS NULL)
		AND p.DefaultPractice IN (SELECT id FROM @PracticeIdsList)
		AND (te.TimeEntryId IS NOT NULL OR proj.ProjectStatusId = 3) --For getting only active projects or there are timeentries for projects.
	ORDER BY Name ,[Date], proj.ProjectNumber 

END
