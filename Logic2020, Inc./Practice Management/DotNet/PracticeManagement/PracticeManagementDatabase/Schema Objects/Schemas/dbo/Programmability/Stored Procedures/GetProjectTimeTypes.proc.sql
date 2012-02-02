﻿CREATE PROCEDURE [dbo].[GetProjectTimeTypes]
	@ProjectId		INT,
	@IsOnlyActive	BIT,
	@StartDate		DATETIME = NULL,
	@EndDate		DATETIME = NULL

AS
BEGIN
		;WITH DefaultTimeTypes AS
		(
			SELECT TT.TimeTypeId, TT.Name
			FROM dbo.TimeType TT 
			WHERE TT.IsDefault = 1 AND (TT.IsActive = @IsOnlyActive OR @IsOnlyActive = 0)
		)
		, ProjectTimeTypes AS 
		(
			SELECT PT.ProjectId, PT.TimeTypeId, PT.IsAllowedToShow, TT.Name
			FROM dbo.ProjectTimeType PT
			JOIN TimeType TT ON TT.TimeTypeId = PT.TimeTypeId AND (TT.IsActive = @IsOnlyActive OR @IsOnlyActive = 0)
			WHERE PT.ProjectId = @ProjectId
		)
		, ProjectTimeTypesInUse AS 
		(
			SELECT DISTINCT TimeTypeId
			FROM dbo.ChargeCode CC 
			INNER JOIN dbo.TimeEntry TE 
			  ON CC.ID = TE.ChargeCodeId 
				AND CC.ProjectID = @ProjectId
		)

		--Configure select columns.
		SELECT ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) AS 'TimeTypeId'
			, ISNULL(PTT.Name, DTT.Name) AS 'Name'
			, CASE WHEN ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) IN (SELECT * FROM ProjectTimeTypesInUse) THEN 1 ELSE 0 END AS 'InUse'
		FROM ProjectTimeTypes PTT 
		FULL JOIN DefaultTimeTypes DTT ON DTT.TimeTypeId = PTT.TimeTypeId
		LEFT JOIN dbo.ChargeCode CC  ON CC.ProjectId = @ProjectId AND ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) = CC.TimeTypeId 
		LEFT JOIN dbo.ChargeCodeTurnOffHistory CCH ON CC.Id = CCH.ChargeCodeId
		WHERE ISNULL(IsAllowedToShow, 1) = 1 
		AND ( 
				( @StartDate IS NULL 
					AND @EndDate IS NULL 
				) 
				OR ( CCH.ChargeCodeId IS NULL 
						OR NOT ( CCH.StartDate <= @StartDate 
									AND @EndDate <= ISNULL(CCH.EndDate,dbo.GetFutureDate())
							   )	
					)
			)
		ORDER BY 2

END

