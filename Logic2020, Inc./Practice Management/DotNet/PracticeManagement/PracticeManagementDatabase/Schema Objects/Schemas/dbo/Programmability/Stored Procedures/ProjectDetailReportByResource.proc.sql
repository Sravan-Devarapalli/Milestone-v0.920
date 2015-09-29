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
	  @PersonRoleNames NVARCHAR(MAX) = NULL,
	  @IsExport BIT = 0 
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
		DECLARE @PersonRoleNamesTable TABLE
			(
				RoleName NVARCHAR(1024)
			)
		IF(@IsExport = 0)
		BEGIN
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

				
				INSERT  INTO @PersonRoleNamesTable
						SELECT  ResultString
						FROM    [dbo].[ConvertXmlStringInToStringTable](@PersonRoleNames);

						WITH PersonForeCastedHoursRoleValues
					AS
					(	
						SELECT   MP.PersonId ,
								AVG(MPE.Amount) AS BillRate,
								PC.Date,								
								SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ForecastedHours,
								MAX(ISNULL(PR.RoleValue, 0)) AS MaxRoleValue ,
								MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
								MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue 
						FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
								INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
														AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate	
								LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
						WHERE    M.ProjectId = @ProjectId
								AND ( @MilestoneIdLocal IS NULL
										OR M.MilestoneId = @MilestoneIdLocal
									)
								AND ( ( @StartDateLocal IS NULL
										AND @EndDateLocal IS NULL
										)
										OR ( PC.Date BETWEEN @StartDateLocal AND @EndDateLocal )
									)
						GROUP BY MP.PersonId,PC.Date 
					)
					,PersonForeCastedHours
					AS
					(
					SELECT p.personid,SUM(ForecastedHours) AS ForecastedHours
					FROM PersonForeCastedHoursRoleValues p
					GROUP BY p.personid
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

						SELECT	P.PersonId ,
								P.LastName ,
								ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
								P.IsOffshore ,
								TEP.TimeTypeName AS TimeTypeName ,
								TEP.TimeTypeCode AS TimeTypeCode ,
								TEP.ChargeCodeDate ,
								( CASE WHEN  TEP.TimeEntrySectionId = 4
									   THEN TEP.Note
											+ dbo.GetApprovedByName(TEP.ChargeCodeDate,
																  TEP.TimeTypeId,
																  P.PersonId)
									   ELSE TEP.Note
								  END ) AS Note,
								(
								  CASE 
										WHEN ( (PFR.MinimumValue = PFR.MaximumValue AND PFR.MinimumValue = 1) 
												OR PFR.MinimumValue <> PFR.MaximumValue) 
										THEN ISNULL(PFR.BillRate,0) 
										ELSE 0 
								END)  BillRate,
								TEP.TimeEntrySectionId ,
								P.EmployeeNumber,

								ROUND(SUM(CASE WHEN TEP.IsChargeable = 1  AND @ProjectNumberLocal != 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS BillableHours ,
							ROUND(SUM(CASE WHEN TEP.IsChargeable = 0  OR @ProjectNumberLocal = 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS NonBillableHours ,
							ISNULL(PR.Name, '') AS ProjectRoleName ,
							ROUND(MAX(ISNULL(PFH.ForecastedHours, 0)), 2) AS ForecastedHours
						FROM PersonForeCastedHoursRoleValues	PFR
						FULL JOIN TimeEntryPersons TEP ON TEP.PersonId = PFR.PersonId AND TEP.ChargeCodeDate = PFR.Date
						INNER JOIN dbo.person P ON P.personid = ISNULL(PFR.PersonId,TEP.PersonId)
						LEFT JOIN PersonForeCastedHours PFH ON p.personid = PFH.personid
						LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PFR.MaxRoleValue
						WHERE ( (TEP.TimeEntryId IS NOT NULL 
									AND TEP.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
								    AND ( TEP.timeTypeId != @HolidayTimeType
										 OR ( TEP.timeTypeId = @HolidayTimeType
											  AND TEP.PersonStatusId IN (1,5)
											)
									   )
								 )
							 OR PFR.PersonId IS NOT NULL
							)
							AND ( @PersonRoleNames IS NULL
								  OR ISNULL(PR.Name, '') IN (
								  SELECT    RoleName
								  FROM      @PersonRoleNamesTable )
								)
						GROUP BY	P.personid,
									P.LastName ,
									ISNULL(P.PreferredFirstName,P.FirstName) ,
									P.IsOffshore ,
									P.EmployeeNumber,
									TEP.TimeTypeName,
									TEP.TimeTypeCode ,
									TEP.ChargeCodeDate ,
									TEP.TimeTypeId ,
									TEP.TimeEntrySectionId,
									TEP.Note,
									PFR.MinimumValue,
									PFR.MaximumValue,
									PFR.BillRate,
									PR.Name
						ORDER BY  P.LastName ,
								  ISNULL(P.PreferredFirstName,P.FirstName) ,
								  TEP.ChargeCodeDate,
								  TEP.TimeTypeName

			END
			ELSE 
				BEGIN
					RAISERROR('There is no Project with this Project Number.', 16, 1)
				END
		END
		ELSE
		BEGIN
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

				INSERT  INTO @PersonRoleNamesTable
						SELECT  ResultString
						FROM    [dbo].[ConvertXmlStringInToStringTable](@PersonRoleNames);

						WITH PersonForeCastedHoursRoleValues
					AS
					(	
						SELECT   MP.PersonId ,
								AVG(MPE.Amount) AS BillRate,
								PC.Date,								
								SUM(dbo.PersonProjectedHoursPerDay(PC.DayOff,PC.CompanyDayOff,PC.TimeOffHours,MPE.HoursPerDay)) AS ForecastedHours,
								MAX(ISNULL(PR.RoleValue, 0)) AS MaxRoleValue ,
								MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
								MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue 
						FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.person AS P ON P.PersonId = MP.PersonId AND P.IsStrawman = 0
								INNER JOIN dbo.PersonCalendarAuto PC ON PC.PersonId = MP.PersonId
														AND PC.Date BETWEEN MPE.StartDate AND MPE.EndDate	
								LEFT  JOIN dbo.PersonRole AS PR ON PR.PersonRoleId = MPE.PersonRoleId
						WHERE    M.ProjectId = @ProjectId
								AND ( @MilestoneIdLocal IS NULL
										OR M.MilestoneId = @MilestoneIdLocal
									)
								AND ( ( @StartDateLocal IS NULL
										AND @EndDateLocal IS NULL
										)
										OR ( PC.Date BETWEEN @StartDateLocal AND @EndDateLocal )
									)
								AND PC.DayOff = 0
								AND M.ProjectId <> 174
						GROUP BY MP.PersonId,PC.Date 
					)
					,PersonForeCastedHours
					AS
					(
					SELECT p.personid,SUM(ForecastedHours) AS ForecastedHours
					FROM PersonForeCastedHoursRoleValues p
					GROUP BY p.personid
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

						SELECT	P.PersonId ,
								P.LastName ,
								ISNULL(P.PreferredFirstName,P.FirstName) AS FirstName,
								P.IsOffshore ,
								TEP.TimeTypeName AS TimeTypeName ,
								TEP.TimeTypeCode AS TimeTypeCode ,
								isnull(TEP.ChargeCodeDate,PFR.Date) as ChargeCodeDate,
								( CASE WHEN  TEP.TimeEntrySectionId = 4
									   THEN TEP.Note
											+ dbo.GetApprovedByName(isnull(TEP.ChargeCodeDate,PFR.Date),
																  TEP.TimeTypeId,
																  P.PersonId)
									   ELSE TEP.Note
								  END ) AS Note,
								(
								  CASE 
										WHEN ( (PFR.MinimumValue = PFR.MaximumValue AND PFR.MinimumValue = 1) 
												OR PFR.MinimumValue <> PFR.MaximumValue) 
										THEN ISNULL(PFR.BillRate,0) 
										ELSE 0 
								END)  BillRate,
								TEP.TimeEntrySectionId ,
								P.EmployeeNumber,

								ROUND(SUM(CASE WHEN TEP.IsChargeable = 1  AND @ProjectNumberLocal != 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS BillableHours ,
							ROUND(SUM(CASE WHEN TEP.IsChargeable = 0  OR @ProjectNumberLocal = 'P031000'
										   THEN TEP.ActualHours
										   ELSE 0
									  END), 2) AS NonBillableHours ,
							ISNULL(PR.Name, '') AS ProjectRoleName ,
							ROUND(MAX(ISNULL(PFH.ForecastedHours, 0)), 2) AS ForecastedHours,
							ROUND(MAX(ISNULL(PFR.ForecastedHours, 0)), 2) AS ForecastedHoursDaily,
							CASE	WHEN ( PFR.MinimumValue IS NULL ) THEN ''
										WHEN ( PFR.MinimumValue = PFR.MaximumValue AND PFR.MinimumValue = 0 ) THEN 'Fixed'
	 									WHEN ( PFR.MinimumValue = PFR.MaximumValue AND PFR.MinimumValue = 1) THEN 'Hourly'
								ELSE 'Both'
								END  AS BillingType
						FROM PersonForeCastedHoursRoleValues	PFR
						FULL JOIN TimeEntryPersons TEP ON TEP.PersonId = PFR.PersonId AND TEP.ChargeCodeDate = PFR.Date
						INNER JOIN dbo.person P ON P.personid = ISNULL(PFR.PersonId,TEP.PersonId)
						LEFT JOIN PersonForeCastedHours PFH ON p.personid = PFH.personid
						LEFT  JOIN dbo.PersonRole AS PR ON PR.RoleValue = PFR.MaxRoleValue
						WHERE ( (TEP.TimeEntryId IS NOT NULL 
									AND TEP.ChargeCodeDate <= ISNULL(P.TerminationDate,@FutureDate)
								    AND ( TEP.timeTypeId != @HolidayTimeType
										 OR ( TEP.timeTypeId = @HolidayTimeType
											  AND TEP.PersonStatusId IN (1,5)
											)
									   )
								 )
							 OR PFR.PersonId IS NOT NULL
							)
							AND ( @PersonRoleNames IS NULL
								  OR ISNULL(PR.Name, '') IN (
								  SELECT    RoleName
								  FROM      @PersonRoleNamesTable )
								)
						GROUP BY	P.personid,
									P.LastName ,
									ISNULL(P.PreferredFirstName,P.FirstName) ,
									P.IsOffshore ,
									P.EmployeeNumber,
									TEP.TimeTypeName,
									TEP.TimeTypeCode ,
									isnull(TEP.ChargeCodeDate,PFR.Date) ,
									TEP.TimeTypeId ,
									TEP.TimeEntrySectionId,
									TEP.Note,
									PFR.MinimumValue,
									PFR.MaximumValue,
									PFR.BillRate,
									PR.Name
						ORDER BY  P.LastName ,
								  ISNULL(P.PreferredFirstName,P.FirstName) ,
								  isnull(TEP.ChargeCodeDate,PFR.Date) ,
								  TEP.TimeTypeName
			END
		ELSE 
			BEGIN
				RAISERROR('There is no Project with this Project Number.', 16, 1)
			END
		END
	END

