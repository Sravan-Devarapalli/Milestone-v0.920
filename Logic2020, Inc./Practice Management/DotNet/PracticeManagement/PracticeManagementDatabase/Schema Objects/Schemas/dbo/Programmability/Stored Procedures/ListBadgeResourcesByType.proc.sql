CREATE PROCEDURE [dbo].[ListBadgeResourcesByType]
(
   @PayTypeIds		NVARCHAR(MAX)=NULL,
   @PersonStatusIds NVARCHAR(MAX),
   @StartDate		DATETIME,
   @EndDate			DATETIME,
   @IsNotBadged		BIT=0,
   @IsClockNotStart	BIT=0,
   @IsBlocked		BIT=0,
   @IsBreak			BIT=0,
   @BadgedOnProject BIT=0,
   @IsBadgedException		BIT=0,
   @IsNotBadgedException	BIT=0
)
AS
BEGIN

	DECLARE @StartDateLocal DATETIME ,
			@EndDateLocal DATETIME

	DECLARE @PayTypeIdsTable TABLE ( Ids INT )
	DECLARE @PersonStatusIdsTable TABLE ( Ids INT )

	DECLARE @PayTypeIdsLocal	NVARCHAR(MAX),
			@PersonStatusIdsLocal NVARCHAR(MAX)
	SET @PayTypeIdsLocal = @PayTypeIds
	SET @PersonStatusIdsLocal = @PersonStatusIds

	INSERT INTO @PayTypeIdsTable( Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PayTypeIdsLocal)

	INSERT INTO @PersonStatusIdsTable(Ids)
	SELECT ResultId
	FROM dbo.ConvertStringListIntoTable(@PersonStatusIdsLocal)

	SELECT @StartDateLocal = CONVERT(DATE, @StartDate)
		 , @EndDateLocal = CONVERT(DATE, @EndDate)

	IF(@BadgedOnProject =1 OR @IsBadgedException = 1)
	BEGIN
				;WITH BadgedProjects
				AS
				(
					SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,C.ClientId,C.Name AS ClientName,Pr.ProjectId,Pr.Name AS ProjectName,
						   Pr.ProjectNumber,Pr.StartDate,Pr.EndDate,MPE.BadgeStartDate AS ProjectBadgeStartDate,MPE.BadgeEndDate AS ProjectBadgeEndDate,MPE.IsApproved,MPE.IsBadgeException
					FROM dbo.MilestonePersonEntry MPE
					INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
					INNER JOIN dbo.Milestone MS ON MS.MilestoneId = MP.MilestoneId
					INNER JOIN dbo.Project Pr ON Pr.ProjectId = MS.ProjectId
					INNER JOIN dbo.MSBadge M ON M.PersonId = MP.PersonId
					INNER JOIN v_Person P ON P.PersonId = M.PersonId 
					INNER JOIN dbo.Client C ON C.ClientId = Pr.ClientId
					INNER JOIN v_PersonHistory PH ON PH.PersonId = P.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
					LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
					WHERE M.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND Pr.ProjectStatusId IN (1,2,3,4)
						  AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
						  AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
				
					UNION ALL
					SELECT P.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,2,'Microsoft',-1,'Previous MS Badge History','',M.LastBadgeStartDate,M.LastBadgeEndDate,M.LastBadgeStartDate,M.LastBadgeEndDate,1,0
					FROM dbo.MSBadge M
					INNER JOIN v_Person P ON P.PersonId = M.PersonId
					INNER JOIN v_PersonHistory PH ON PH.PersonId = P.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
					LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
					WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
							AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
							AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))

					UNION ALL
					SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BadgeStartDate,M.BadgeEndDate,C.ClientId,C.Name AS ClientName,Pr.ProjectId,Pr.Name AS ProjectName,
						   Pr.ProjectNumber,Pr.StartDate,Pr.EndDate,MPE.BadgeStartDate AS ProjectBadgeStartDate,MPE.BadgeEndDate AS ProjectBadgeEndDate,0,0
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
										WHERE M.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND Pr.ProjectStatusId IN (1,2,3,4))
				),
				PeopleHavingException
				AS
				(
					SELECT PersonId
					FROM BadgedProjects
					GROUP BY PersonId
					HAVING MAX(IsBadgeException) = 1
				)
				SELECT BP.*
				FROM BadgedProjects BP
				LEFT JOIN PeopleHavingException PE ON PE.PersonId = BP.PersonId
				WHERE (@IsBadgedException = 0 OR (@IsBadgedException = 1 AND PE.PersonId IS NOT NULL))
				      AND
					  (@BadgedOnProject = 0 OR (@BadgedOnProject = 1 AND PE.PersonId IS NULL))
				ORDER BY BP.LastName,BP.FirstName
	END
    IF(@IsNotBadged=1 OR @IsNotBadgedException=1)
	BEGIN
			;WITH BadgedOnProject
			 AS
			 (
				SELECT DISTINCT MP.PersonId
				FROM dbo.MilestonePersonEntry MPE
				INNER JOIN dbo.MilestonePerson MP ON MP.MilestonePersonId = MPE.MilestonePersonId
				INNER JOIN dbo.Milestone M ON M.MilestoneId = MP.MilestoneId
				INNER JOIN dbo.Project P ON P.ProjectId = M.ProjectId
				INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
				WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

				UNION ALL
				SELECT DISTINCT M.PersonId
				FROM dbo.MSBadge M
				WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
			 )
			 SELECT DISTINCT M.PersonId,p.FirstName,p.LastName,m.BadgeStartDate,m.BadgeEndDate,m.DeactivatedDate
			 FROM v_CurrentMSBadge M 
			 INNER JOIN v_Person P ON P.PersonId = M.PersonId 
			 INNER JOIN dbo.MSBadge MB ON MB.PersonId = M.PersonId
			 INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
			 LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			 LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
			 WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
					AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
					AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
					AND (M.IsBlocked = 0 OR (M.IsBlocked = 1 AND (@StartDateLocal > M.BlockEndDate OR M.BlockStartDate > @EndDateLocal)))
					AND ((@IsNotBadgedException = 1 AND MB.IsException = 1 AND MB.ExceptionStartDate <= @EndDateLocal AND @StartDateLocal <= MB.ExceptionEndDate) 
							OR 
						 (@IsNotBadged = 1 AND (MB.IsException = 0 OR MB.ExceptionStartDate > @EndDateLocal OR @StartDateLocal > MB.ExceptionEndDate)))
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
			INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
			WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
			AND (M.IsBlocked = 0 OR (M.IsBlocked = 1 AND (@StartDateLocal > M.BlockEndDate OR M.BlockStartDate > @EndDateLocal)))
		 )
		   SELECT M.PersonId,P.FirstName,P.LastName,P.Title,M.BlockStartDate,M.BlockEndDate
		   FROM dbo.MSBadge M 
		   INNER JOIN v_Person P ON P.PersonId = M.PersonId 
		   INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		   LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
		   LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = M.PersonId
		   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
		   WHERE M.ExcludeInReports = 0 AND (@StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal)
				 AND BP.PersonId IS NULL AND BNP.PersonId IS NULL
				 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
				 AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
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
			INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
			WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM v_CurrentMSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
			AND (M.IsBlocked = 0 OR (M.IsBlocked = 1 AND (@StartDateLocal > M.BlockEndDate OR M.BlockStartDate > @EndDateLocal)))
		 ),
		 BlockedPeople
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM dbo.MSBadge M 
		   WHERE M.ExcludeInReports = 0 AND @StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal
		 )
		   SELECT DISTINCT M.PersonId,P.FirstName,P.LastName,P.Title,M.BreakEndDate,M.BreakStartDate
		   FROM v_CurrentMSBadge M 
		   INNER JOIN v_Person P ON P.PersonId = M.PersonId 
		   INNER JOIN v_PersonHistory PH ON PH.PersonId = M.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		   LEFT JOIN BlockedPeople B ON B.PersonId = M.PersonId
		   LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId
		   LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = M.PersonId
		   LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = M.PersonId
		   WHERE M.ExcludeInReports = 0 AND (@StartDateLocal <= M.BreakEndDate AND M.BreakStartDate <= @EndDateLocal) AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL
				 AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
				 AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
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
			INNER JOIN dbo.MSBadge MB ON MB.PersonId = MP.PersonId
			WHERE MB.ExcludeInReports = 0 AND mpe.IsbadgeRequired = 1 AND (MPE.BadgeStartDate <= @EndDateLocal AND @StartDateLocal <= MPE.BadgeEndDate) AND P.ProjectStatusId IN (1,2,3,4)

			UNION ALL
			SELECT DISTINCT M.PersonId
			FROM dbo.MSBadge M
			WHERE M.ExcludeInReports = 0 AND M.IsPreviousBadge = 1 AND (M.LastBadgeStartDate <= @EndDateLocal AND @StartDateLocal <= M.LastBadgeEndDate)
		 ),
		 BadgedNotOnProject
		 AS
		 (
			SELECT DISTINCT M.PersonId
			FROM v_CurrentMSBadge M 
			LEFT JOIN BadgedOnProject BP ON BP.PersonId = M.PersonId 
			WHERE M.ExcludeInReports = 0 AND BP.PersonId IS NULL AND (@StartDateLocal <= M.BadgeEndDate AND M.BadgeStartDate <= @EndDateLocal)
			AND (M.IsBlocked = 0 OR (M.IsBlocked = 1 AND (@StartDateLocal > M.BlockEndDate OR M.BlockStartDate > @EndDateLocal)))
		 ),
		 BlockedPeople
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM dbo.MSBadge M 
		   WHERE M.ExcludeInReports = 0 AND @StartDateLocal <= M.BlockEndDate AND M.BlockStartDate <= @EndDateLocal
		 ),
		  InBreakPeriod
		 AS
		 (
		   SELECT DISTINCT M.PersonId
		   FROM v_CurrentMSBadge M 
		   WHERE M.ExcludeInReports = 0 AND @StartDateLocal <= M.BreakEndDate AND M.BreakStartDate <= @EndDateLocal
		 )
		SELECT P.PersonId,P.FirstName,P.LastName,M.BadgeStartDate,M.BadgeEndDate
		FROM v_Person P
		LEFT JOIN MSBadge M ON M.PersonId = P.PersonId
		INNER JOIN v_PersonHistory PH ON PH.PersonId = P.PersonId AND PH.HireDate <= @EndDateLocal AND (PH.TerminationDate IS NULL OR @StartDateLocal <= PH.TerminationDate)
		LEFT JOIN InBreakPeriod Brk ON Brk.PersonId = P.PersonId
		LEFT JOIN BlockedPeople B ON B.PersonId = P.PersonId
		LEFT JOIN BadgedOnProject BP ON BP.PersonId = P.PersonId
		LEFT JOIN BadgedNotOnProject BNP ON BNP.PersonId = P.PersonId
		LEFT JOIN dbo.GetCurrentPayTypeTable() CP ON CP.PersonId = P.PersonId
		WHERE ISNULL(M.ExcludeInReports,0) = 0 AND BNP.PersonId IS NULL AND BP.PersonId IS NULL AND B.PersonId IS NULL AND Brk.PersonId IS NULL
			AND (@PayTypeIds IS NULL OR CP.Timescale IN (SELECT Ids FROM @PayTypeIdsTable))
			AND (PH.PersonStatusId IN (SELECT Ids FROM @PersonStatusIdsTable))
		ORDER BY P.LastName,P.FirstName
	END
END

