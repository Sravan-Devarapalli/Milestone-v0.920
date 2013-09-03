-- =============================================
-- Author:		Anatoliy Lokshin
-- Create date: 12-04-2008
-- Updated by:	
-- Update date: 
-- Description:	Retrives the filtered list of persons who are not in the Administration practice.
-- =============================================
CREATE PROCEDURE dbo.PersonListAllForMilestone
(
	@MilestonePersonId   INT,
	@StartDate           DATETIME,
    @EndDate             DATETIME
)
AS
	SET NOCOUNT ON

	DECLARE @FutureDate DATETIME

	SELECT @FutureDate = dbo.GetFutureDate()

	SELECT *
	FROM 
	(SELECT p.PersonId,
		   p.FirstName,
		   p.LastName,
		   p.IsDefaultManager,
		   p.HireDate,
		  p.IsStrawman AS IsStrawman
	FROM dbo.Person AS p
	INNER JOIN v_PersonHistory AS PH ON PH.PersonId = p.PersonId 
	LEFT JOIN dbo.Practice AS Pra ON Pra.PracticeId = p.DefaultPractice 
	WHERE 
	   @MilestonePersonId IS NULL 
	   AND PH.HireDate <= @EndDate AND (PH.TerminationDate IS NULL OR @StartDate <= PH.TerminationDate)
	   AND ((p.IsStrawman = 0 AND (p.DefaultPractice IS NOT NULL AND Pra.IsCompanyInternal = 0)) OR (p.IsStrawman = 1 AND p.PersonStatusId = 1))
	   

	UNION
	SELECT P.PersonId,
		   p.FirstName,
		   p.LastName,
		   p.IsDefaultManager,
		   p.HireDate,
		  p.IsStrawman AS IsStrawman
	FROM dbo.Person AS P 
	INNER JOIN dbo.MilestonePerson AS MP ON MP.PersonId = P.PersonId
	WHERE mp.MilestonePersonId = @MilestonePersonId) AS Per
	ORDER BY Per.LastName, Per.FirstName

