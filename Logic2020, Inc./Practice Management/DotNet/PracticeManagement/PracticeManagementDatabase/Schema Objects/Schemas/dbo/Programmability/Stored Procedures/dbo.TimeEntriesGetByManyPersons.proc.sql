CREATE PROCEDURE [dbo].[TimeEntriesGetByManyPersons]
	@PersonIds	varchar(max),
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL,
	@TimescaleIds nvarchar(4000),
	@PracticeIds  nvarchar(MAX)
	
AS
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	-- Convert project owner ids from string to table
	declare @PersonList table (Id int)
	insert into @PersonList
	select * FROM dbo.ConvertStringListIntoTable(@PersonIds)
	
	declare @TimescaleIdList table (Id int)
	insert into @TimescaleIdList
	select * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
	
	declare @PracticeIdsList table (Id int)
	insert into @PracticeIdsList
	select * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

	select p.PersonId,
		   p.FirstName ObjectFirstName,
		   p.LastName ObjectLastName,
		   proj.ProjectId,
		   proj.ProjectNumber,
		   proj.Name ProjectName,
		   tt.Name TimeTypeName,
		   te.Note ,
		   te.MilestoneDate,
		   te.ActualHours,
		   mp.MilestonePersonId
	from dbo.Person p
	join dbo.MilestonePerson mp on mp.PersonId = p.PersonId
	join dbo.MilestonePersonEntry mpe on mpe.MilestonePersonId = mp.MilestonePersonId
	join dbo.Milestone m on m.MilestoneId = mp.MilestoneId
	join dbo.Project proj on proj.ProjectId = m.ProjectId
	left join dbo.TimeEntries te on te.MilestonePersonId = mp.MilestonePersonId
									and te.MilestoneDate between ISNULL(@StartDate, te.MilestoneDate) and ISNULL(@EndDate, te.MilestoneDate)
	left join dbo.TimeType tt on tt.TimeTypeId = te.TimeTypeId
	where p.PersonId in (select Id from @PersonList)
		and	(ISNULL(@StartDate,Mpe.StartDate) >= mpe.StartDate AND ISNULL(@EndDate,mpe.EndDate) <= mpe.EndDate
			OR ISNULL(@StartDate,Mpe.StartDate - 1) < mpe.StartDate AND ISNULL(@EndDate,mpe.StartDate) >= mpe.StartDate
			OR ISNULL(@StartDate,Mpe.EndDate) <= mpe.EndDate AND ISNULL(@EndDate,mpe.EndDate + 1) >= mpe.EndDate
			)
		and (dbo.GetCurrentPayType(p.PersonId) in (select Id from @TimescaleIdList)  or dbo.GetCurrentPayType(p.PersonId) is null)
		and p.DefaultPractice in (SELECT id FROM @PracticeIdsList)
		and (te.TimeEntryId is Not null or proj.ProjectStatusId = 3) --For getting only active projects or there are timeentries for projects.
	order by proj.ProjectId, te.MilestoneDate
	 
RETURN 0
