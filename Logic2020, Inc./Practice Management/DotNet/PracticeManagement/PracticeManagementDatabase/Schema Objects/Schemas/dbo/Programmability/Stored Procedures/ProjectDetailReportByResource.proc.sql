-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 04-05-2012
-- Updated by : Sainath.CH
-- Update Date: 05-01-2012
-- =========================================================================
CREATE PROCEDURE [dbo].[ProjectDetailReportByResource]
	(
	  @ProjectNumber NVARCHAR(12) ,
	  @MilestoneId INT = NULL ,
	  @StartDate DATETIME = NULL ,
	  @EndDate DATETIME = NULL ,
	  @PersonRoleNames NVARCHAR(MAX) = NULL
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
			@ORTTimeTypeId INT,
			@UnpaidTimeTypeId	INT,
			@FutureDate DATETIME


		SELECT @ProjectNumberLocal = @ProjectNumber,@FutureDate = dbo.GetFutureDate()
	
		SELECT  @ProjectId = P.ProjectId
		FROM    dbo.Project AS P
		WHERE   P.ProjectNumber = @ProjectNumberLocal
				AND @ProjectNumberLocal != 'P999918' --Business Development Project 

		IF ( @ProjectId IS NOT NULL ) 
			BEGIN

				SET @Today = dbo.GettingPMTime(GETUTCDATE())
				SET @MilestoneIdLocal = @MilestoneId
				SET @HolidayTimeType = dbo.GetHolidayTimeTypeId()
				SET @ORTTimeTypeId = dbo.GetORTTimeTypeId()
				SET @UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()

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

				DECLARE @PersonRoleNamesTable TABLE
					(
					  RoleName NVARCHAR(1024)
					)
				INSERT  INTO @PersonRoleNamesTable
						SELECT  ResultString
						FROM    [dbo].[ConvertXmlStringInToStringTable](@PersonRoleNames);
				WITH    PersonMaxRoleValues
						  AS ( SELECT   MP.PersonId ,
										MAX(ISNULL(PR.RoleValue, 0)) AS MaxRoleValue
							   FROM     dbo.MilestonePersonEntry AS MPE
										INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
										INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
										INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
															  AND C.DayOff = 0
										LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
							   WHERE    M.ProjectId = @ProjectId
										AND ( @MilestoneIdLocal IS NULL
											  OR M.MilestoneId = @MilestoneIdLocal
											)
										AND ( ( @StartDateLocal IS NULL
												AND @EndDateLocal IS NULL
											  )
											  OR ( C.Date BETWEEN @StartDateLocal AND @EndDateLocal )
											)
							   GROUP BY MP.PersonId
							 ),
						 PersonForeCastedHours
						  AS ( SELECT   MP.PersonId ,
										SUM(CASE WHEN CAL.Date < @Today
												 THEN MPE.HoursPerDay
												 ELSE 0
											END) AS ForecastedHoursUntilToday ,
										SUM(MPE.HoursPerDay) AS ForecastedHours
							   FROM     dbo.MilestonePersonEntry AS MPE
										INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
										INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
										INNER JOIN dbo.v_PersonHistory AS P ON P.personId = MP.PersonId
										INNER JOIN dbo.Calendar AS CAL ON CAL.Date BETWEEN MPE.StartDate AND MPE.EndDate 
																		AND CAL.Date >= P.HireDate
																		AND CAL.Date <= ISNULL(P.TerminationDate,
																		@FutureDate)
										LEFT JOIN dbo.PersonCalendar AS PCAL ON PCAL.Date = CAL.Date
																		AND PCAL.PersonId = P.PersonId
							   WHERE    M.ProjectId = @ProjectId
										AND ( @MilestoneIdLocal IS NULL
											  OR M.MilestoneId = @MilestoneIdLocal
											)
										AND ( ( @StartDateLocal IS NULL
												AND @EndDateLocal IS NULL
											  )
											  OR ( CAL.Date BETWEEN @StartDateLocal AND @EndDateLocal )
											)
										AND ( ( CAL.DayOff = 0
												AND ISNULL(PCAL.TimeTypeId, 0) != @HolidayTimeType
											  )
											  OR ( CAL.DayOff = 1
												   AND PCAL.SubstituteDate IS NOT NULL
												 )
											)
							   GROUP BY MP.PersonId
							 )
					SELECT  P.PersonId ,
							P.LastName ,
							P.FirstName ,
							P.IsOffshore ,
							TT.Name AS TimeTypeName ,
							TT.Code AS TimeTypeCode ,
							TE.ChargeCodeDate ,
							( CASE WHEN ( TT.TimeTypeId = @ORTTimeTypeId
										  OR TT.TimeTypeId = @HolidayTimeType
										  OR TT.TimeTypeId = @UnpaidTimeTypeId
										)
								   THEN TE.Note
										+ dbo.GetApprovedByName(TE.ChargeCodeDate,
															  TT.TimeTypeId,
															  P.PersonId)
								   ELSE TE.Note
							  END ) AS Note ,
							ROUND(SUM(CASE WHEN TEH.IsChargeable = 1  AND @ProjectNumberLocal != 'P031000'
										   THEN TEH.ActualHours
										   ELSE 0
									  END), 2) AS BillableHours ,
							ROUND(SUM(CASE WHEN TEH.IsChargeable = 0  OR @ProjectNumberLocal = 'P031000'
										   THEN TEH.ActualHours
										   ELSE 0
									  END), 2) AS NonBillableHours ,
							ISNULL(PR.Name, '') AS ProjectRoleName ,
							CC.TimeEntrySectionId ,
							ROUND(MAX(ISNULL(PFH.ForecastedHours, 0)), 2) AS ForecastedHours
					FROM    dbo.TimeEntry AS TE
							INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
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
							INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId
															  AND CC.ProjectId = @ProjectId
							INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
							INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
															  AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate)
							FULL  JOIN PersonMaxRoleValues AS PMRV ON PMRV.PersonId = TE.PersonId
							INNER JOIN dbo.Person AS P ON ( P.PersonId = TE.PersonId
															OR PMRV.PersonId = P.PersonId
														  )
														  AND p.IsStrawman = 0
							LEFT  JOIN PersonForeCastedHours AS PFH ON PFH.PersonId = P.PersonId
															  AND PFH.PersonId = PMRV.PersonId
							LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
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
							AND ( @PersonRoleNames IS NULL
								  OR ISNULL(PR.Name, '') IN (
								  SELECT    RoleName
								  FROM      @PersonRoleNamesTable )
								)
					GROUP BY P.PersonId ,
							P.LastName ,
							P.FirstName ,
							P.IsOffshore ,
							TT.Name ,
							TT.Code ,
							TE.ChargeCodeDate ,
							TT.TimeTypeId ,
							PR.Name ,
							TE.Note ,
							CC.TimeEntrySectionId
					ORDER BY  P.LastName ,
							  P.FirstName ,
							  TE.ChargeCodeDate,
							   TT.Name

			END
		ELSE 
			BEGIN
				RAISERROR('There is no Project with this Project Number.', 16, 1)
			END
	END

