CREATE PROCEDURE [dbo].[TimeEntryHoursByManyPersonsProject]
	@PersonIds	varchar(max),
	@StartDate	datetime = NULL,
	@EndDate	datetime = NULL,
	@TimescaleIds NVARCHAR(4000),
	@PracticeIds  NVARCHAR(MAX)
AS
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
	
 	select  te.MilestoneDate, 
 			te.ProjectId ,
 			te.PersonId,
			CONVERT(INT,RANK() OVER(ORDER BY te.projectId,te.TimeTypeId)) as [Id],
 			te.ProjectName as [Name],
 			te.TimeTypeName, 
 			SUM(te.ActualHours) as ActualHours
	from v_TimeEntries as te
	join dbo.Person AS p on te.PersonId = p.PersonId
	where te.MilestoneDate between @StartDate and @EndDate
			and te.PersonId in (select Id from @PersonList)
			and (dbo.GetCurrentPayType(te.PersonId) IN (select Id from @TimescaleIdList)  OR dbo.GetCurrentPayType(te.PersonId) IS NULL)
			and p.DefaultPractice in (SELECT id FROM @PracticeIdsList)
	group by te.MilestoneDate, te.ProjectId, te.ProjectName,te.PersonId, te.TimeTypeId,te.TimeTypeName
	order by te.ProjectId
RETURN 0
