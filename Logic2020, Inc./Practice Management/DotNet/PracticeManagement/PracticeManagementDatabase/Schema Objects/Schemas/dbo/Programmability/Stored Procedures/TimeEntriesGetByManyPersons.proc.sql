CREATE PROCEDURE [dbo].[TimeEntriesGetByManyPersons]
	@PersonIds	NVARCHAR(MAX),
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimescaleIds NVARCHAR(4000)= NULL,
	@PracticeIds  NVARCHAR(MAX)
	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Convert project owner ids from string to table
	DECLARE @PersonList table (Id int)
	insert into @PersonList
	select * FROM dbo.ConvertStringListIntoTable(@PersonIds)
	
	--@TimescaleIds is null means all timescales.
	IF @TimescaleIds IS NOT NULL
	BEGIN
		DECLARE @TimescaleIdList TABLE (Id INT)
		INSERT INTO @TimescaleIdList
		select * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
	END
	
	declare @PracticeIdsList table (Id int)
	insert into @PracticeIdsList
	select * FROM dbo.ConvertStringListIntoTable(@PracticeIds)
	
	;WITH PersonsFilteredByPersonIdsAndPayIds AS
	(
		SELECT Distinct P.PersonId
						, P.FirstName
						, P.LastName
		FROM Person P
		JOIN @PersonList PL ON PL.Id = P.PersonId
		LEFT JOIN Pay pa ON pa.Person = P.PersonId AND pa.StartDate <= @EndDate AND (ISNULL(pa.EndDate, dbo.GetFutureDate()) -1) >= @StartDate
		WHERE (@TimescaleIds IS NULL OR pa.Timescale IN (SELECT Id FROM @TimescaleIdList)) 
		      AND (@PracticeIds IS NULL) OR ISNULL(pa.PracticeId,P.DefaultPractice) IN (SELECT Id FROM @PracticeIdsList)
	)

	SELECT DISTINCT p.PersonId,
		   p.FirstName ObjectFirstName,
		   p.LastName ObjectLastName,
		   proj.ProjectId,
		   proj.ProjectNumber,
		   proj.Name ProjectName,
		   c.Name ClientName,
		   tt.Name TimeTypeName,
		   te.Note ,
		   te.MilestoneDate,
		   te.ActualHours,
		   mp.MilestonePersonId
	FROM PersonsFilteredByPersonIdsAndPayIds p
	JOIN dbo.MilestonePerson mp on mp.PersonId = p.PersonId
	JOIN dbo.MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
	JOIN dbo.Milestone m on m.MilestoneId = mp.MilestoneId
	JOIN dbo.Project proj on proj.ProjectId = m.ProjectId
	JOIN dbo.Client c on proj.ClientId = c.ClientId
	JOIN dbo.TimeEntries te on te.MilestonePersonId = mp.MilestonePersonId
									and te.MilestoneDate BETWEEN ISNULL(@StartDate, te.MilestoneDate) and ISNULL(@EndDate, te.MilestoneDate)
	LEFT JOIN dbo.TimeType tt on tt.TimeTypeId = te.TimeTypeId
	WHERE (ISNULL(@StartDate,Mpe.StartDate) >= mpe.StartDate AND ISNULL(@EndDate,mpe.EndDate) <= mpe.EndDate
			OR ISNULL(@StartDate,Mpe.StartDate - 1) < mpe.StartDate AND ISNULL(@EndDate,mpe.StartDate) >= mpe.StartDate
			OR ISNULL(@StartDate,Mpe.EndDate) <= mpe.EndDate AND ISNULL(@EndDate,mpe.EndDate + 1) >= mpe.EndDate
			)
		AND (te.TimeEntryId is Not null or proj.ProjectStatusId = 3) --For getting only active projects or there are timeentries for projects.
	ORDER BY proj.ProjectId, te.MilestoneDate
	 
END
