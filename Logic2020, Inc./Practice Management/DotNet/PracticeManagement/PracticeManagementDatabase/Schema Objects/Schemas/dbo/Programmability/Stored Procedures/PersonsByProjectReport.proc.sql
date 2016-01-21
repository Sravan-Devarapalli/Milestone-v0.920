CREATE PROCEDURE [dbo].[PersonsByProjectReport]
(
	@PayTypeIds			NVARCHAR(MAX)=NULL,
	@PersonStatusIds	NVARCHAR(MAX)=NULL,
	@Practices			NVARCHAR(MAX)=NULL,
	@ProjectStatusIds	NVARCHAR(MAX)=NULL,
	@AccountIds			NVARCHAR(MAX)=NULL,
	@ExcludeInternalPractices	BIT
)
AS
BEGIN

	DECLARE @PayIdsTable TABLE ( Ids INT )
	DECLARE	@PersonStatusIdsTable TABLE ( Ids INT )
	DECLARE	@PracticesTable TABLE ( Ids INT )
	DECLARE	@ProjectStatusTable TABLE ( Ids INT )
	DECLARE	@AccountIdsTable TABLE ( Ids INT )

	INSERT INTO @PayIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIds)

	INSERT INTO @PersonStatusIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIds)

	INSERT INTO @PracticesTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@Practices)

	INSERT INTO @ProjectStatusTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@ProjectStatusIds)

	INSERT INTO @AccountIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@AccountIds)

	SELECT	DISTINCT P.ProjectId,
			P.ProjectNumber,
			P.Name AS ProjectName,
			P.StartDate AS ProjectStartDate,
			P.EndDate AS ProjectEndDate,
			M.MilestoneId,
			M.StartDate AS MilestoneStartDate,
			M.ProjectedDeliveryDate AS MilestoneEndDate,
			M.Description,
			MPE.StartDate AS MilestoneResourceStartDate,
			MPE.EndDate AS MilestoneResourceEndDate,
			Per.PersonId,
			ISNULL(Per.PreferredFirstName,Per.FirstName) AS FirstName,
			Per.LastName,
			Per.TitleId,
			Per.Title,
			Per.PersonStatusId,
			Per.PersonStatusName,
			CMB.BadgeStartDate,
			CMB.BadgeEndDate,
			MB.OrganicBreakStartDate,
			MB.OrganicBreakEndDate,
			MB.BlockStartDate,
			MB.BlockEndDate
	FROM dbo.Project P
	JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
	JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	JOIN dbo.v_Person Per ON Per.PersonId = MP.PersonId 
	JOIN dbo.v_Pay pay ON pay.PersonId = Per.PersonId
	JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	JOIN dbo.v_CurrentMSBadge CMB ON CMB.PersonId = MP.PersonId
	JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
	JOIN dbo.Practice Pra ON Pra.PracticeId = P.PracticeId
	WHERE (@ProjectStatusIds IS NULL OR P.ProjectStatusId IN (SELECT Ids FROM @ProjectStatusTable))
		  AND (@AccountIds IS NULL OR P.ClientId IN (SELECT Ids FROM @AccountIdsTable))
		  AND (@PayTypeIds IS NULL OR (pay.Timescale IN (SELECT Ids FROM @PayIdsTable)))
		  AND (@PersonStatusIds IS NULL OR Per.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
		  AND (@Practices IS NULL OR P.PracticeId IN (SELECT Ids FROM @PracticesTable))
		  AND (@ExcludeInternalPractices = 0 OR (@ExcludeInternalPractices = 1 AND Pra.IsCompanyInternal = 0))
		  AND Per.IsStrawman = 0
	ORDER BY P.ProjectNumber,M.StartDate,m.Description,Per.LastName,ISNULL(Per.PreferredFirstName,Per.FirstName)
END

