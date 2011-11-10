CREATE PROCEDURE [dbo].[CheckTimeEntriesForMilestonePerson]
(
	@MilestonePersonId INT,
	@StartDate DATETIME = NULL,
	@EndDate DATETIME = NULL,
	@CheckStartDateEquality BIT = 1,
	@CheckEndDateEquality BIT =1
)
AS
BEGIN


DECLARE @EntriesCount INT

SELECT @EntriesCount =  MIN(CountOfMpeApplicable)
FROM (  
		SELECT COUNT(te.TimeEntryId) AS CountOfMpeApplicable
		FROM dbo.TimeEntries AS te
		LEFT JOIN  dbo.MilestonePersonEntry AS mpe 
		ON MPE.MilestonePersonId = TE.MilestonePersonId  
		AND  TE.MilestoneDate BETWEEN MPE.StartDate AND MPE.EndDate
		WHERE mpe.MilestonePersonId = @MilestonePersonId AND  TE.MilestoneDate BETWEEN @StartDate AND @EndDate
		GROUP BY te.TimeEntryId
) AS CTE


	IF (@EntriesCount = 1 AND EXISTS   (SELECT TOP 1 1 FROM dbo.TimeEntries AS te 
				 WHERE te.MilestonePersonId = @MilestonePersonId
					   AND ((te.MilestoneDate > @StartDate OR (te.MilestoneDate = @StartDate AND @CheckStartDateEquality =1))
							AND (te.MilestoneDate < @EndDate OR (te.MilestoneDate = @EndDate AND @CheckEndDateEquality =1))
							OR @StartDate IS NULL OR @EndDate IS NULL
							)
				))
	BEGIN
		SELECT CONVERT(BIT,1) Result 
	END
	ELSE
	BEGIN
		SELECT CONVERT(BIT,0) Result
	END
END

