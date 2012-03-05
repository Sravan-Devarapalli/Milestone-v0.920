CREATE PROCEDURE [dbo].[PersonTimeEntriesByPeriod]
	@PersonId	INT,
	@StartDate	DATETIME,
	@EndDate	DATETIME
AS
BEGIN
	
	DECLARE @HolidayTimeTypeId	INT,
			@PTOTimeTypeId		INT,
			@IsW2SalaryPerson	BIT = 0,
			@W2SalaryId			INT,
			@FutureDateLocal	DATETIME,
			@StartDateLocal		DATETIME,
			@EndDateLocal		DATETIME,
			@ORTTimeTypeId		INT
			 
	SELECT @HolidayTimeTypeId = dbo.GetHolidayTimeTypeId(), 
		   @PTOTimeTypeId     = dbo.GetPTOTimeTypeId(),
		   @ORTTimeTypeId	  = dbo.GetORTTimeTypeId(),
		   @FutureDateLocal   = dbo.GetFutureDate(),
		   @StartDateLocal    = @StartDate,
		   @EndDateLocal      = @EndDate
	
	SELECT @W2SalaryId = TimescaleId 
	FROM Timescale WHERE Name = 'W2-Salary'

	SELECT @IsW2SalaryPerson = 1
	FROM dbo.Pay pay
	WHERE	pay.Person = @PersonId AND 
			pay.Timescale = @W2SalaryId AND 
			pay.StartDate <=  @EndDateLocal AND 
			ISNULL(pay.EndDate,@FutureDateLocal) >= @StartDateLocal



	--List Of time entries with detail
	SELECT TE.TimeEntryId,
			TE.ChargeCodeId,
			TE.ChargeCodeDate,
			TEH.CreateDate,
			TEH.ModifiedDate,
			TEH.ActualHours,
			TE.ForecastedHours,
			TE.Note,
			TEH.IsChargeable,
			TEH.ReviewStatusId,
			CC.TimeTypeId,
			TEH.ModifiedBy,
			PC.ApprovedBy 'ApprovedBy',
			AP.LastName 'ApprovedByLastName',
			AP.FirstName 'ApprovedByFirstName'
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.TimeEntryHours AS TEH  ON TE.TimeEntryId = TEH.TimeEntryId
	INNER JOIN ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
	LEFT JOIN PersonCalendar PC ON PC.PersonId = @PersonId AND PC.Date = TE.ChargeCodeDate AND PC.TimeTypeId = CC.TimeTypeId AND PC.TimeTypeId = @ORTTimeTypeId
	LEFT JOIN Person AP ON AP.PersonId = PC.ApprovedBy

	
	--List of Charge codes with recursive flag.
	SELECT DISTINCT ISNULL(CC.TimeEntrySectionId, PTRS.TimeEntrySectionId) AS 'TimeEntrySectionId',
		CC.Id AS 'ChargeCodeId', 
		ISNULL(CC.ClientId, PTRS.ClientId) AS 'ClientId', 
		C.Name 'ClientName',
		ISNULL(CC.ProjectGroupId, PTRS.ProjectGroupId) AS 'GroupId', 
		PG.Name 'GroupName',
		ISNULL(CC.ProjectId, PTRS.ProjectId) AS 'ProjectId',
		p.ProjectNumber, 
		P.Name 'ProjectName',
		ISNULL(CONVERT(NVARCHAR(1), PTRS.IsRecursive), 0) AS 'IsRecursive',
		P.EndDate
	FROM dbo.TimeEntry TE
	INNER JOIN dbo.ChargeCode CC ON CC.Id = TE.ChargeCodeId AND TE.PersonId = @PersonId AND TE.ChargeCodeDate BETWEEN @StartDateLocal AND @EndDateLocal
	FULL JOIN dbo.PersonTimeEntryRecursiveSelection PTRS 
		ON PTRS.ClientId = CC.ClientId AND ISNULL(PTRS.ProjectGroupId, 0) = ISNULL(CC.ProjectGroupId, 0) AND PTRS.ProjectId = CC.ProjectId AND StartDate < @EndDateLocal AND EndDate > @StartDateLocal 
	INNER JOIN Client C ON C.ClientId = ISNULL(CC.ClientId, PTRS.ClientId)
	LEFT JOIN ProjectGroup PG ON PG.GroupId = ISNULL(CC.ProjectGroupId, PTRS.ProjectGroupId)
	INNER JOIN Project P ON P.ProjectId = ISNULL(CC.ProjectId, PTRS.ProjectId)
	WHERE ISNULL(PTRS.PersonId, @PersonId) = @PersonId AND (CC.Id IS NULL AND PTRS.StartDate < @EndDateLocal AND ISNULL(PTRS.EndDate,dbo.GetFutureDate()) > @StartDateLocal) OR CC.Id IS NOT NULL
	UNION
	SELECT CC.TimeEntrySectionId AS 'TimeEntrySectionId',
		CC.Id AS 'ChargeCodeId', 
		CC.ClientId AS 'ClientId', 
		C.Name 'ClientName',
		CC.ProjectGroupId AS 'GroupId', 
		PG.Name 'GroupName',
		CC.ProjectId AS 'ProjectId', 
		p.ProjectNumber,
		P.Name 'ProjectName',
		0 AS 'IsRecursive',
		P.EndDate
	FROM ChargeCode CC
	INNER JOIN Client C ON C.ClientId = CC.ClientId AND CC.TimeEntrySectionId = 4 --Administrative Section 
						   AND ((CC.TimeTypeId = @HolidayTimeTypeId AND @IsW2SalaryPerson = 1) OR CC.TimeTypeId = @PTOTimeTypeId)
	INNER JOIN Project P ON P.ProjectId = CC.ProjectId
	INNER JOIN ProjectGroup PG ON PG.GroupId = CC.ProjectGroupId 

	--List of Charge codes with ISPTO and IsHoliday
	SELECT  CC.ProjectId AS 'ProjectId', 1 IsPTO, 0 IsHoliday, 0 IsORT
	FROM dbo.ChargeCode CC
	WHERE CC.TimeTypeId = @PTOTimeTypeId
	UNION ALL
	SELECT  CC.ProjectId AS 'ProjectId', 0 IsPTO, 1 IsHoliday, 0 IsORT
	FROM dbo.ChargeCode CC
	WHERE CC.TimeTypeId = @HolidayTimeTypeId AND @IsW2SalaryPerson = 1
	UNION ALL
	SELECT CC.ProjectId AS 'ProjectId', 0 IsPTO, 0 IsHoliday, 1 IsORT
	FROM dbo.ChargeCode CC
	WHERE CC.TimeTypeId = @ORTTimeTypeId

END

