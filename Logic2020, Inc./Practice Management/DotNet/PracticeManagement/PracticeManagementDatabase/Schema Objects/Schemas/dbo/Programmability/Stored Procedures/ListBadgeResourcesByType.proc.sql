CREATE PROCEDURE [dbo].[ListBadgeResourcesByType]
(
   @StartDate		DATETIME,
   @EndDate			DATETIME,
   @IsNotBadged		BIT=0,
   @IsClockNotStart	BIT=0,
   @IsBlocked		BIT=0,
   @IsBreak			BIT=0,
   @BadgedOnProject BIT=0
)
AS
BEGIN

	DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)
	IF(@BadgedOnProject =1)
	BEGIN
				SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,C.ClientId,C.Name AS ClientName,Pr.ProjectId,Pr.Name AS ProjectName,
					   Pr.ProjectNumber,Pr.StartDate,Pr.EndDate,MPE.BadgeStartDate AS ProjectBadgeStartDate,MPE.BadgeEndDate AS ProjectBadgeEndDate,MPE.IsApproved
				FROM dbo.MilestonePersonEntry MPE
				INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				INNER JOIN dbo.Milestone MS ON MS.MilestoneId = MP.MilestoneId
				INNER JOIN dbo.Project Pr ON Pr.ProjectId = MS.ProjectId
				INNER JOIN dbo.MSBadge M ON M.PersonId = MP.PersonId
				INNER JOIN v_Person P ON P.PersonId = M.PersonId 
				INNER JOIN dbo.Client C ON C.ClientId = Pr.ClientId
				WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND Pr.ProjectStatusId IN (1,2,3,4)
				
				UNION ALL
				SELECT P.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,2,'Microsoft',-1,'Previous MS Badge History','',M.LastBadgeStartDate,M.LastBadgeEndDate,M.LastBadgeStartDate,M.LastBadgeEndDate,1
				FROM dbo.MSBadge M
				INNER JOIN v_Person P ON P.PersonId = M.PersonId
				WHERE M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)

				UNION ALL
				SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,C.ClientId,C.Name AS ClientName,Pr.ProjectId,Pr.Name AS ProjectName,
					   Pr.ProjectNumber,Pr.StartDate,Pr.EndDate,MPE.BadgeStartDate AS ProjectBadgeStartDate,MPE.BadgeEndDate AS ProjectBadgeEndDate,0
				FROM dbo.MilestonePersonEntry MPE
				INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				INNER JOIN dbo.Milestone MS ON MS.MilestoneId = MP.MilestoneId
				INNER JOIN dbo.Project Pr ON Pr.ProjectId = MS.ProjectId
				INNER JOIN dbo.MSBadge M ON M.PersonId = MP.PersonId
				INNER JOIN v_Person P ON P.PersonId = M.PersonId 
				INNER JOIN dbo.Client C ON C.ClientId = Pr.ClientId
				WHERE ISNULL(mpe.IsbadgeRequired,0) = 0 AND (MPE.StartDate <= @EndDateLocal AND @StartDateLocal <= MPE.EndDate) AND Pr.ProjectNumber <> 'P031000' AND Pr.ProjectStatusId IN (1,2,3,4)
				AND M.PersonId IN (SELECT DISTINCT MP.PersonId
									FROM dbo.MilestonePersonEntry MPE
									INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
									INNER JOIN dbo.Milestone MS ON MS.MilestoneId = MP.MilestoneId
									INNER JOIN dbo.Project Pr ON Pr.ProjectId = MS.ProjectId
									WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND Pr.ProjectStatusId IN (1,2,3,4))
				ORDER BY P.LastName,P.FirstName
	END
    IF(@IsNotBadged=1)
	BEGIN
			;WITH BadgedOnProject
			 AS
			 (
				SELECT DISTINCT MP.PersonId
				FROM dbo.MilestonePersonEntry MPE
				INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
				INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
				WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

				UNION ALL
				SELECT DISTINCT M.PersonId
				FROM dbo.MSBadge M
				WHERE M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
			 )
			 SELECT DISTINCT M.PersonId,p.FirstName,p.LastName,m.BadgeStartDate,m.BadgeEndDate
			 FROM v_CurrentMSBadge M 
			 INNER JOIN v_Person P ON P.PersonId = M.PersonId 
			 INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
			 LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			 WHERE BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
			 ORDER BY P.LastName,P.FirstName
	END
	IF(@IsBlocked=1)
	BEGIN
	    ;WITH BadgedOnProject
		 AS
		 (
			SELECT DISTINCT MP.PersonId
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
		 )
		   SELECT M.PersonId,P.FirstName,P.LastName,P.Title,M.BlockStartDate,M.BlockEndDate
		   FROM dbo.MSBadge M 
		   INNER JOIN v_Person P ON P.PersonId = M.PersonId 
		   INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		   LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
		   LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = M.PersonId
		   WHERE (@StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal)
				 AND BP.PersonId IS NULL AND BNP.PersonId IS NULL
		   ORDER BY P.LastName,P.FirstName
	END
	IF(@IsBreak=1)
	BEGIN
		;WITH BadgedOnProject
		 AS
		 (
			SELECT DISTINCT MP.PersonId
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM v_CurrentMSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
		 ),
		 BlockedPeople
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM dbo.MSBadge M 
		   WHERE @StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal
		 )
		   SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BreakEndDate,M.BreakStartDate
		   FROM v_CurrentMSBadge M 
		   INNER JOIN v_Person P ON P.PersonId = M.PersonId 
		   INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		   LEFT JOIN BlockedPeople B ON B.PersonId = M.PersonId
		   LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId
		   LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = M.PersonId
		   WHERE (@StartDateLocal <= M.BreakEndDate AND M.BreakStartDate <= @EndDateLocal) AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL
		   ORDER BY P.LastName,P.FirstName
	END
	IF(@IsClockNotStart=1)
	BEGIN
		;WITH BadgedOnProject
		 AS
		 (
			SELECT DISTINCT MP.PersonId
			FROM dbo.MilestonePersonEntry MPE
			INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
			INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
			INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
			WHERE mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM v_CurrentMSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
		 ),
		 BlockedPeople
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM dbo.MSBadge M 
		   WHERE @StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal
		 ),
		  InBreakPeriod
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM v_CurrentMSBadge M 
		   WHERE @StartDateLocal <= M.BreakEndDate AND M.BreakStartDate <= @EndDateLocal
		 )
		  SELECT P.PersonId,P.FirstName,P.LastName,M.BadgeStartDate,M.BadgeEndDate
		   FROM v_Person P
		   LEFT JOIN v_CurrentMSBadge M ON M.PersonId = P.PersonId
		   INNER JOIN v_PersonHistory PH ON PH.PersonId = P.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		   LEFT JOIN InBreakPeriod Brk ON Brk.PersonId = P.PersonId
		   LEFT JOIN BlockedPeople B ON B.PersonId = P.PersonId
		   LEFT JOIN BadgedOnProject BP ON BP.PersonId = P.PersonId
		   LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = P.PersonId
		   WHERE (M.BadgeStartDate IS NULL OR @EndDateLocal < M.BadgeStartDate) AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND Brk.PersonId IS NULL
		   ORDER BY P.LastName,P.FirstName
	END
END
