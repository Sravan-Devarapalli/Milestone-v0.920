--Need to call this sproc whenever person hire date,termination date,division changed.
CREATE PROCEDURE [dbo].[SetCommissionsAttributions]
(
	@PersonId INT
)
AS
BEGIN
	
	  --Deleting Attribution Records based on pay/division/employment history		
		DELETE A 
		FROM	dbo.Attribution A
		LEFT JOIN dbo.[v_PersonValidAttributionRange] PH ON PH.PersonId = A.TargetId 
		WHERE A.AttributionRecordTypeId = 1 AND (A.StartDate <= PH.Enddate) AND (PH.Startdate <= A.EndDate) AND PH.PersonId IS NULL AND A.TargetId = @PersonId
		
		--Updating Attribution Records based on pay/division/employment history. 
		;WITH UpdatableAttributions
		AS
		(
		SELECT  A.AttributionId,
				A.StartDate AS AStartDate,
				A.EndDate AS AEndDate,
				PH.PersonId,
				PH.Startdate,
				PH.Enddate,
				RANK() over (PARTITION	by A.AttributionId order by PH.StartDate) as Rank
		FROM	dbo.Attribution A
		INNER JOIN dbo.[v_PersonValidAttributionRange] PH ON PH.PersonId = A.TargetId AND (A.StartDate <= PH.Enddate) AND (PH.Startdate <= A.EndDate)
		WHERE A.AttributionRecordTypeId = 1 AND A.TargetId = @PersonId
		)
		UPDATE A
		SET A.StartDate = CASE WHEN A.StartDate > UA.Startdate THEN A.StartDate ELSE UA.Startdate END,
			A.EndDate = CASE WHEN A.EndDate < UA.Enddate  THEN A.EndDate ELSE UA.Enddate END
	 	FROM UpdatableAttributions UA
		INNER JOIN dbo.Attribution A ON A.AttributionId = UA.AttributionId 
		WHERE UA.Rank = 1 AND 
							( 
								A.StartDate <> CASE WHEN A.StartDate > UA.Startdate THEN A.StartDate ELSE UA.Startdate END 
								OR 
								 A.EndDate <> CASE WHEN A.EndDate < UA.Enddate  THEN A.EndDate ELSE UA.Enddate END
							 )
		OPTION (RECOMPILE);
END

