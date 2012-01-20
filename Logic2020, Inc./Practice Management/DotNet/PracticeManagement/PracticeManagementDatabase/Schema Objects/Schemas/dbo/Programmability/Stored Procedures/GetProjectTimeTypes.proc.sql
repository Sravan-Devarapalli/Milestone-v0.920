﻿CREATE PROCEDURE [dbo].[GetProjectTimeTypes]
	@ProjectId	INT
AS
BEGIN
		;WITH DefaultTimeTypes AS
		(
			SELECT TT.TimeTypeId, TT.Name
			FROM dbo.TimeType TT
			WHERE TT.IsDefault = 1
		)
		, ProjectTimeTypes AS 
		(
			SELECT PT.ProjectId, PT.TimeTypeId, PT.IsAllowedToShow, TT.Name
			FROM dbo.ProjectTimeType PT
			JOIN TimeType TT ON TT.TimeTypeId = PT.TimeTypeId
			WHERE PT.ProjectId = @ProjectId
		)
		, ProjectTimeTypesInUse AS 
		(
			SELECT DISTINCT TimeTypeId
			FROM CHARGECODE cc 
			INNER JOIN TIMETRACK te ON cc.ID = te.ChargeCodeId AND cc.ProjectID = @ProjectId
		)

		--Configure select columns.
		SELECT ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) AS 'TimeTypeId', ISNULL(PTT.Name, DTT.Name) AS 'Name' ,
			CASE WHEN ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) IN (SELECT * FROM ProjectTimeTypesInUse) THEN 1 ELSE 0 END AS InUse
		FROM ProjectTimeTypes PTT 
		FULL JOIN DefaultTimeTypes DTT ON DTT.TimeTypeId = PTT.TimeTypeId
		WHERE ISNULL(IsAllowedToShow, 1) = 1

END

