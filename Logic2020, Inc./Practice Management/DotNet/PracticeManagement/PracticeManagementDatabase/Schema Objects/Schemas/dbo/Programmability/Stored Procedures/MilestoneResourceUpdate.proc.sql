CREATE PROCEDURE dbo.MilestoneResourceUpdate
(
	@MilestoneId              INT,
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@IsStartDateChangeReflectedForMilestoneAndPersons BIT,
	@IsEndDateChangeReflectedForMilestoneAndPersons   BIT
)
AS
BEGIN
	SET NOCOUNT ON;
		
		DECLARE @FutureDate DATETIME
		SET @FutureDate = dbo.GetFutureDate()

		--Get the active start date and active end date of the mile stone person in the selected date range.
		DECLARE @PersonActiveDates TABLE (PersonId INT ,ActiveStartDate DATETIME,ActiveEndDate DATETIME)
		INSERT INTO @PersonActiveDates 
		SELECT MP.PersonId,MIN(C.Date) AS ActiveStartDate,MAX(C.Date) AS ActiveEndDate
		FROM dbo.MilestonePerson MP 
		INNER JOIN dbo.Calendar C ON C.Date BETWEEN @StartDate AND @ProjectedDeliveryDate AND MP.MilestoneId = @MilestoneId
		INNER JOIN dbo.v_PersonHistory PH ON MP.PersonId = PH.PersonId AND C.Date BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate,@FutureDate) 
		GROUP BY MP.PersonId

		--Delete milestone person entries which don't have atleast 1 active day in the selected range.
		DECLARE @DeleteMileStonePersonEntriesTable TABLE(MilestonePersonId INT)
		
		INSERT INTO @DeleteMileStonePersonEntriesTable
		SELECT   mp.MilestonePersonId
		FROM [dbo].[MilestonePerson] AS mp
		INNER JOIN dbo.Person P ON mp.PersonId = P.PersonId AND P.IsStrawman = 0
		LEFT JOIN @PersonActiveDates AS PA ON mp.PersonId = PA.PersonId
		WHERE  PA.PersonId IS NULL AND mp.MilestoneId = @MilestoneId 
		 
		DELETE MPE FROM dbo.MilestonePersonEntry MPE
		JOIN @DeleteMileStonePersonEntriesTable Temp ON Temp.MilestonePersonId = MPE.MilestonePersonId
		 
		DELETE MP FROM dbo.MilestonePerson MP
		JOIN @DeleteMileStonePersonEntriesTable Temp ON Temp.MilestonePersonId = MP.MilestonePersonId

		--When the milestone person entries are out of the range of the selected milestone range,Keep atleast one record with granularity(milestone person,role)
		DECLARE @TempMPE TABLE
		(MilestonePersonId INT,PersonRoleId INT,Amount DECIMAL(18,2),HoursPerDay DECIMAL(4,2))
		
		INSERT INTO @TempMPE(MilestonePersonId,PersonRoleId ,Amount ,HoursPerDay )
		SELECT MPE.MilestonePersonId,MPE.PersonRoleId,MAX(MPE.Amount),MAX(MPE.HoursPerDay)
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		INNER JOIN dbo.Person AS p ON p.PersonId = MP.PersonId AND p.IsStrawman = 0
		WHERE MP.MilestoneId = @MilestoneId 
			AND ( MPE.StartDate > @ProjectedDeliveryDate OR MPE.EndDate < @StartDate )
		GROUP BY MPE.MilestonePersonId,MPE.PersonRoleId
		
		
		DELETE  MPE 
		FROM dbo.MilestonePersonEntry MPE
		JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		INNER JOIN dbo.Person AS p ON p.PersonId = MP.PersonId AND p.IsStrawman = 0
		WHERE MP.MilestoneId = @MilestoneId 
			AND ( MPE.StartDate > @ProjectedDeliveryDate OR MPE.EndDate < @StartDate )
			
		INSERT INTO dbo.MilestonePersonEntry(MilestonePersonId,StartDate,EndDate,PersonRoleId,Amount,Location,HoursPerDay )
		SELECT TEMPmpe.MilestonePersonId,@StartDate,@StartDate,TEMPmpe.PersonRoleId,TEMPmpe.Amount, NULL, TEMPmpe.HoursPerDay  
		FROM @TempMPE AS TEMPmpe
		WHERE NOT EXISTS (SELECT 1 FROM dbo.MilestonePersonEntry MPE
						 WHERE  TEMPmpe.MilestonePersonId = MPE.MilestonePersonId 
								AND TEMPmpe.PersonRoleId = MPE.PersonRoleId)
		
		DELETE MP 
		FROM dbo.MilestonePerson MP
		LEFT JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		WHERE MPE.MilestonePersonId IS NULL AND MP.MilestoneId = @MilestoneId


		-- Update For Straw Man
		   UPDATE mpentry
			   SET StartDate = CASE
									 WHEN ( P.HireDate > @StartDate) THEN  P.HireDate
									 ELSE @StartDate
								   END
								
			  FROM MilestonePersonEntry as mpentry
			  INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpentry.MilestonePersonId
			  INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId AND p.IsStrawman = 1
			  WHERE  mp.MilestoneId = @MilestoneId  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE mpentry
				SET EndDate =  CASE
									WHEN ( @ProjectedDeliveryDate > p.TerminationDate ) THEN  p.TerminationDate
									ELSE (@ProjectedDeliveryDate)
								  END
			    FROM MilestonePersonEntry as mpentry
				INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpentry.MilestonePersonId
			    INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId AND p.IsStrawman = 1
			    WHERE  mp.MilestoneId = @MilestoneId   AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 


		-- Update For Persons

	    UPDATE mpentry
			   SET StartDate = P.ActiveStartDate
			  FROM MilestonePersonEntry as mpentry
			  INNER JOIN 
						(
						 SELECT RANK() OVER(PARTITION BY  mpe.MilestonePersonId,ISNULL(mpe.PersonRoleId,0) ORDER BY StartDate) AS RANKnO,
								mpe.Id,
								mp.MilestoneId,
								mp.PersonId
						 FROM dbo.MilestonePersonEntry AS mpe
						 INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
						 ) AS q ON mpentry.Id = q.Id
			  INNER JOIN @PersonActiveDates AS p ON p.PersonId = q.PersonId
			  WHERE 1 = q.RANKnO AND q.MilestoneId = @MilestoneId  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE mpentry
				SET EndDate =  P.ActiveEndDate
			    FROM MilestonePersonEntry as mpentry
				INNER JOIN 
					(
					 SELECT RANK() OVER(PARTITION BY  mpe.MilestonePersonId,ISNULL(mpe.PersonRoleId,0) ORDER BY EndDate DESC) AS RANKnO,
							mpe.Id,
							mp.MilestoneId,
							mp.PersonId
					 FROM dbo.MilestonePersonEntry AS mpe
					 INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
					 ) AS q ON mpentry.Id = q.Id
				INNER JOIN @PersonActiveDates AS p ON p.PersonId = q.PersonId
				WHERE  1 = q.RANKnO AND q.MilestoneId = @MilestoneId AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 

		--Ensure that now wrong data is present
	    UPDATE mpe
			   SET EndDate =  CASE WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.StartDate
									 ELSE mpe.EndDate
								   END
								) ELSE mpe.EndDate END,
				  StartDate = CASE WHEN @IsEndDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.EndDate
									 ELSE mpe.StartDate
								   END
								) ELSE mpe.StartDate END
			  FROM dbo.MilestonePersonEntry AS mpe
				   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
				   INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId
			 WHERE mp.MilestoneId = @MilestoneId AND (@IsStartDateChangeReflectedForMilestoneAndPersons = 1 OR @IsEndDateChangeReflectedForMilestoneAndPersons = 1 )

 END

