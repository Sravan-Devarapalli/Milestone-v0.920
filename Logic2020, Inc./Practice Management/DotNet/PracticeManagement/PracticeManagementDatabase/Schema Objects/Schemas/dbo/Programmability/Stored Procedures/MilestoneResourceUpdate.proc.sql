CREATE PROCEDURE dbo.MilestoneResourceUpdate
(
	@MilestoneId              INT,
	@StartDate                DATETIME,
	@ProjectedDeliveryDate    DATETIME,
	@IsStartDateChangeReflectedForMilestoneAndPersons BIT,
	@IsEndDateChangeReflectedForMilestoneAndPersons   BIT,
	@IsExtendedORCompleteOutOfRange BIT,
	@UserLogin				 NVARCHAR(255)
)
AS
BEGIN
	   SET NOCOUNT ON;
		SET ANSI_WARNINGS OFF
	BEGIN TRY
	BEGIN TRAN  Tran_MilestoneResourceUpdate
		DECLARE @FutureDate			DATETIME,
		        @MilestoneIdLocal	INT,
				@StartDateLocal		DATETIME,
				@ProjectedDeliveryDateLocal DATETIME

		SET @FutureDate = dbo.GetFutureDate()
		SELECT	@MilestoneIdLocal =@MilestoneId,
				@StartDateLocal = @StartDate,
				@ProjectedDeliveryDateLocal = @ProjectedDeliveryDate

		DECLARE @Today		DATETIME,
				@ProjectId	INT

		SELECT @Today = CONVERT(DATETIME,CONVERT(DATE,[dbo].[GettingPMTime](GETUTCDATE())))
		SELECT @ProjectId=ProjectId 
		FROM dbo.Milestone 
		WHERE MilestoneId = @MilestoneId

		 
		DECLARE @DefaultStartDate DATETIME = '20140701'
		DECLARE @MinDate DATETIME = '19000101'

		DECLARE @UpdatedBy INT
		SELECT @UpdatedBy = PersonId FROM dbo.Person WHERE Alias = @UserLogin

		--Get the active start date and active end date of the mile stone person in the selected date range.
		DECLARE @PersonActiveDates TABLE (PersonId INT ,ActiveStartDate DATETIME,ActiveEndDate DATETIME, CurrentBadgeStartDate DATETIME, CurrentBadgeEndDate DATETIME, NewBadgeStartDate DATETIME, NewBadgeEndDate DATETIME, IsExtended BIT)
		INSERT INTO @PersonActiveDates(PersonId,ActiveStartDate,ActiveEndDate)
		SELECT MP.PersonId,MIN(C.Date) AS ActiveStartDate,MAX(C.Date) AS ActiveEndDate
		FROM dbo.MilestonePerson MP 
		INNER JOIN dbo.Calendar C ON C.Date BETWEEN @StartDateLocal AND @ProjectedDeliveryDateLocal AND MP.MilestoneId = @MilestoneIdLocal
		INNER JOIN dbo.v_PersonHistory PH ON MP.PersonId = PH.PersonId AND C.Date BETWEEN PH.HireDate AND ISNULL(PH.TerminationDate,@FutureDate) 
		GROUP BY MP.PersonId

		UPDATE P
		SET P.CurrentBadgeStartDate = M.BadgeStartDate,
			P.CurrentBadgeEndDate = M.BadgeEndDate
		FROM @PersonActiveDates P
		INNER JOIN v_CurrentMSBadge M ON M.PersonId = P.PersonId

		--Delete milestone person entries which don't have atleast 1 active day in the selected range.
		DECLARE @DeleteMileStonePersonEntriesTable TABLE(MilestonePersonId INT)
		
		INSERT INTO @DeleteMileStonePersonEntriesTable
		SELECT   mp.MilestonePersonId
		FROM [dbo].[MilestonePerson] AS mp
		INNER JOIN dbo.Person P ON mp.PersonId = P.PersonId AND P.IsStrawman = 0
		LEFT JOIN @PersonActiveDates AS PA ON mp.PersonId = PA.PersonId
		WHERE  PA.PersonId IS NULL AND mp.MilestoneId = @MilestoneIdLocal 
		 
		DELETE MPE FROM dbo.MilestonePersonEntry MPE
		JOIN @DeleteMileStonePersonEntriesTable Temp ON Temp.MilestonePersonId = MPE.MilestonePersonId

		DELETE MP FROM dbo.MilestonePerson MP
		JOIN @DeleteMileStonePersonEntriesTable Temp ON Temp.MilestonePersonId = MP.MilestonePersonId

		--When the milestone person entries are out of the range of the selected milestone range,Keep atleast one record with granularity(milestone person,role)
		DECLARE @TempMPE TABLE
		(MilestonePersonId INT,PersonRoleId INT,Amount DECIMAL(18,2),HoursPerDay DECIMAL(4,2), IsBadgeRequired BIT, IsBadgeException BIT, IsApproved BIT)
		
		INSERT INTO @TempMPE(MilestonePersonId,PersonRoleId ,Amount ,HoursPerDay ,IsBadgeRequired, IsBadgeException, IsApproved)
		SELECT MPE.MilestonePersonId,MPE.PersonRoleId,MAX(MPE.Amount),MAX(MPE.HoursPerDay),MAX(CAST(MPE.IsBadgeRequired AS INT)),MAX(CAST(MPE.IsBadgeException AS INT)),0
		FROM dbo.MilestonePersonEntry MPE
		INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		INNER JOIN dbo.Person AS p ON p.PersonId = MP.PersonId AND p.IsStrawman = 0
		WHERE MP.MilestoneId = @MilestoneIdLocal 
			AND (MPE.StartDate > @ProjectedDeliveryDateLocal OR MPE.EndDate < @StartDateLocal)
		GROUP BY MPE.MilestonePersonId,MPE.PersonRoleId
		
		DELETE  MPE 
		FROM dbo.MilestonePersonEntry MPE
		JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId 
		INNER JOIN dbo.Person AS p ON p.PersonId = MP.PersonId AND p.IsStrawman = 0
		WHERE MP.MilestoneId = @MilestoneIdLocal 
			AND ( MPE.StartDate > @ProjectedDeliveryDateLocal OR MPE.EndDate < @StartDateLocal )
			
		INSERT INTO dbo.MilestonePersonEntry(MilestonePersonId,StartDate,EndDate,PersonRoleId,Amount,Location,HoursPerDay,IsBadgeRequired,BadgeStartDate,BadgeEndDate,IsBadgeException,IsApproved,Requester)
		SELECT TEMPmpe.MilestonePersonId,@StartDateLocal,@StartDateLocal,TEMPmpe.PersonRoleId,TEMPmpe.Amount, NULL, TEMPmpe.HoursPerDay,TEMPmpe.IsBadgeRequired,
			   CASE WHEN TEMPmpe.IsBadgeRequired = 1 THEN @StartDateLocal ELSE NULL END,
			    CASE WHEN TEMPmpe.IsBadgeRequired = 1 THEN @StartDateLocal ELSE NULL END,
				TEMPmpe.IsBadgeException,TEMPmpe.IsApproved,@UpdatedBy
		FROM @TempMPE AS TEMPmpe
		WHERE NOT EXISTS (SELECT 1 FROM dbo.MilestonePersonEntry MPE
						 WHERE  TEMPmpe.MilestonePersonId = MPE.MilestonePersonId 
								AND TEMPmpe.PersonRoleId = MPE.PersonRoleId)
		
		DELETE MP 
		FROM dbo.MilestonePerson MP
		LEFT JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		WHERE MPE.MilestonePersonId IS NULL AND MP.MilestoneId = @MilestoneIdLocal


		-- Update For Straw Man
		   UPDATE mpentry
			   SET StartDate = CASE
									 WHEN ( P.HireDate > @StartDateLocal) THEN  P.HireDate
									 ELSE @StartDateLocal
								   END
								
			  FROM MilestonePersonEntry as mpentry
			  INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpentry.MilestonePersonId
			  INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId AND p.IsStrawman = 1
			  WHERE  mp.MilestoneId = @MilestoneIdLocal  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE mpentry
				SET EndDate =  CASE
									WHEN ( @ProjectedDeliveryDateLocal > p.TerminationDate ) THEN  p.TerminationDate
									ELSE (@ProjectedDeliveryDateLocal)
								  END
			    FROM MilestonePersonEntry as mpentry
				INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpentry.MilestonePersonId
			    INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId AND p.IsStrawman = 1
			    WHERE  mp.MilestoneId = @MilestoneIdLocal   AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 


		--
		UPDATE p
		SET p.NewBadgeStartDate = CASE WHEN mpentry.BadgeStartDate IS NULL THEN NULL
														 WHEN P.ActiveEndDate >= @DefaultStartDate THEN ( CASE	WHEN P.CurrentBadgeEndDate IS NULL THEN dbo.GreaterDateBetweenTwo(P.ActiveStartDate,@DefaultStartDate)
																												WHEN P.CurrentBadgeEndDate >= P.ActiveStartDate THEN dbo.SmallerDateBetweenTwo(P.CurrentBadgeEndDate,(dbo.GreaterDateBetweenTwo(P.ActiveStartDate,@DefaultStartDate))) 
																												ELSE NULL END)
														 ELSE NULL END,
		    p.IsExtended = CASE WHEN ISNULL(mpentry.BadgeStartDate,@FutureDate) > ISNULL((CASE WHEN mpentry.BadgeStartDate IS NULL THEN NULL
														 WHEN P.ActiveEndDate >= @DefaultStartDate THEN ( CASE	WHEN P.CurrentBadgeEndDate IS NULL THEN dbo.GreaterDateBetweenTwo(P.ActiveStartDate,@DefaultStartDate)
																												WHEN P.CurrentBadgeEndDate >= P.ActiveStartDate THEN dbo.SmallerDateBetweenTwo(P.CurrentBadgeEndDate,(dbo.GreaterDateBetweenTwo(P.ActiveStartDate,@DefaultStartDate))) 
																												ELSE NULL END)
														 ELSE NULL END),@FutureDate) THEN 1 ELSE 0 END
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
		WHERE 1 = q.RANKnO AND q.MilestoneId = @MilestoneIdLocal  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE p
		SET p.NewBadgeEndDate = CASE WHEN BadgeEndDate IS NULL THEN NULL 
									WHEN P.ActiveEndDate >= @DefaultStartDate THEN (CASE WHEN P.CurrentBadgeEndDate IS NULL THEN P.ActiveEndDate
																						 WHEN P.CurrentBadgeEndDate >= P.ActiveStartDate THEN dbo.SmallerDateBetweenTwo(P.CurrentBadgeEndDate,P.ActiveEndDate) 
																						 ELSE NULL END)
									ELSE NULL END,
		   p.IsExtended = CASE WHEN p.IsExtended = 1 THEN 1
							   WHEN ISNULL(mpentry.BadgeEndDate,@MinDate) < ISNULL((CASE WHEN BadgeEndDate IS NULL THEN NULL 
																 WHEN P.ActiveEndDate >= @DefaultStartDate THEN (CASE WHEN P.CurrentBadgeEndDate IS NULL THEN P.ActiveEndDate
																													  WHEN P.CurrentBadgeEndDate >= P.ActiveStartDate THEN dbo.SmallerDateBetweenTwo(P.CurrentBadgeEndDate,P.ActiveEndDate) 
																													  ELSE NULL END)
																	ELSE NULL END),@MinDate) THEN 1 ELSE 0 END
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
		WHERE  1 = q.RANKnO AND q.MilestoneId = @MilestoneIdLocal AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 

		-- Update For Persons
		
	    UPDATE mpentry
			   SET StartDate = P.ActiveStartDate,
			   BadgeStartDate = P.NewBadgeStartDate,
			   IsApproved = CASE WHEN P.IsExtended = 1 THEN 0 ELSE IsApproved END,
			   Requester = @UpdatedBy,
			   IsBadgeRequired = CASE WHEN P.NewBadgeStartDate IS NULL THEN 0
									  ELSE 1 END
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
		WHERE 1 = q.RANKnO AND q.MilestoneId = @MilestoneIdLocal  AND @IsStartDateChangeReflectedForMilestoneAndPersons = 1 

		UPDATE mpentry
				SET EndDate =  P.ActiveEndDate,
				BadgeEndDate = P.NewBadgeEndDate,
				IsApproved = CASE WHEN P.IsExtended = 1 THEN 0 ELSE IsApproved END,
				Requester = @UpdatedBy,
				IsBadgeRequired = CASE WHEN P.NewBadgeEndDate IS NULL THEN 0
									   ELSE 1 END
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
		WHERE  1 = q.RANKnO AND q.MilestoneId = @MilestoneIdLocal AND  @IsEndDateChangeReflectedForMilestoneAndPersons = 1 

		--Ensure that now wrong data is present
	    UPDATE mpe
			   SET EndDate =  CASE WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.StartDate
									 ELSE mpe.EndDate
								   END
								) ELSE mpe.EndDate END,
				   BadgeEndDate = CASE	WHEN mpe.BadgeEndDate IS NULL THEN NULL
										WHEN @IsStartDateChangeReflectedForMilestoneAndPersons =1 THEN 
										( CASE
											 WHEN ( mpe.BadgeStartDate > mpe.BadgeEndDate) THEN  mpe.BadgeStartDate
											 ELSE mpe.BadgeEndDate
										   END
										) 
										ELSE mpe.BadgeEndDate END,
				  StartDate = CASE WHEN @IsEndDateChangeReflectedForMilestoneAndPersons =1 THEN 
								( CASE
									 WHEN ( mpe.StartDate > mpe.EndDate) THEN  mpe.EndDate
									 ELSE mpe.StartDate
								   END
								) ELSE mpe.StartDate END,
				  BadgeStartDate = CASE WHEN mpe.BadgeStartDate IS NULL THEN NULL
										WHEN @IsEndDateChangeReflectedForMilestoneAndPersons =1 THEN 
											( CASE
												 WHEN ( mpe.BadgeStartDate > mpe.BadgeEndDate) THEN  mpe.BadgeEndDate
												 ELSE mpe.BadgeStartDate
											   END
											) 
										ELSE mpe.BadgeStartDate END
			  FROM dbo.MilestonePersonEntry AS mpe
				   INNER JOIN dbo.MilestonePerson AS mp ON mp.MilestonePersonId = mpe.MilestonePersonId
				   INNER JOIN dbo.Person AS p ON p.PersonId = mp.PersonId
			 WHERE mp.MilestoneId = @MilestoneIdLocal AND (@IsStartDateChangeReflectedForMilestoneAndPersons = 1 OR @IsEndDateChangeReflectedForMilestoneAndPersons = 1 )

	IF @MilestoneIdLocal IS NOT NULL
	BEGIN
		EXEC [dbo].[InsertProjectFeedbackByMilestonePersonId] @MilestonePersonId=NULL,@MilestoneId = @MilestoneIdLocal
	END
	
	UPDATE dbo.Project
	SET CreatedDate = @Today
	WHERE ProjectId = @ProjectId

	COMMIT TRAN Tran_MilestoneResourceUpdate
	END TRY
	BEGIN CATCH
		ROLLBACK TRAN Tran_MilestoneResourceUpdate
		DECLARE @ErrorMessage NVARCHAR(MAX)
		SET @ErrorMessage = ERROR_MESSAGE()
		RAISERROR(@ErrorMessage, 16, 1)
	END CATCH
 END

