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

find the projectenddateWeekday  
check whether projectenddateWeekday is before the startdate if yes raise error

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
DECLARE @ProjectEndDateWeekday DATETIME ,@ErrorMessage	NVARCHAR(MAX)
BEGIN TRY
	BEGIN TRAN tran_PersonTERecurSel
--get the project enddate
SELECT @ProjectEndDateWeekday = EndDate+(7-DATEPART(dw,EndDate))
	  FROM [dbo].[Project]
	  WHERE ProjectId = @ProjectId
  IF @ProjectEndDateWeekday < @StartDate
  BEGIN
  SET @ErrorMessage = 'Can not enable recurring behavior as project enddate is less than the week startdate.'
	RAISERROR (@ErrorMessage, 16, 1) 
  END 

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
					CASE WHEN @StartDate BETWEEN startdate AND ISNULL(enddate,dbo.GetFutureDate()) THEN startdate -- given startdate is that record need to update that record
					  	 WHEN @StartDate-1 BETWEEN startdate AND ISNULL(enddate,dbo.GetFutureDate())THEN startdate -- for prev record
						 ELSE NULL END  
	FROM PersonTimeEntryRecursiveSelection
	WHERE PersonId = @PersonId 
		  AND ClientId = @ClientId 
		  AND ProjectId = @ProjectId 
		  AND ProjectGroupId = @ProjectGroupId 
		  AND TimeEntrySectionId = @TimeEntrySectionId


	IF @RecursiveRecordStartDate  IS NULL
	BEGIN
		INSERT INTO PersonTimeEntryRecursiveSelection(StartDate,EndDate, PersonId, ClientId, ProjectGroupId, ProjectId, TimeEntrySectionId,IsRecursive)
		VALUES(@StartDate,@ProjectEndDateWeekday, @PersonId, @ClientId, @ProjectGroupId, @ProjectId, @TimeEntrySectionId,1)
	END
	ELSE
	BEGIN
		UPDATE PTRS
		SET EndDate = CASE WHEN @IsRecursive = 0 THEN @StartDate +6
						   ELSE  @ProjectEndDateWeekday END,
			IsRecursive = CASE WHEN @IsRecursive = 0 THEN 0 ELSE 1 END
		FROM PersonTimeEntryRecursiveSelection PTRS
		WHERE PersonId = @PersonId 
		AND ClientId = @ClientId 
		AND ProjectGroupId = @ProjectGroupId
		AND ProjectId = @ProjectId
		AND TimeEntrySectionId = @TimeEntrySectionId
		AND STARTDATE = @recursiverecordstartdate  
	END
	COMMIT TRAN tran_PersonTERecurSel
END TRY
BEGIN CATCH
	RollBACK TRAN tran_PersonTERecurSel
	SELECT @ErrorMessage = ERROR_MESSAGE()
	RAISERROR (@ErrorMessage, 16, 1)

END CATCH
END

