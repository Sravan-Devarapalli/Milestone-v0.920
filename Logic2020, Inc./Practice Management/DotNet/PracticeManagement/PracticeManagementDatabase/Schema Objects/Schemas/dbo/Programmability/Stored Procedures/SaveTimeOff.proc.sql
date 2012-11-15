-- =============================================
-- Author:		Srinivas.M
-- Create date: 02-23-2012
-- Updated by:	Sainath C
-- Update date:	06-01-2012
-- Description: Insert/Update/Delete Time Off(s) for a person.
-- =============================================
CREATE PROCEDURE [dbo].[SaveTimeOff]
    (
      @StartDate DATETIME ,
      @EndDate DATETIME ,
      @DayOff BIT ,
      @PersonId INT ,
      @UserLogin NVARCHAR(255) ,
      @ActualHours REAL ,
      @TimeTypeId INT ,
      @ApprovedBy INT
    )
AS 
    BEGIN
	/*
		If DayOff = 1 
		{
			Insert entry into Personcalendar table
			
		}
		else
		{
			delete entry from PersonCalendar table.
	
			delete timeEntries of @TimeTypeId.
		}
		
		Delete TimeEtnries if there is no entry with DayOff = 1 in PersonCalendar and exists in TimeEntry table.
		Update TimeEntries if there is an entry with DayOff = 1 in PersonCalendar and exists with different ACTUAL HOURS in TimeEntry table.
		Insert TimeEntries if there is an entry with DayOff = 1 in PersonCalendar and not exists in TimeEntry table ONLY for w2salaried/w2hourly persons.
	*/
        DECLARE @Today			DATETIME ,
            @CurrentPMTime		DATETIME ,
            @ModifiedBy			INT ,
            @HolidayTimeTypeId	INT ,
            @Description		NVARCHAR(500) ,
            @ORTTimeTypeId		INT,
			@UnpaidTimeTypeId	INT,
			@W2SalaryId			INT,
			@W2HourlyId			INT
			
        SELECT  @Today = dbo.GettingPMTime(GETUTCDATE()) ,
                @CurrentPMTime = dbo.InsertingTime() ,
                @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId() ,
                @ORTTimeTypeId = dbo.GetORTTimeTypeId(),
				@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()
		SELECT	@W2SalaryId = TimescaleId FROM Timescale WHERE Name = 'W2-Salary'
		SELECT	@W2HourlyId = TimescaleId FROM Timescale WHERE Name = 'W2-Hourly'

        SELECT  @ModifiedBy = PersonId
        FROM    Person
        WHERE   Alias = @UserLogin
        SELECT  @Description = Name + '.'
        FROM    TimeType
        WHERE   TimeTypeId = @TimeTypeId
       
	--SELECT @ApprovedBy = CASE WHEN @ApprovedBy IS NOT NULL THEN @ApprovedBy ELSE @ModifiedBy END

        BEGIN TRY
            BEGIN TRANSACTION tran_SaveTimeOff

            IF @DayOff = 1 
                BEGIN
		/*
			. Adding/Updating the series of Offs then we need to exclude days after this series endDate and before this series startDate from the series.
			. If already other Offs present then we need to update them with new Off.
				
			Note:- If any changes(actualhours, worktype) done in PersonCalendar table then update those in TimeEntry table also.
		*/

		--If selected days has not atleast 1 working day, the we need to throw validation.
                    IF 1 > ( SELECT COUNT(*)
                             FROM   dbo.Calendar C
                                    LEFT JOIN dbo.PersonCalendar PC ON PC.PersonId = @PersonId
                                                              AND C.Date = PC.Date
                                    LEFT JOIN dbo.PersonCalendar Holiday ON PC.Date = Holiday.SubstituteDate
                                                              AND PC.PersonId = Holiday.PersonId
                             WHERE  C.Date BETWEEN @StartDate AND @EndDate
                                    AND C.DayOff = 0
                                    AND ( PC.Date IS NULL
                                          OR ( PC.Date IS NOT NULL
                                               AND PC.DayOff = 1
                                               AND Holiday.SubstituteDate IS NULL
                                             )
                                        )
                           ) 
                        BEGIN
                            RAISERROR('Selected day(s) are not working day(s). Please select any working day(s).', 16, 1)
                        END

		

                    DECLARE @DaysExceptHolidays TABLE ( [Date] DATETIME )

                    INSERT  INTO @DaysExceptHolidays
                            SELECT  C.Date
                            FROM    dbo.Calendar C
                                    LEFT JOIN dbo.PersonCalendar PC ON PC.PersonId = @PersonId
                                                              AND C.Date = PC.Date
                            WHERE   C.Date BETWEEN @StartDate AND @EndDate
                                    AND ( C.DayOff = 0
                                          AND ( PC.Date IS NULL
                                                OR ( PC.DATE IS NOT NULL
                                                     AND PC.DayOff = 1
                                                     AND PC.TimeTypeId <> @HolidayTimeTypeId
                                                   )
                                              )
                                        )

				--delete old Offs.
                    DELETE PC
                    FROM    dbo.PersonCalendar PC
                            JOIN @DaysExceptHolidays DEH ON PC.PersonId = @PersonId
                                                            AND PC.Date = DEH.Date

		--Insert all Offs.
                    INSERT  INTO PersonCalendar
                            ( PersonId ,
                              Date ,
                              DayOff ,
                              TimeTypeId ,
                              ActualHours ,
                              Description ,
                              
                              IsFromTimeEntry ,
                              ApprovedBy
                            )
                            SELECT  @PersonId ,
                                    DEH.Date ,
                                    @DayOff ,
                                    @TimeTypeId ,
                                    @ActualHours ,
                                    @Description ,
                                    0 ,
                                    CASE WHEN @TimeTypeId = @ORTTimeTypeId OR @TimeTypeId = @UnpaidTimeTypeId
                                         THEN @ApprovedBy
                                         ELSE NULL
                                    END
                            FROM    @DaysExceptHolidays DEH
                                    LEFT JOIN PersonCalendar PC ON PC.Date = DEH.Date
                                                              AND PC.PersonId = @PersonId
 


                END
            ELSE 
                BEGIN


                    DELETE  dbo.PersonCalendar
                    WHERE   PersonId = @PersonId
                            AND Date BETWEEN @StartDate AND @EndDate
                END
	
	--Delete TimeOff(other than holiday administrative timetype) TimeEntries for all type of persons(w2salary/w2hourly/etc.) if there is no entry in PersonCalendar
            DELETE  TEH
            FROM    dbo.TimeEntry TE
                    INNER JOIN dbo.ChargeCode CC ON TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
                                                    AND TE.PersonId = @PersonId
                                                    AND TE.ChargeCodeId = CC.Id
                    INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
                                                  AND TT.IsAdministrative = 1
                                                  AND TT.TimeTypeId <> @HolidayTimeTypeId
                    INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
                    INNER JOIN dbo.Calendar C ON C.Date = TE.ChargeCodeDate
                                                 AND C.DayOff = 0
                    LEFT JOIN dbo.PersonCalendar PC ON PC.Date = TE.ChargeCodeDate
                                                       AND PC.TimeTypeId = CC.TimeTypeId
                                                       AND PC.PersonId = TE.PersonId
                                                       AND PC.DayOff = 1
            WHERE   PC.Date IS NULL

            DELETE  TE
            FROM    dbo.TimeEntry TE
                    INNER JOIN dbo.ChargeCode CC ON TE.ChargeCodeDate BETWEEN @StartDate AND @EndDate
                                                    AND TE.PersonId = @PersonId
                                                    AND TE.ChargeCodeId = CC.Id
                    INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
                                                  AND TT.IsAdministrative = 1
                                                  AND TT.TimeTypeId <> @HolidayTimeTypeId
                    INNER JOIN dbo.Calendar C ON C.Date = TE.ChargeCodeDate
                                                 AND C.DayOff = 0
                    LEFT JOIN dbo.PersonCalendar PC ON PC.Date = TE.ChargeCodeDate
                                                       AND PC.TimeTypeId = CC.TimeTypeId
                                                       AND PC.PersonId = TE.PersonId
                                                       AND PC.DayOff = 1
            WHERE   PC.Date IS NULL

	--Update TimeTypeId And Note
            UPDATE  TE
            SET     ChargeCodeId = CC.Id ,
                    Note = PC.Description
            FROM    dbo.PersonCalendar PC
                    INNER JOIN dbo.ChargeCode CC ON PC.Date BETWEEN @StartDate AND @EndDate
                                                    AND PC.PersonId = @PersonId
                                                    AND PC.DayOff = 1
                                                    AND PC.TimeTypeId = CC.TimeTypeId
                    INNER JOIN dbo.TimeEntry TE ON TE.PersonId = PC.PersonId
                                                   AND TE.ChargeCodeDate = PC.Date
                    INNER JOIN dbo.ChargeCode TECC ON TE.ChargeCodeId = TECC.Id
                    INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = TECC.TimeTypeId
                                                  AND TT.IsAdministrative = 1
                                                  AND ( TE.ChargeCodeId <> CC.Id
                                                        OR TE.Note <> PC.Description
                                                      )

	--Update ActualHours
            UPDATE  TEH
            SET     ActualHours = PC.ActualHours
            FROM    dbo.PersonCalendar PC
                    INNER JOIN dbo.ChargeCode CC ON PC.Date BETWEEN @StartDate AND @EndDate
                                                    AND PC.PersonId = @PersonId
                                                    AND PC.DayOff = 1
                                                    AND PC.TimeTypeId = CC.TimeTypeId
                    INNER JOIN dbo.TimeEntry TE ON TE.PersonId = PC.PersonId
                                                   AND TE.ChargeCodeId = CC.Id
                                                   AND TE.ChargeCodeDate = PC.Date
                    INNER JOIN dbo.TimeEntryHours TEH ON TE.TimeEntryId = TEH.TimeEntryId
                                                         AND PC.ActualHours <> TEH.ActualHours

	--Insert TimeEntries only for w2salaried/w2hourly persons, if there is entry in PersonCalendar.
            INSERT  INTO [dbo].[TimeEntry]
                    ( [PersonId] ,
                      [ChargeCodeId] ,
                      [ChargeCodeDate] ,
                      [Note] ,
                      [ForecastedHours] ,
                      [IsCorrect] ,
                      [IsAutoGenerated]
		            )
                    SELECT DISTINCT
                            P.PersonId ,
                            CC.Id ,
                            PC.[Date] ,
                            PC.Description ,
                            0 ,
                            1 ,
                            1 --Here it is Auto generated.
                    FROM    dbo.PersonCalendar PC
                            INNER JOIN dbo.Calendar C ON PC.Date BETWEEN @StartDate AND @EndDate
                                                         AND PC.PersonId = @PersonId
                                                         AND PC.DayOff = 1
                                                         AND C.Date = PC.Date
                                                         AND C.DayOff <> 1
                            INNER JOIN dbo.Person P ON P.PersonId = PC.PersonId
                                                       AND P.IsStrawman = 0
                            INNER JOIN dbo.Pay pay ON p.PersonId = pay.Person
                                                      AND pay.Timescale IN (@W2SalaryId,@W2HourlyId)
                                                      AND PC.Date BETWEEN pay.StartDate
                                                              AND
                                                              ( CASE
                                                              WHEN p.TerminationDate IS NOT NULL
                                                              AND pay.EndDate
                                                              - 1 > p.TerminationDate
                                                              THEN p.TerminationDate
                                                              ELSE pay.EndDate
                                                              - 1
                                                              END )
                            INNER JOIN dbo.ChargeCode CC ON CC.TimeTypeId = PC.TimeTypeId
                            LEFT JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId
                                                          AND TE.ChargeCodeId = CC.Id
                                                          AND TE.ChargeCodeDate = PC.Date
                    WHERE   TE.TimeEntryId IS NULL

            INSERT  INTO [dbo].[TimeEntryHours]
                    ( [TimeEntryId] ,
                      [ActualHours] ,
                      [CreateDate] ,
                      [ModifiedDate] ,
                      [ModifiedBy] ,
                      [IsChargeable] ,
                      [ReviewStatusId]
						
                    )
                    SELECT  TE.TimeEntryId ,
                            CASE PC.TimeTypeId
                              WHEN @HolidayTimeTypeId THEN 8
                              ELSE @ActualHours
                            END ,
                            @CurrentPMTime ,
                            @CurrentPMTime ,
                            @ModifiedBy ,
                            0 ,--Non Billable
                            CASE WHEN PC.IsFromTimeEntry <> 1 THEN 2
                                 ELSE 1
                            END --Inserting timeEntries with Approved Status.
                    FROM    dbo.PersonCalendar PC
                            INNER JOIN dbo.Calendar C ON PC.Date BETWEEN @StartDate AND @EndDate
                                                         AND PC.PersonId = @PersonId
                                                         AND PC.DayOff = 1
                                                         AND C.Date = PC.Date
                                                         AND C.DayOff <> 1
                            INNER JOIN dbo.Person P ON P.PersonId = PC.PersonId
                                                       AND P.IsStrawman = 0
                            INNER JOIN dbo.Pay pay ON p.PersonId = pay.Person
                                                      AND pay.Timescale IN (@W2SalaryId,@W2HourlyId)
                                                      AND PC.Date BETWEEN pay.StartDate
                                                              AND
                                                              ( CASE
                                                              WHEN p.TerminationDate IS NOT NULL
                                                              AND pay.EndDate
                                                              - 1 > p.TerminationDate
                                                              THEN p.TerminationDate
                                                              ELSE pay.EndDate
                                                              - 1
                                                              END )
                            INNER JOIN dbo.ChargeCode CC ON PC.TimeTypeId = CC.TimeTypeId
                            INNER JOIN dbo.TimeEntry TE ON TE.PersonId = PC.PersonId
                                                           AND TE.ChargeCodeId = CC.Id
                                                           AND TE.ChargeCodeDate = PC.Date
                            LEFT JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
                    WHERE   TEH.TimeEntryId IS NULL

            COMMIT TRANSACTION tran_SaveTimeOff
        END TRY
        BEGIN CATCH
            ROLLBACK TRANSACTION tran_SaveTimeOff

            DECLARE @Error NVARCHAR(2000)
            SET @Error = ERROR_MESSAGE()

            RAISERROR(@Error, 16, 1)
        END CATCH
    END

