CREATE PROCEDURE [dbo].[SetPersonTimeEntrySelection]
(
	@PersonId			INT,
	@ClientId			INT,
	@ProjectGroupId		INT,
	@ProjectId			INT,
	@TimeEntrySectionId	INT,
	@IsDelete        BIT ,
	@StartDate			DATETIME,
	@EndDate			DATETIME,
	@UserLogin  NVARCHAR(255)
)
AS

BEGIN


 BEGIN TRAN PersonTimeEntrySelection
 
	EXEC dbo.SessionLogPrepare @UserLogin = @UserLogin

/*
if @IsDelete = 0 
--insert section
check weather next record or prev record exists 
if not Insert 
else
update the nextreocord startdate with the @startDate.
find  @NextRecordEndDate 
Adjust the previous record with either @NextRecordEndDate or @EndDate.
if (count of records with EndDate = @NextRecordEndDate) >1
delete record EndDate = @NextRecordEndDate AND StartDate = @StartDate

else
--delete section
case1 @startdate = startdate and @enddate = enddate 
	delete the record
case2 @startdate = startdate 
	update startdate = @enddate+1
case3 @enddate = enddate
	update endate = @startdate-1
case 4 else
find @recordenddate in which @startdate and @enddate exists 
update record enddate to  @startdate-1
insert (startdate,enddate)
(@enddate+1,@recordenddate)


*/
	DECLARE @FutureDate DATETIME
	SET @FutureDate = dbo.GetFutureDate()

	IF (@IsDelete = 0 AND @StartDate < @EndDate)
	BEGIN
		--If any entry exists immediate next week then update that startdate with the @startDate.
		--Adjust the next record.
		UPDATE PTRS
		SET StartDate = @StartDate
		FROM dbo.PersonTimeEntryRecursiveSelection PTRS
		WHERE PersonId = @PersonId 
				AND ClientId = @ClientId 
				AND ProjectGroupId = @ProjectGroupId
				AND ProjectId = @ProjectId
				AND	TimeEntrySectionId = @TimeEntrySectionId
				AND IsRecursive = 0
				AND StartDate = @EndDate + 1

		DECLARE @NextRecordEndDate DATETIME

		SELECT @NextRecordEndDate = ISNULL(EndDate,@FutureDate) --if next record enddate is null then set to futuredate
		FROM dbo.PersonTimeEntryRecursiveSelection PTRS
		WHERE PersonId = @PersonId 
			AND ClientId = @ClientId 
			AND ProjectGroupId = @ProjectGroupId
			AND ProjectId = @ProjectId
			AND	TimeEntrySectionId = @TimeEntrySectionId
			AND IsRecursive = 0
			AND StartDate = @StartDate

		SELECT @NextRecordEndDate
		
		--Adjust the previous record with either @NextRecordEndDate or @EndDate.
		UPDATE PTRS
		SET EndDate = CASE WHEN @NextRecordEndDate = @FutureDate THEN NULL 
						   ELSE ISNULL(@NextRecordEndDate, @EndDate) END
		FROM dbo.PersonTimeEntryRecursiveSelection PTRS
		WHERE PersonId = @PersonId 
			AND ClientId = @ClientId 
			AND ProjectGroupId = @ProjectGroupId
			AND ProjectId = @ProjectId
			AND	TimeEntrySectionId = @TimeEntrySectionId
			AND IsRecursive = 0
			AND EndDate + 1 = @StartDate
		
		IF 1 < (
		SELECT COUNT(*) FROM dbo.PersonTimeEntryRecursiveSelection PTRS
									WHERE PersonId = @PersonId 
									AND ClientId = @ClientId 
									AND ProjectGroupId = @ProjectGroupId
									AND ProjectId = @ProjectId
									AND	TimeEntrySectionId = @TimeEntrySectionId
									AND IsRecursive = 0
									AND StartDate < @EndDate 
									AND ISNULL(EndDate,@FutureDate) > @StartDate
								)
		BEGIN
			DELETE PTRS
			FROM dbo.PersonTimeEntryRecursiveSelection PTRS
			WHERE PersonId = @PersonId 
			AND ClientId = @ClientId 
			AND ProjectGroupId = @ProjectGroupId
			AND ProjectId = @ProjectId
			AND	TimeEntrySectionId = @TimeEntrySectionId
			AND IsRecursive = 0
			AND StartDate = @StartDate
		END

		IF NOT EXISTS (SELECT 1 FROM dbo.PersonTimeEntryRecursiveSelection PTRS
								WHERE PersonId = @PersonId 
								AND ClientId = @ClientId 
								AND ProjectGroupId = @ProjectGroupId
								AND ProjectId = @ProjectId
								AND	TimeEntrySectionId = @TimeEntrySectionId
								AND IsRecursive = 0
								AND (@StartDate BETWEEN StartDate AND ISNULL(EndDate,@FutureDate) + 1 OR @EndDate BETWEEN StartDate - 1 AND ISNULL(EndDate,@FutureDate) )
						)
		BEGIN
			INSERT INTO dbo.PersonTimeEntryRecursiveSelection(StartDate, EndDate, PersonId, ClientId, ProjectGroupId, ProjectId, TimeEntrySectionId)
			VALUES (@StartDate, @EndDate, @PersonId, @ClientId, @ProjectGroupId, @ProjectId, @TimeEntrySectionId)
		END
	END
	ELSE IF (@IsDelete = 1 AND @StartDate < @EndDate)
	BEGIN
		IF EXISTS(SELECT 1 FROM dbo.PersonTimeEntryRecursiveSelection
						WHERE PersonId = @PersonId AND ClientId = @ClientId AND ProjectId = @ProjectId AND ProjectGroupId = @ProjectGroupId 
						AND TimeEntrySectionId = @TimeEntrySectionId AND EndDate = @EndDate AND StartDate = @StartDate 
				 )
		BEGIN
			DELETE PTRS
			FROM dbo.PersonTimeEntryRecursiveSelection PTRS
			WHERE PersonId = @PersonId 
				AND ClientId = @ClientId 
				AND ProjectGroupId = @ProjectGroupId
				AND ProjectId = @ProjectId
				AND	TimeEntrySectionId = @TimeEntrySectionId
				AND EndDate = @EndDate
				AND StartDate = @StartDate
		END
		ELSE
		BEGIN
			-- if endate = @EndDate then update endate 
			UPDATE PTRS
				SET EndDate = @StartDate-1
			FROM dbo.PersonTimeEntryRecursiveSelection PTRS
			WHERE PersonId = @PersonId 
				AND ClientId = @ClientId 
				AND ProjectGroupId = @ProjectGroupId
				AND ProjectId = @ProjectId
				AND	TimeEntrySectionId = @TimeEntrySectionId
				AND EndDate = @EndDate

			-- if StartDate = @StartDate then update StartDate
			UPDATE PTRS
				SET StartDate = @EndDate+1 
			FROM dbo.PersonTimeEntryRecursiveSelection PTRS
			WHERE PersonId = @PersonId 
				AND ClientId = @ClientId 
				AND ProjectGroupId = @ProjectGroupId
				AND ProjectId = @ProjectId
				AND	TimeEntrySectionId = @TimeEntrySectionId
				AND StartDate = @StartDate

			-- else  
			IF EXISTS (SELECT 1 FROM dbo.PersonTimeEntryRecursiveSelection 
							WHERE PersonId = @PersonId 
								AND ClientId = @ClientId 
								AND ProjectGroupId = @ProjectGroupId
								AND ProjectId = @ProjectId
								AND	TimeEntrySectionId = @TimeEntrySectionId
								AND StartDate < @EndDate 
								AND ISNULL(EndDate,@FutureDate) > @StartDate)
			BEGIN
				DECLARE @RecordStartDate DATETIME

				SELECT @RecordStartDate = startdate
				FROM dbo.PersonTimeEntryRecursiveSelection 
				WHERE PersonId = @PersonId 
					AND ClientId = @ClientId 
					AND ProjectGroupId = @ProjectGroupId
					AND ProjectId = @ProjectId
					AND	TimeEntrySectionId = @TimeEntrySectionId
					AND StartDate < @EndDate 
					AND ISNULL(EndDate,@FutureDate) > @StartDate

			
				--update record startdate to  @enddate+1
				UPDATE PTRS
					SET startdate =  @enddate+1
				FROM dbo.PersonTimeEntryRecursiveSelection PTRS
				WHERE PersonId = @PersonId 
					AND ClientId = @ClientId 
					AND ProjectGroupId = @ProjectGroupId
					AND ProjectId = @ProjectId
					AND TimeEntrySectionId = @TimeEntrySectionId
					AND StartDate = @RecordStartDate
	
				INSERT INTO dbo.PersonTimeEntryRecursiveSelection(StartDate, EndDate, PersonId, ClientId, ProjectGroupId, ProjectId, TimeEntrySectionId)
				VALUES (@RecordStartDate,@startdate-1, @PersonId, @ClientId, @ProjectGroupId, @ProjectId, @TimeEntrySectionId)								
			END
							
		END

		--Delete all time entries for that section
		DELETE TEH 
		FROM dbo.TimeEntry TE 
	    INNER JOIN dbo.TimeEntryHours AS TEH  ON TE.TimeEntryId = TEH.TimeEntryId
		INNER JOIN dbo.ChargeCode cc ON TE.ChargeCodeId = cc.Id 
										AND cc.ClientId = @ClientId 
										AND cc.ProjectGroupId = @ProjectGroupId 
										AND cc.ProjectId = @ProjectId 
										AND	cc.TimeEntrySectionId = @TimeEntrySectionId
		WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
				AND TE.PersonId = @personId 

		DELETE TE 
		FROM dbo.TimeEntry TE 
		INNER JOIN dbo.ChargeCode cc ON TE.ChargeCodeId = cc.Id 
										AND cc.ClientId = @ClientId 
										AND cc.ProjectGroupId = @ProjectGroupId 
										AND cc.ProjectId = @ProjectId 
										AND	cc.TimeEntrySectionId = @TimeEntrySectionId
		WHERE TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate 
				AND TE.PersonId = @personId 

	END

	EXEC dbo.SessionLogUnprepare

 COMMIT TRAN PersonTimeEntrySelection

END
