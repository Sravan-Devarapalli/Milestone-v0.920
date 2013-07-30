--Need to call this sproc whenever person hire date,termination date,division changed.
CREATE PROCEDURE [dbo].[SetCommissionsAttributions]
(
	@PersonId INT
)
AS
BEGIN
	DECLARE @PersonStatusId		INT,
			@ConsultingDivId	INT,
			@BusinessDevelopmentDivId INT,
	  		@W2SalaryId			INT,
			@W2HourlyId			INT
			
	  SELECT	@W2SalaryId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Salary'
	  SELECT	@W2HourlyId = TimescaleId FROM dbo.Timescale WHERE Name = 'W2-Hourly'
	  SELECT    @ConsultingDivId = DivisionId FROM dbo.PersonDivision WHERE DivisionName = 'Consulting'
	  SELECT    @BusinessDevelopmentDivId = DivisionId FROM dbo.PersonDivision WHERE DivisionName = 'Business Development'

	  --Deleting Attribution Records based on Hire date and Termination date.		
		DELETE A 
		FROM	dbo.Attribution A
		LEFT JOIN v_PersonHistory PH ON PH.PersonId = A.TargetId AND (PH.TerminationDate IS NULL OR A.StartDate <= PH.TerminationDate) AND (PH.HireDate <= A.EndDate)
		WHERE A.AttributionRecordTypeId = 1 AND PH.PersonId IS NULL AND A.TargetId = @PersonId
		
		--Deleting Attribution Records based on Division History.
		DELETE A 
		FROM	dbo.Attribution A
		LEFT JOIN v_DivisionHistory DH ON DH.PersonId = A.TargetId AND (DH.EndDate IS NULL OR A.StartDate < DH.EndDate) AND (DH.StartDate <= A.EndDate) AND DH.DivisionId IN (@ConsultingDivId,@BusinessDevelopmentDivId)
		WHERE A.AttributionRecordTypeId = 1 AND DH.PersonId IS NULL AND A.TargetId = @PersonId
		
		--Updating Attribution Records based on Hire date and Termination date. 
		;WITH UpdatableAttributions
		AS
		(
		SELECT *,RANK() over (PARTITION	by A.AttributionId order by PH.HireDate) as Rank
		FROM	dbo.Attribution A
		INNER JOIN v_PersonHistory PH ON PH.PersonId = A.TargetId AND (PH.TerminationDate IS NULL OR A.StartDate <= PH.TerminationDate) AND (PH.HireDate <= A.EndDate)
		WHERE A.AttributionRecordTypeId = 1 AND A.TargetId = @PersonId
		)
		UPDATE A
		SET A.StartDate = CASE WHEN A.StartDate > UA.HireDate THEN A.StartDate ELSE UA.HireDate END,
			A.EndDate = CASE WHEN UA.TerminationDate IS NULL OR A.EndDate < UA.TerminationDate  THEN A.EndDate ELSE UA.TerminationDate END
	 	FROM UpdatableAttributions UA
		INNER JOIN dbo.Attribution A ON A.AttributionId = UA.AttributionId 
		WHERE UA.Rank = 1 AND 
							( 
								A.StartDate <> CASE WHEN A.StartDate > UA.HireDate THEN A.StartDate ELSE UA.HireDate END 
								OR 
								 A.EndDate <> CASE WHEN UA.TerminationDate IS NULL OR A.EndDate < UA.TerminationDate  THEN A.EndDate ELSE UA.TerminationDate END
							 )

		--Updating Attribution Records based on Division History.
		;WITH UpdatableDivisionAttributions
		AS
		(
		SELECT A.AttributionId,A.StartDate AStartDate,A.EndDate AEndDate,DH.StartDate DStartDate,DH.EndDate DEndDate,RANK() over (PARTITION	by A.AttributionId order by DH.StartDate) as Rank
		FROM	dbo.Attribution A
		INNER JOIN v_DivisionHistory DH ON DH.PersonId = A.TargetId AND (DH.EndDate IS NULL OR A.StartDate < DH.EndDate) AND (DH.StartDate <= A.EndDate) AND DH.DivisionId IN (@ConsultingDivId,@BusinessDevelopmentDivId)
		WHERE A.AttributionRecordTypeId = 1 AND A.TargetId = @PersonId
		)
		UPDATE A
		SET A.StartDate = CASE WHEN A.StartDate > UA.DStartDate THEN A.StartDate ELSE UA.DStartDate END,
			A.EndDate = CASE WHEN UA.DEndDate IS NULL OR A.EndDate < DATEADD(dd,-1,UA.DEndDate)  THEN A.EndDate ELSE DATEADD(dd,-1,UA.DEndDate) END
	 	FROM UpdatableDivisionAttributions UA
		INNER JOIN dbo.Attribution A ON A.AttributionId = UA.AttributionId 
		WHERE UA.Rank = 1 
		AND 
			( 
				A.StartDate <> CASE WHEN A.StartDate > UA.DStartDate THEN A.StartDate ELSE UA.DStartDate END
				OR 
				A.EndDate <> CASE WHEN UA.DEndDate IS NULL OR A.EndDate < DATEADD(dd,-1,UA.DEndDate)  THEN A.EndDate ELSE DATEADD(dd,-1,UA.DEndDate) END
			)
END
