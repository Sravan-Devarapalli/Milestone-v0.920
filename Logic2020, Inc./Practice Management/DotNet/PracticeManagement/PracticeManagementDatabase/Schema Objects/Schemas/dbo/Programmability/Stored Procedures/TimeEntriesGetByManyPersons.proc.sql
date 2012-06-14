CREATE PROCEDURE [dbo].[TimeEntriesGetByManyPersons]
(
	@PersonId	INT,
	@StartDate	DATETIME = NULL,
	@EndDate	DATETIME = NULL,
	@TimescaleIds NVARCHAR(4000)= NULL,
	@PracticeIds  NVARCHAR(MAX)
)	
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;
	
	--@TimescaleIds is null means all timescales.
	IF @TimescaleIds IS NOT NULL
	BEGIN
		DECLARE @TimescaleIdList TABLE (Id INT)
		INSERT INTO @TimescaleIdList
		SELECT * FROM dbo.ConvertStringListIntoTable(@TimescaleIds)
	END
	
	DECLARE @PracticeIdsList table (Id int)
	INSERT INTO @PracticeIdsList
	SELECT * FROM dbo.ConvertStringListIntoTable(@PracticeIds)
	
	;WITH PersonsFilteredByPersonIdsAndPayIds AS
	(
		SELECT P.PersonId
		FROM Person P
		LEFT JOIN dbo.Pay pa ON pa.Person = P.PersonId AND pa.StartDate <= @EndDate AND (ISNULL(pa.EndDate, dbo.GetFutureDate()) -1) >= @StartDate
		WHERE (@TimescaleIds IS NULL OR pa.Timescale IN (SELECT Id FROM @TimescaleIdList)) 
		      AND ((@PracticeIds IS NULL) OR ISNULL(pa.PracticeId,P.DefaultPractice) IN (SELECT Id FROM @PracticeIdsList)) 
			  AND (P.PersonId = @PersonId)
	)
	SELECT PROJ.ProjectNumber,
		   PROJ.Name ProjectName,
		   C.Name ClientName,
		   TT.Name TimeTypeName,
		   PG.Name AS GroupName,
		   TE.Note ,
		   TE.ChargeCodeDate,
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
		   TE.ChargeCodeId
	FROM PersonsFilteredByPersonIdsAndPayIds P
	INNER JOIN dbo.TimeEntry TE ON TE.PersonId = P.PersonId
									AND TE.ChargeCodeDate BETWEEN ISNULL(@StartDate, te.ChargeCodeDate) and ISNULL(@EndDate, te.ChargeCodeDate)
    INNER JOIN dbo.TimeEntryHours AS TEH ON TE.TimeEntryId = TEH.TimeEntryId
	INNER JOIN dbo.ChargeCode AS CC ON CC.Id = TE.ChargeCodeId				
	INNER JOIN dbo.Project AS PROJ ON PROJ.ProjectId = CC.ProjectId
	INNER JOIN dbo.Client AS C ON C.ClientId = CC.ClientId
	INNER JOIN dbo.TimeType AS TT ON TT.TimeTypeId = CC.TimeTypeId
	INNER JOIN dbo.ProjectGroup AS PG ON PG.GroupId = CC.ProjectGroupId
	GROUP BY TE.ChargeCodeDate,
			 TE.ChargeCodeId,
		     PROJ.ProjectNumber,
		     PROJ.Name,
		     C.Name,
		     TT.Name,
		     TE.Note,
			 PG.Name
	ORDER BY PROJ.ProjectNumber, TE.ChargeCodeDate

	SELECT P.FirstName,
		   P.LastName
	FROM dbo.Person AS P
	WHERE (P.PersonId = @PersonId)
	 
END

