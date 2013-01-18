CREATE PROCEDURE [dbo].[TimeEntriesGetByPersonsForExcel]
(   @PersonIds	NVARCHAR(MAX),
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimescaleIds NVARCHAR(4000)= NULL,
	@PracticeIds  NVARCHAR(MAX) = NULL
)
AS
BEGIN
  
  SET NOCOUNT ON;

  DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME ,
			@ORTTimeTypeId INT ,
			@HolidayTimeType INT ,
			@UnpaidTimeTypeId	INT,
			@FutureDate DATETIME

	SELECT  @StartDateLocal = CONVERT(DATE, @StartDate),
			@EndDateLocal = CONVERT(DATE, @EndDate),
			@ORTTimeTypeId = dbo.GetORTTimeTypeId(),
			@HolidayTimeType = dbo.GetHolidayTimeTypeId(),
			@UnpaidTimeTypeId = dbo.GetUnpaidTimeTypeId(),
			@FutureDate = dbo.GetFutureDate()
    
	DECLARE @PersonList TABLE (Id INT PRIMARY KEY)
	INSERT INTO @PersonList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PersonIds)
	
	--@TimescaleIds is null means all timescales.
	IF @TimescaleIds IS NOT NULL
	BEGIN
		DECLARE @TimescaleIdList TABLE (Id int PRIMARY KEY)
		INSERT INTO @TimescaleIdList
		SELECT * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
	END	
	DECLARE @PracticeIdsList TABLE (Id INT  PRIMARY KEY)
	INSERT INTO @PracticeIdsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)

    ;WITH PersonsFilteredByPersonIdsAndPayIds AS
	(
		SELECT DISTINCT P.PersonId
						, P.FirstName
						, P.LastName
						, P.EmployeeNumber
		FROM dbo.Person P
		INNER JOIN @PersonList PL ON PL.Id = P.PersonId
		LEFT JOIN Pay pa ON pa.Person = P.PersonId  AND pa.StartDate <= @EndDate AND (ISNULL(pa.EndDate, @FutureDate) -1) >= @StartDate
		WHERE (@TimescaleIds IS NULL OR pa.Timescale IN (SELECT Id FROM @TimescaleIdList))
		      AND (@PracticeIds IS NULL) OR ISNULL(pa.PracticeId,P.DefaultPractice) IN (SELECT Id FROM @PracticeIdsList)
	)
	SELECT p.EmployeeNumber AS 'Employee Id',
		   p.LastName +', ' +p.FirstName AS Name,		   
		   C.Name AS Account,
		   PG.Name AS [Business Unit],
		   PROJ.ProjectNumber AS [P#],
		   PROJ.Name AS [Project Name],
		   TT.Name AS [Work Type],
		   TE.ChargeCodeDate AS [Date],
		   (CASE WHEN ( TT.TimeTypeId = @ORTTimeTypeId
								  OR TT.TimeTypeId = @HolidayTimeType
								  OR TT.TimeTypeId = @UnpaidTimeTypeId
								)
						   THEN TE.Note
								+ dbo.GetApprovedByName(TE.ChargeCodeDate,
														TT.TimeTypeId,
														p.PersonId)
						   ELSE TE.Note
					  END ) AS Note,
		 ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 1 AND PROJ.ProjectNumber != 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [BillableHours],
		 ROUND(SUM(CASE
					WHEN TEH.IsChargeable = 0 OR PROJ.ProjectNumber = 'P031000' THEN
						TEH.ActualHours
					ELSE
						0
				END), 2) AS [NonBillableHours] ,
          ROUND(SUM(TEH.ActualHours), 2) AS [Total Hours]
	FROM PersonsFilteredByPersonIdsAndPayIds P
	INNER JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId
									AND TE.ChargeCodeDate BETWEEN ISNULL(@StartDate, te.ChargeCodeDate) and ISNULL(@EndDate, te.ChargeCodeDate)
    INNER JOIN dbo.TimeEntryHours AS TEH ON TE.TimeEntryId = TEH.TimeEntryId
	INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId				
	INNER JOIN dbo.Project AS PROJ ON PROJ.ProjectId = CC.ProjectId
	INNER JOIN dbo.Client AS C ON C.ClientId = CC.ClientId
	INNER JOIN dbo.TimeType AS TT ON TT.TimeTypeId = CC.TimeTypeId
	INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
	GROUP BY p.PersonId,
			 TE.ChargeCodeDate,
			 TE.ChargeCodeId,
		     PROJ.ProjectNumber,
			 TT.TimeTypeId,
		     PROJ.Name,
		     C.Name,
		     TT.Name,
		     TE.Note,
			 PG.Name,
			 p.LastName,
			 p.FirstName,
			 p.EmployeeNumber
	ORDER BY  p.LastName,p.FirstName,C.Name,PG.Name,PROJ.ProjectNumber,TT.Name,TE.ChargeCodeDate

END
GO
