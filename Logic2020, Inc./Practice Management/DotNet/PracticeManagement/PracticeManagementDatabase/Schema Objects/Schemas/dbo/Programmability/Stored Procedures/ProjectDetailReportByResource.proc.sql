-- =========================================================================
-- Author:		Sainath.CH
-- Create date: 04-05-2012
-- Updated by : Srinivas.M
-- Update Date: 09-25-2012
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
										SUM(CASE WHEN PC.Date < @Today
												 THEN MPE.HoursPerDay
												 ELSE 0
											END) AS ForecastedHoursUntilToday ,
										SUM(MPE.HoursPerDay) AS ForecastedHours
							   FROM     dbo.MilestonePersonEntry AS MPE
										INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
										INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
										INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
									    INNER JOIN dbo.v_PersonCalendar PC ON PC.PersonId = MP.PersonId
															  AND PC.DayOff = 0
															  AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate	
							   WHERE    M.ProjectId = @ProjectId
										AND ( @MilestoneIdLocal IS NULL
											  OR M.MilestoneId = @MilestoneIdLocal
											)
										AND ( ( @StartDateLocal IS NULL
												AND @EndDateLocal IS NULL
											  )
											  OR ( PC.Date BETWEEN @StartDateLocal AND @EndDateLocal )
											)
							   GROUP BY MP.PersonId
							 )
					,TimeEntryPersons AS
					(
						SELECT TE.*, TEH.IsChargeable, TEH.ActualHours, CC.ProjectId, CC.TimeEntrySectionId, PTSH.PersonStatusId, TT.TimeTypeId, TT.Name AS 'TimeTypeName', TT.Code AS 'TimeTypeCode'
						FROM TimeEntry TE
						INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId AND CC.ProjectId = @ProjectId
						INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
																  AND ( ( @MilestoneIdLocal IS NULL )
																	OR ( TE.ChargeCodeDate BETWEEN @MilestoneStartDate AND @MilestoneEndDate )
																  )
																  AND ( ( @StartDateLocal IS NULL AND @EndDateLocal IS NULL )
																	OR ( TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal )
																  )
						INNER JOIN dbo.PersonStatusHistory PTSH ON PTSH.PersonId = TE.PersonId
															  AND TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate)
						INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
					)
					SELECT  P.PersonId ,
							P.LastName ,
							P.FirstName ,
							P.IsOffshore ,
							TEP.TimeTypeName AS TimeTypeName ,
							TEP.TimeTypeCode AS TimeTypeCode ,
							TEP.ChargeCodeDate ,
							( CASE WHEN ( TEP.TimeTypeId = @ORTTimeTypeId
										  OR TEP.TimeTypeId = @HolidayTimeType
										  OR TEP.TimeTypeId = @UnpaidTimeTypeId
										)
								   THEN TEP.Note
										+ dbo.GetApprovedByName(TEP.ChargeCodeDate,
															  TEP.TimeTypeId,
															  P.PersonId)
								   ELSE TEP.Note
							  END ) AS Note ,
							ROUND(SUM(CASE WHEN TEP.IsChargeable = 1  AND @ProjectNumberLocal != 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS BillableHours ,
							ROUND(SUM(CASE WHEN TEP.IsChargeable = 0  OR @ProjectNumberLocal = 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS NonBillableHours ,
							ISNULL(PR.Name, '') AS ProjectRoleName ,
							TEP.TimeEntrySectionId ,
							ROUND(MAX(ISNULL(PFH.ForecastedHours, 0)), 2) AS ForecastedHours,
							P.EmployeeNumber
					FROM    dbo.Person P
					LEFT JOIN TimeEntryPersons TEP ON TEP.PersonId = P.PersonId
					LEFT  JOIN PersonMaxRoleValues AS PMRV ON P.PersonId = PMRV.PersonId
					LEFT  JOIN PersonForeCastedHours AS PFH ON PFH.PersonId = P.PersonId
															  AND PFH.PersonId = PMRV.PersonId
					LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PMRV.MaxRoleValue
					WHERE P.IsStrawman = 0
						AND ( (TEP.TimeEntryId IS NOT NULL 
									AND TEP.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
								    AND ( TEP.timeTypeId != @HolidayTimeType
										 OR ( TEP.timeTypeId = @HolidayTimeType
											  AND TEP.PersonStatusId IN (1,5)
											)
									   )
								 )
							 OR PMRV.PersonId IS NOT NULL
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
							P.EmployeeNumber,
							TEP.TimeTypeName,
							TEP.TimeTypeCode ,
							TEP.ChargeCodeDate ,
							TEP.TimeTypeId ,
							PR.Name ,
							TEP.Note ,
							TEP.TimeEntrySectionId
					ORDER BY  P.LastName ,
							  P.FirstName ,
							  TEP.ChargeCodeDate,
							  TEP.TimeTypeName

			END
		ELSE 
			BEGIN
				RAISERROR('There is no Project with this Project Number.', 16, 1)
			END
	END

