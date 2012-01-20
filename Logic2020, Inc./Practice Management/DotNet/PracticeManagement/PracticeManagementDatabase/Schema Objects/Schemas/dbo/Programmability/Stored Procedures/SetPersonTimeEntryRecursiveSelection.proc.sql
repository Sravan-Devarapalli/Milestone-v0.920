CREATE PROCEDURE [dbo].[SetPersonTimeEntryRecursiveSelection]
	@PersonId			INT,
	@ClientId			INT,
	@ProjectGroupId		INT,
	@ProjectId			INT,
	@TimeEntrySectionId	INT,
	@IsRecursive		BIT,
	@StartDate			DATETIME
AS
BEGIN
/*
if recursive or not recursive is set delete all the future reocrds

find the @recursiverecordstartdate i.e. which record need to be done be updated
 conditions
@recursiverecordstartdate =
if @startdate between startdate and enddate then startdate 
else if @startdate-1 between startdate and enddate then startdate 
else null

if @recursiverecordstartdate = null
insert
else
update @recursiverecordstartdate record
set enddate =
	if @IsRecursive = 0 then startdate+6 
        else null 
*/


--Delete all entries  after this @StartDate 
	DELETE PTRS
	FROM PersonTimeEntryRecursiveSelection PTRS
	WHERE PersonId = @PersonId 
		AND ClientId = @ClientId 
		AND ProjectGroupId = @ProjectGroupId
		AND ProjectId = @ProjectId
		AND	TimeEntrySectionId = @TimeEntrySectionId
		AND StartDate > @StartDate

	DECLARE @RecursiveRecordStartDate  DATETIME

	SELECT @RecursiveRecordStartDate  =	
					CASE WHEN @StartDate between startdate and ISNULL(enddate,dbo.GetFutureDate()) THEN startdate -- given startdate is that record need to update that record
					  	 WHEN @StartDate-1 between startdate and ISNULL(enddate,dbo.GetFutureDate())THEN startdate -- for prev record
						 ELSE NULL END  
	FROM PersonTimeEntryRecursiveSelection
	WHERE PersonId = @PersonId 
		  AND ClientId = @ClientId 
		  AND ProjectId = @ProjectId 
		  AND ProjectGroupId = @ProjectGroupId 
		  AND TimeEntrySectionId = @TimeEntrySectionId


	IF @RecursiveRecordStartDate  IS NULL
	BEGIN
		INSERT INTO PersonTimeEntryRecursiveSelection(StartDate,EndDate, PersonId, ClientId, ProjectGroupId, ProjectId, TimeEntrySectionId)
		VALUES(@StartDate,null, @PersonId, @ClientId, @ProjectGroupId, @ProjectId, @TimeEntrySectionId)
	END
	ELSE
	BEGIN
		UPDATE PTRS
		SET EndDate = CASE WHEN @IsRecursive = 0 THEN @StartDate +6
						   ELSE  NULL END
		FROM PersonTimeEntryRecursiveSelection PTRS
		WHERE PersonId = @PersonId 
		AND ClientId = @ClientId 
		AND ProjectGroupId = @ProjectGroupId
		AND ProjectId = @ProjectId
		AND TimeEntrySectionId = @TimeEntrySectionId
		AND STARTDATE = @recursiverecordstartdate  
	END
END

