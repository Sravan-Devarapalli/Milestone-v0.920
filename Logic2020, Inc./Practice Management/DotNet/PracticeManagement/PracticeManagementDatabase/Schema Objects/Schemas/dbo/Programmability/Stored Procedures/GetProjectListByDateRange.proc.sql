CREATE PROCEDURE [dbo].[GetProjectListByDateRange]
	@ShowProjected		BIT = 0,
	@ShowCompleted		BIT = 0,
	@ShowActive			BIT = 0,
	@showInternal		BIT = 0,
	@ShowExperimental	BIT = 0,
	@ShowInactive		BIT = 0,
	@StartDate			DATETIME,
	@EndDate			DATETIME
AS 
BEGIN
	SET NOCOUNT ON ;

DECLARE @DefaultProjectId INT
	SELECT @DefaultProjectId = ProjectId
	FROM dbo.DefaultMilestoneSetting

	SELECT  p.ClientId,
			p.ProjectId,
			p.Name,		
			p.StartDate,
			p.EndDate,
			p.ClientName,
			p.ProjectStatusId,
			p.ProjectStatusName,
			p.ProjectNumber,
			p.ProjectManagersIdFirstNameLastName,
			CASE WHEN A.ProjectId IS NOT NULL THEN 1 
					ELSE 0 END AS HasAttachments
	FROM	dbo.v_Project AS p
	OUTER APPLY (SELECT TOP 1 pa.ProjectId FROM ProjectAttachment pa WHERE pa.ProjectId = p.ProjectId) A
	WHERE	(dbo.IsDateRangeWithingTimeInterval(p.StartDate, p.EndDate, @StartDate, @EndDate) = 1)
			AND (    ( @ShowProjected = 1 AND p.ProjectStatusId = 2 )
				  OR ( @ShowActive = 1 AND p.ProjectStatusId = 3 )
				  OR ( @ShowCompleted = 1 AND p.ProjectStatusId = 4 )
				  OR ( @showInternal = 1 AND p.ProjectStatusId = 6 ) -- Internal
				  OR ( @ShowExperimental = 1 AND p.ProjectStatusId = 5 )
				  OR ( @ShowInactive = 1 AND p.ProjectStatusId = 1 ) -- Inactive
			)
			AND P.ProjectId <> @DefaultProjectId
    ORDER BY StartDate DESC

END
