CREATE PROCEDURE [dbo].[GetPeopleAssignedInOtherProjectsForGivenRange]
(
	@MilestoneNewStartDate  DATETIME,
	@MilestoneNewEndDate	DATETIME,
	@MilestoneId			INT
)
AS
BEGIN

	DECLARE @MilestoneOldStartDate DATETIME
	SELECT @MilestoneOldStartDate = StartDate FROM dbo.Milestone WHERE MilestoneId = @MilestoneId

	DECLARE @DefaultStartDate DATETIME = '20140701'

	DECLARE @BadgePeople TABLE(Id INT,NewBadgeBreakStartDate DATETIME,NewBadgeBreakEndDate DATETIME)

	INSERT INTO @BadgePeople
	SELECT DISTINCT MP.PersonId, CASE WHEN @MilestoneNewStartDate >= @DefaultStartDate THEN DATEADD(MM,18,@MilestoneNewStartDate) 
									  ELSE DATEADD(MM,18,@DefaultStartDate) END,
								 CASE WHEN @MilestoneNewStartDate >= @DefaultStartDate THEN DATEADD(MM,24,@MilestoneNewStartDate)-1 
									  ELSE DATEADD(MM,24,@DefaultStartDate)-1 END
	FROM dbo.Milestone M 
		 JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId 
		 JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
		 JOIN dbo.Project P ON P.ProjectId = M.ProjectId
		 JOIN ( 
				SELECT DISTINCT MPI.PersonId 
				FROM dbo.Milestone MI
				JOIN dbo.MilestonePerson MPI ON MPI.MilestoneId = MI.MilestoneId
				JOIN dbo.MilestonePersonEntry MPEI ON MPEI.MilestonePersonId = MPI.MilestonePersonId
				JOIN dbo.Project PP ON PP.ProjectId = MI.ProjectId
				WHERE MI.MilestoneId <> @MilestoneId
					  AND MPEI.BadgeStartDate IS NOT NULL AND MPEI.BadgeEndDate IS NOT NULL AND MPEI.IsApproved = 1
					  AND PP.ProjectStatusId IN (1,2,3,4)
			  ) OtherProjects ON OtherProjects.PersonId = MP.PersonId
	WHERE M.MilestoneId = @MilestoneId
		  AND MPE.BadgeStartDate IS NOT NULL AND MPE.BadgeEndDate IS NOT NULL AND MPE.IsApproved = 1
		  AND P.ProjectStatusId IN (1,2,3,4)
		  AND @MilestoneNewStartDate < M.StartDate
		  AND @MilestoneNewEndDate >= @DefaultStartDate

	SELECT MP.PersonId,Per.FirstName,Per.LastName,MPE.BadgeStartDate,MPE.BadgeEndDate,P.ProjectId,P.Name AS ProjectName,P.ProjectNumber
	FROM dbo.Milestone M
	JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	JOIN dbo.Project P ON P.ProjectId = M.ProjectId
	JOIN @BadgePeople B ON B.Id = MP.PersonId
	JOIN dbo.Person Per ON Per.PersonId = MP.PersonId
	WHERE M.MilestoneId <> @MilestoneId
			AND MPE.BadgeStartDate IS NOT NULL AND MPE.BadgeEndDate IS NOT NULL AND MPE.IsApproved = 1
			AND P.ProjectStatusId IN (1,2,3,4)
			AND (MPE.BadgeStartDate <= B.NewBadgeBreakEndDate AND B.NewBadgeBreakStartDate <= MPE.BadgeEndDate)
		
END
