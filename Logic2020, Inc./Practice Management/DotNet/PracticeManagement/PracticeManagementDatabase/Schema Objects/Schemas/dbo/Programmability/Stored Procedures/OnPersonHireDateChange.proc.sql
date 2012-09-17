CREATE PROCEDURE [dbo].[OnPersonHireDateChange]
(
	@PersonId        INT,
	@NewHireDate	 DATETIME
)
AS
/*
A.Update The person Compensation 
1.Get the previous termination date of the person
2.Delete all the compensation records that are before the new hire date and greater than the previous termination date
3.Get the latest start date of the compensation record after the previous termination date
4.Update the latest compensation start date with the hire date.

B.Need to adjust  person status history record if the person is hired for the first time

*/
BEGIN
--a.Update The person Compensation 
	DECLARE @PreviousTerminationDate DATETIME ,@LatestCompensationStartDate DATETIME

	--1.Get the previous termination date of the person
	SELECT Top 1 @PreviousTerminationDate = VP.TerminationDate 
	FROM dbo.v_PersonHistory VP
	WHERE VP.TerminationDate < @NewHireDate
			AND VP.PersonId = @PersonId 
	ORDER BY VP.HireDate DESC

	--2.Delete all the compensation records that are before the new hire date and greater than the previous termination date
	DELETE pay
	FROM dbo.pay pay 
	WHERE Pay.Person = @PersonId 
			AND ( @PreviousTerminationDate  IS NULL OR pay.StartDate > @PreviousTerminationDate) -- Compensation After the previous termination date
			AND Pay.EndDate-1 < @NewHireDate -- Compensation before new hire date


	--3.Get the latest start date of the compensation record after the previous termination date
	SELECT TOP 1 @LatestCompensationStartDate = pay.StartDate
	FROM dbo.pay 
	WHERE Pay.Person = @PersonId 
				--AND @NewHireDate < pay.StartDate 
				AND (@PreviousTerminationDate  IS NULL OR pay.StartDate > @PreviousTerminationDate) -- Compensation After the previous termination date
				--AND	@NewHireDate BETWEEN pay.StartDate AND pay.EndDate-1
	ORDER BY pay.StartDate

	--4.Update the latest compensation start date with the hire date.
	UPDATE pay
	SET Pay.StartDate = @NewHireDate
	FROM dbo.pay pay
	WHERE Pay.Person = @PersonId 
			AND Pay.StartDate = @LatestCompensationStartDate

	--B.Need to adjust  person status history record if the person is hired for the first time

	;WITH IsFirstHire AS
	(
		SELECT PH.PersonId, MIN(PH.HireDate) 'HireDate', MIN(PH.CreatedDate) AS 'CreatedDate', MIN(PH.TerminationDate) AS 'TerminationDate'
		FROM dbo.v_PersonHistory PH
		WHERE PH.PersonId = @PersonId
		GROUP BY PH.PersonId
		HAVING COUNT(*) = 1 AND MIN(PH.TerminationDate) IS NULL
	)
	,FirstPSHRow  AS
	(
		SELECT MIN(StartDate) AS StartDate,
			@PersonId AS PersonId,
			CONVERT(DATE, MIN(FH.CreatedDate)) AS CreatedDate,
			CONVERT(DATE, MIN(FH.HireDate)) AS HireDate
		FROM dbo.PersonStatusHistory AS PSH
		INNER JOIN IsFirstHire FH ON FH.PersonId = PSH.PersonId
	)

	UPDATE PSH
	SET PSH.startdate = CASE WHEN FPSHR.HireDate < FPSHR.CreatedDate THEN FPSHR.HireDate
										ELSE FPSHR.CreatedDate END
	FROM FirstPSHRow AS FPSHR
	INNER JOIN dbo.PersonStatusHistory AS PSH ON PSH.PersonId = FPSHR.PersonId AND  FPSHR.startdate = PSH.startdate
	WHERE  (FPSHR.HireDate < FPSHR.CreatedDate AND FPSHR.HireDate <> PSH.StartDate) OR (FPSHR.HireDate > FPSHR.CreatedDate AND FPSHR.CreatedDate <> PSH.StartDate)

END
