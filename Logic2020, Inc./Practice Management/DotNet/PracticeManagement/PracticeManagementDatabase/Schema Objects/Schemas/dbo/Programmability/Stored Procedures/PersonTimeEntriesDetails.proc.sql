-- =============================================
-- Author:		ThulasiRam.P
-- Create date: 03-05-2012
-- Description: Person TimeEntries Details By Period.
-- Updated by : ThulasiRam.P
-- Update Date: 06-05-2012
-- =============================================
CREATE PROCEDURE [dbo].[PersonTimeEntriesDetails]
	(
	  @PersonId INT = NULL,
	  @StartDate DATETIME ,
	  @EndDate DATETIME
	)
AS 
	BEGIN

		SET NOCOUNT ON;

		DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@PersonIdLocal INT ,
			@ORTTimeTypeId INT ,
			@HolidayTimeType INT ,
			@UnpaidTimeTypeId	INT,
			@FutureDate DATETIME

		SELECT @StartDateLocal = CONVERT(DATE, @StartDate), @EndDateLocal = CONVERT(DATE, @EndDate),@PersonIdLocal = @PersonId,@ORTTimeTypeId = dbo.GetORTTimeTypeId(),@HolidayTimeType = dbo.GetHolidayTimeTypeId(),@FutureDate = dbo.GetFutureDate(),@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId()

		;WITH    PersonDayWiseByProjectsBillableTypes
				  AS ( SELECT   M.ProjectId ,
								MP.PersonId,
								C.Date ,
								MIN(CAST(M.IsHourlyAmount AS INT)) MinimumValue ,
								MAX(CAST(M.IsHourlyAmount AS INT)) MaximumValue
								,CASE WHEN MAX(CAST(m.IsHOurlyAmount AS INT)) = 1 AND MIN(CAST(m.IsHourlyAmount AS INT)) = 1 THEN SUM(MPE.Amount * MPE.HoursPerDay)/ SUM(MPE.HoursPerDay)
								ELSE NULL END HourlyRate
					   FROM     dbo.MilestonePersonEntry AS MPE
								INNER JOIN dbo.MilestonePerson AS MP ON MP.MilestonePersonId = MPE.MilestonePersonId
								INNER JOIN dbo.Milestone AS M ON M.MilestoneId = MP.MilestoneId
								INNER JOIN dbo.Calendar AS C ON C.Date BETWEEN MPE.StartDate AND MPE.EndDate
															  AND C.Date BETWEEN @StartDateLocal AND @EndDateLocal
					   WHERE    (@PersonIdLocal IS NULL OR MP.PersonId = @PersonIdLocal)
								AND M.StartDate < @EndDateLocal
								AND @StartDateLocal < M.ProjectedDeliveryDate
					   GROUP BY M.ProjectId ,
								MP.PersonId,
								C.Date
					 )

			SELECT  TE.PersonId,
					P.FirstName,
					P.LastName,
					CC.TimeEntrySectionId ,
					C.ClientId ,
					C.Name AS ClientName ,
					C.Code AS ClientCode ,
					BU.Name AS GroupName ,
					BU.Code AS GroupCode ,
					PRO.ProjectId ,
					PRO.Name AS ProjectName ,
					PRO.ProjectNumber ,
					( CASE WHEN ( CC.TimeEntrySectionId <> 1 ) THEN ''
						   ELSE PS.Name
					  END ) AS ProjectStatusName ,
					TT.TimeTypeId ,
					TT.Name AS TimeTypeName ,
					TT.Code AS TimeTypeCode ,
					TE.ChargeCodeDate ,
					( CASE WHEN ( PDBR.MinimumValue IS NULL
								  OR CC.TimeEntrySectionId <> 1
								) THEN ''
						   WHEN ( PDBR.MinimumValue = PDBR.MaximumValue
								  AND PDBR.MinimumValue = 0
								) THEN 'Fixed'
						   WHEN ( PDBR.MinimumValue = PDBR.MaximumValue
								  AND PDBR.MinimumValue = 1
								) THEN 'Hourly'
						   ELSE 'Both'
					  END ) AS BillingType ,
					 CASE WHEN CC.TimeEntrySectionId = 3 THEN NULL
							ELSE PDBR.HourlyRate END HourlyRate,
					( CASE WHEN ( TT.TimeTypeId = @ORTTimeTypeId
								  OR TT.TimeTypeId = @HolidayTimeType
								  OR TT.TimeTypeId = @UnpaidTimeTypeId
								)
						   THEN TE.Note
								+ dbo.GetApprovedByName(TE.ChargeCodeDate,
														TT.TimeTypeId,
														@PersonIdLocal)
						   ELSE TE.Note
					  END ) AS Note ,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 1
								   THEN TEH.ActualHours
								   ELSE 0
							  END), 2) AS BillableHours ,
					ROUND(SUM(CASE WHEN TEH.IsChargeable = 0
								   THEN TEH.ActualHours
								   ELSE 0
							  END), 2) AS NonBillableHours
			FROM    dbo.TimeEntry AS TE
					INNER JOIN dbo.TimeEntryHours AS TEH ON TEH.TimeEntryId = TE.TimeEntryId
					INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId
					INNER JOIN dbo.ProjectGroup BU ON BU.GroupId = CC.ProjectGroupId
					INNER JOIN dbo.Client C ON CC.ClientId = C.ClientId
					INNER JOIN dbo.Project PRO ON PRO.ProjectId = CC.ProjectId
					INNER JOIN dbo.ProjectStatus PS ON PRO.ProjectStatusId = PS.ProjectStatusId
					INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = CC.TimeTypeId
					INNER JOIN dbo.PersonStatusHistory PTSH ON TE.ChargeCodeDate BETWEEN PTSH.StartDate
															  AND ISNULL(PTSH.EndDate,@FutureDate) AND PTSH.PersonId = TE.PersonId
					INNER JOIN dbo.Person P ON P.PersonId = TE.PersonId
					LEFT JOIN PersonDayWiseByProjectsBillableTypes PDBR ON PDBR.ProjectId = CC.ProjectId AND TE.PersonId = PDBR.PersonId
															  AND PDBR.Date = TE.ChargeCodeDate
			WHERE   (@PersonIdLocal IS NULL OR TE.PersonId = @PersonIdLocal)
					AND TE.ChargeCodeDate BETWEEN @StartDateLocal
										  AND     @EndDateLocal
					AND ( CC.timeTypeId != @HolidayTimeType
						  OR ( CC.timeTypeId = @HolidayTimeType
							   AND PTSH.PersonStatusId IN (1,5)
							 )
						)
			GROUP BY TE.PersonId ,
					P.FirstName,
					P.LastName,
					CC.TimeEntrySectionId ,
					C.ClientId ,
					C.Name ,
					C.Code ,
					BU.Name ,
					BU.Code ,
					PRO.ProjectId ,
					PRO.Name ,
					PRO.ProjectNumber ,
					PS.Name ,
					TT.TimeTypeId ,
					TT.Name ,
					TT.Code ,
					TE.ChargeCodeDate ,
					TE.Note ,
					PDBR.MinimumValue ,
					PDBR.MaximumValue,
					PDBR.HourlyRate
			ORDER BY P.FirstName,
					P.LastName,
					CC.TimeEntrySectionId ,
					PRO.ProjectNumber ,
					TE.ChargeCodeDate ,
					TT.Name
	END	
	

