CREATE PROCEDURE [dbo].[GetPersonMilestonesOnPreviousHireDate]
	(
		@PersonId	INT,
		@PreviousHireDate	DATETIME 
	)
AS
BEGIN

	SELECT DISTINCT P.ProjectId,
			M.MilestoneId,
		   P.ProjectNumber,
		   P.Name AS ProjectName,
		   M.Description 
	FROM dbo.Project P
 	JOIN dbo.Milestone M ON M.ProjectId = P.ProjectId
	JOIN dbo.MilestonePerson MP ON MP.MilestoneId = M.MilestoneId
	JOIN dbo.MilestonePersonEntry MPE ON MPE.MilestonePersonId = MP.MilestonePersonId
	WHERE MP.PersonId = @PersonId AND MPE.StartDate = @PreviousHireDate
	ORDER BY ProjectNumber
END
