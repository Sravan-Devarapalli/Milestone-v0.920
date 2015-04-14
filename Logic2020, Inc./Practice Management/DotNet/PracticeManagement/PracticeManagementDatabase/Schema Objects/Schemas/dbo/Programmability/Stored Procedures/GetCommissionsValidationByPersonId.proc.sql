CREATE PROCEDURE [dbo].[GetCommissionsValidationByPersonId]
	(
		@PersonId  INT,
		@HireDate DATETIME,
		@TerminationDate DATETIME = NULL,
		@PersonStatusId INT,
		@DivisionId INT,
		@IsReHire BIT = 0
	)
AS
BEGIN

	  DECLARE	@W2SalaryId			INT,
				@W2HourlyId			INT,
				@ProjectStartDate	DATETIME,
				@ProjectEndDate		DATETIME,
				@CurrentDate		DATETIME

	  SELECT	@W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	  SELECT	@W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
	  SELECT	@CurrentDate = dbo.GettingPMTime(GETUTCDATE())

	  SET	@TerminationDate = CASE WHEN @TerminationDate < @CurrentDate THEN @TerminationDate ELSE NULL END

	  ;WITH PersonDivisionCTE
	  AS
	  (
		SELECT d.DivisionId,d.PersonId,d.StartDate,d.EndDate,RANK() OVER (PARTITION BY d.personid ORDER BY d.startdate DESC) AS personRank
		FROM  v_DivisionHistory d
		UNION ALL
		SELECT @DivisionId,@PersonId,GETDATE(),NULL,-1
		FROM Person p 
		WHERE p.PersonId = @PersonId AND p.DivisionId <> @DivisionId
	  ),
	  PersonDivisionCTE1
	  AS
	  (
	  SELECT p1.PersonId,p1.DivisionId,p1.StartDate,CASE WHEN p1.personRank = 1 AND EXISTS(SELECT 1 FROM PersonDivisionCTE p2 WHERE p2.personRank = -1)  THEN GETDATE() ELSE p1.EndDate END AS EndDate
	  FROM PersonDivisionCTE p1
	  )
	--Check Commissions Validations AS per #3160 
	--1.Any Attribution violations related to HireDate and termination date.
	SELECT DISTINCT P.ProjectNumber,
	P.Name
	FROM dbo.Attribution A
	INNER JOIN dbo.Project P ON A.ProjectId = P.ProjectId
	LEFT JOIN 
	(
		SELECT p.PersonId,p.HireDate,p.TerminationDate,p.PersonStatusId,p.DivisionId,RANK() OVER (PARTITION BY p.PersonId ORDER BY p.id desc) AS personrank
		FROM v_PersonHistory p
		UNION 
		SELECT @PersonId,@HireDate,@TerminationDate,@PersonStatusId,@DivisionId,-1
		FROM Person p 
		WHERE p.PersonId = @PersonId AND @IsReHire = 1
	) PH ON PH.PersonId = A.TargetId 
	AND 
	(
		( 
		@IsReHire = 0 AND (ph.personrank = 1 OR ((PH.PersonStatusId <> 2 OR PH.TerminationDate IS NULL OR PH.TerminationDate >= A.EndDate) AND PH.HireDate <= A.StartDate))
		AND
		(ph.personrank <> 1 OR ((@TerminationDate IS NULL OR @TerminationDate >= A.EndDate) AND @HireDate <= A.StartDate))
		)
	OR 
	(
		@IsReHire = 1
		AND
		((PH.PersonStatusId <> 2 OR PH.TerminationDate IS NULL OR PH.TerminationDate >= A.EndDate) AND PH.HireDate <= A.StartDate)
		)
	)    
	WHERE A.AttributionRecordTypeId = 1 AND ph.PersonId IS NULL AND A.TargetId = @PersonId
	
	UNION 
	--2.Any Attribution violations related to division.
	SELECT	DISTINCT P.ProjectNumber,
			P.Name
	FROM	dbo.Attribution A
	INNER JOIN dbo.Project P ON A.ProjectId = P.ProjectId
	INNER JOIN PersonDivisionCTE1 DH ON DH.PersonId = A.TargetId 
									AND (DH.StartDate <= A.EndDate AND (DH.EndDate IS NULL OR A.StartDate < DH.EndDate)) AND ISNULL(DH.DivisionId,0) NOT IN(SELECT DivisionId FROM dbo.PersonDivision WHERE DivisionName IN ('Consulting','Business Development','Technology Consulting','Business Consulting','Director'))
	WHERE A.AttributionRecordTypeId = 1 AND A.TargetId = @PersonId
	
END

