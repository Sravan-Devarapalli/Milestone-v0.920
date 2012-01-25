CREATE PROCEDURE [dbo].[GetProjectTimeTypes]
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
			INNER JOIN dbo.TimeType TT ON TT.TimeTypeId = PT.TimeTypeId
			WHERE PT.ProjectId = @ProjectId
		)
		, ProjectTimeTypesInUse AS 
		(
			SELECT DISTINCT TimeTypeId
			FROM dbo.ChargeCode CC 
			INNER JOIN dbo.TimeEntry TE ON CC.ID = TE.ChargeCodeId AND CC.ProjectID = @ProjectId
		)

		--Configure select columns.
		SELECT ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) AS 'TimeTypeId'
			, ISNULL(PTT.Name, DTT.Name) AS 'Name'
			, CASE WHEN ISNULL(PTT.TimeTypeId, DTT.TimeTypeId) IN (SELECT * FROM dbo.ProjectTimeTypesInUse) THEN 1 ELSE 0 END AS InUse
		FROM dbo.ProjectTimeTypes PTT 
		FULL JOIN dbo.DefaultTimeTypes DTT ON DTT.TimeTypeId = PTT.TimeTypeId
		WHERE ISNULL(IsAllowedToShow, 1) = 1

END

