-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 03-05-2012
-- Updated by : Sainath.CH
-- Update Date: 04-06-2012
-- Description:  Time Entries grouped by workType for a Project.
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectSummaryReportByWorkType]
	(
	  @ProjectNumber NVARCHAR(12) ,
	  @MilestoneId INT = NULL ,
	  @StartDate DATETIME = NULL ,
	  @EndDate DATETIME = NULL ,
	  @CategoryNames NVARCHAR(MAX) = NULL
	)
AS 
	BEGIN

		DECLARE @StartDateLocal DATETIME = NULL ,
			@EndDateLocal DATETIME = NULL ,
			@ProjectNumberLocal NVARCHAR(12) ,
			@MilestoneIdLocal INT = NULL ,
			@HolidayTimeType INT ,
			@ProjectId INT = NULL ,
			@Today DATE ,
			@MilestoneStartDate DATETIME = NULL ,
			@MilestoneEndDate DATETIME = NULL ,
			@ForecastedHours FLOAT ,
			@CategoryNamesLocal NVARCHAR(MAX) = NULL,
			@FutureDate DATETIME
				

		SELECT @ProjectNumberLocal = @ProjectNumber, @CategoryNamesLocal = @CategoryNames,@FutureDate = dbo.GetFutureDate()

		SELECT  @ProjectId = P.ProjectId
		FROM    dbo.Project AS P
		WHERE   P.ProjectNumber = @ProjectNumberLocal
				AND @ProjectNumberLocal != 'P999918' --Business Development Project 

		IF ( @ProjectId IS NOT NULL ) 
			BEGIN
		
				SET @Today = dbo.GettingPMTime(GETUTCDATE())
				SET @MilestoneIdLocal = @MilestoneId
				SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()

				IF ( @StartDate IS NOT NULL
					 AND @EndDate IS NOT NULL
				   ) 
					BEGIN
						SET @StartDateLocal = CONVERT(DATE, @StartDate)
						SET @EndDateLocal = CONVERT(DATE, @EndDate)
					END

				IF ( @MilestoneIdLocal IS NOT NULL ) 
					BEGIN
						SELECT  @MilestoneStartDate = M.StartDate ,
								@MilestoneEndDate = M.ProjectedDeliveryDate
						FROM    dbo.Milestone AS M
						WHERE   M.MilestoneId = @MilestoneIdLocal 
					END

				SELECT  @ForecastedHours = SUM(dbo.PersonProjectedHoursPerDay(ISNULL(pcal.DayOff,cal.DayOff),cal.DayOff,pcal.ActualHours,MPE.HoursPerDay)) 
				FROM    dbo.MilestonePersonEntry AS MPE
						INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
						INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
						INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
						INNER JOIN Calendar AS cal ON cal.Date BETWEEN mpe.StartDate AND mpe.EndDate
						LEFT JOIN PersonCalendar AS pcal ON cal.Date = pcal.Date AND pcal.PersonId = mp.PersonId

				WHERE   M.ProjectId = @ProjectId
						AND ( @MilestoneIdLocal IS NULL
							  OR M.MilestoneId = @MilestoneIdLocal
							)
						AND ( ( @StartDateLocal IS NULL
								AND @EndDateLocal IS NULL
							  )
							  OR ( pcal.Date BETWEEN @StartDateLocal AND @EndDateLocal )
							)

				DECLARE @CategoryNamesTable TABLE ( Name NVARCHAR(1024) )
				INSERT  INTO @CategoryNamesTable
						SELECT  ResultString
						FROM    [dbo].[ConvertXmlStringInToStringTable](@CategoryNamesLocal);
				WITH    AssignedWorkTypes
						  AS ( SELECT DISTINCT
										ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) AS [TimeTypeId]
							   FROM     ( SELECT    PT.ProjectId ,
													PT.TimeTypeId ,
													PT.IsAllowedToShow ,
													TT.Name
										  FROM      dbo.ProjectTimeType PT
													JOIN TimeType TT ON TT.TimeTypeId = PT.TimeTypeId
										  WHERE     PT.ProjectId = @ProjectId
										) PTT
										FULL JOIN ( SELECT  TT.TimeTypeId ,
															TT.Name
													FROM    dbo.TimeType TT
													WHERE   TT.IsDefault = 1
												  ) DTT ON DTT.TimeTypeId = PTT.TimeTypeId
										LEFT JOIN dbo.ChargeCode CC ON CC.ProjectId = @ProjectId
															  AND ISNULL(PTT.TimeTypeId,
															  DTT.TimeTypeId) = CC.TimeTypeId
							   WHERE    ISNULL(IsAllowedToShow, 1) = 1
							 )
					SELECT  TT.TimeTypeId ,
							TT.Name AS [TimeTypeName] ,
							TT.IsDefault ,
							TT.IsInternal ,
							TT.IsAdministrative ,
							ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
												AND @ProjectNumberLocal != 'P031000'
										   THEN TEH.ActualHours
										   ELSE 0
									  END), 2) AS [BillableHours] ,
							ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
												OR @ProjectNumberLocal = 'P031000'
										   THEN TEH.ActualHours
										   ELSE 0
									  END), 2) AS [NonBillableHours] ,
							CASE WHEN TT.IsAdministrative = 1
								 THEN 'Administrative'
								 WHEN TT.IsDefault = 1 THEN 'Default'
								 WHEN TT.IsInternal = 1 THEN 'Internal'
								 ELSE 'Project'
							END AS [Category] ,
							ISNULL(@ForecastedHours, 0.00) AS [ForecastedHours]
					FROM    dbo.TimeEntry TE
							INNER JOIN dbo.TimeEntryHours TEH ON TEH.TimeEntryId = TE.TimeEntryId
															  AND ( ( @MilestoneIdLocal IS NULL )
															  OR ( TE.ChargeCodeDate BETWEEN @MilestoneStartDate
															  AND
															  @MilestoneEndDate )
															  )
															  AND ( ( @StartDateLocal IS NULL
															  AND @EndDateLocal IS NULL
															  )
															  OR ( TE.ChargeCodeDate BETWEEN @StartDateLocal
															  AND
															  @EndDateLocal )
															  )
							INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
															AND CC.ProjectId = @ProjectId
							INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
															  AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate)
							INNER JOIN dbo.Person AS P ON P.PersonId = TE.PersonId
														  AND p.IsStrawman = 0
							FULL JOIN AssignedWorkTypes AWT ON AWT.TimeTypeId = CC.TimeTypeId
							INNER JOIN dbo.TimeType TT ON ( CC.TimeTypeId = TT.TimeTypeId
															OR AWT.TimeTypeId = TT.TimeTypeId
														  )
					WHERE   ( TE.ChargeCodeDate IS NULL
							  OR ( TE.ChargeCodeDate <= ISNULL(P.TerminationDate,
															  @FutureDate)
								   AND ( CC.timeTypeId != @HolidayTimeType
										 OR ( CC.timeTypeId = @HolidayTimeType
											  AND PTSH.PersonStatusId IN (1,5)
											)
									   )
								 )
							)
							AND ( @CategoryNamesLocal IS NULL
								  OR ( ( CASE WHEN TT.IsAdministrative = 1
											  THEN 'Administrative'
											  WHEN TT.IsDefault = 1
											  THEN 'Default'
											  WHEN TT.IsInternal = 1
											  THEN 'Internal'
											  ELSE 'Project'
										 END ) IN (
									   SELECT   Name
									   FROM     @CategoryNamesTable ) )
								)
					GROUP BY TT.Name ,
							TT.TimeTypeId ,
							TT.IsDefault ,
							TT.IsInternal ,
							TT.IsAdministrative
			 
			END
		ELSE 
			BEGIN
				RAISERROR('There is no Project with this Project Number.', 16, 1)
			END
	END

